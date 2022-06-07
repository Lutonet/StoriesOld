using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stories.Data;
using Stories.Model;
using Stories.Services;
using Stories.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using static Stories.Tools.Settings;

namespace Stories.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailSender;
        private readonly ILogger<ExternalLoginModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ExternalLoginModel(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            ILogger<ExternalLoginModel> logger,
            IEmailService emailSender,
            ApplicationDbContext dbContext,
            RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
            _dbContext = dbContext;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ProviderDisplayName { get; set; }
        public IEnumerable<Country> Countries { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Date)]
            [Display(Name = "Date Of Birth")]
            public DateTime BirthDate { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public int CountryId { get; set; }

            [Required]
            [StringLength(64, ErrorMessage = "The {0} must be at least {2} and max {1} character long.", MinimumLength = 2)]
            [Display(Name = "Public Pseudonym")]
            public string DisplayedName { get; set; }

            [Display(Name = "Email Address")]
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(64, ErrorMessage = "The {0} must be at least {2} and max {1} character long.", MinimumLength = 2)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [DisplayName("Gender")]
            public Gender Gender { get; set; }

            [Required]
            [StringLength(64, ErrorMessage = "The {0} must be at least {2} and max {1} character long.", MinimumLength = 2)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required]
            [Phone]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }
        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            Countries = await _dbContext.Countries.ToListAsync();
            Countries = Countries.OrderBy(pr => pr.CountryName);
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl;
                ProviderDisplayName = info.ProviderDisplayName;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    Input = new InputModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    };
                }
                return Page();
            }
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                if ((from users in _dbContext.Users where users.DisplayedName.ToLower() == Input.DisplayedName.ToLower().Trim() select users).Count() > 0)
                {
                    ModelState.AddModelError("", "This name is already registered with other user");
                    return Page();
                }

                if ((from users2 in _dbContext.Users where users2.Email.ToLower() == Input.Email.ToLower().Trim() select users2).Count() > 0)
                {
                    ModelState.AddModelError("", "This Email is already registered in the system");
                    return Page();
                }

                if (!Dob.AllowedAge(Input.BirthDate))
                {
                    ModelState.AddModelError("", "Minimal allowed age is 13 years.");
                    return Page();
                }

                // Validations are OK, let enter the data

                // If user entered 0 at the beginning, we will remove it and add phone prefix
                if (Input.PhoneNumber.FirstOrDefault().Equals("0"))
                {
                    Input.PhoneNumber = (Input.PhoneNumber).Remove(0, 1);
                }

                string phoneNumber = (from country1 in _dbContext.Countries where country1.Id == Input.CountryId select country1.PhonePrefix).FirstOrDefaultAsync().ToString() + Input.PhoneNumber;
                var user = new User()
                {
                    UserName = (Input.Email).Trim().ToLower(),
                    DisplayedName = (Input.DisplayedName).Trim(),
                    FirstName = (Input.FirstName).Trim(),
                    LastName = (Input.LastName).Trim(),
                    Email = (Input.Email).Trim().ToLower(),
                    Gender = Input.Gender,
                    CountryId = Input.CountryId,
                    BirthDate = Input.BirthDate,
                    PhoneNumber = phoneNumber,
                };

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        try
                        {
                            await _userManager.AddToRoleAsync(user, Settings.Subscriber);
                            _logger.LogInformation("User" + user.Email + "Added to role of Subscriber");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("User" + user.Email + "Couldn't be added to the Subscriber group", ex);
                            return RedirectToPage("/Error");
                        }
                        var profileSecurity = new UserSettings()
                        {
                            UserId = user.Id
                        };

                        try
                        {
                            await _dbContext.UserSettings.AddAsync(profileSecurity);
                            await _dbContext.SaveChangesAsync();
                            _logger.LogInformation("Default set of access rights creted for user " + user.Email);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Creting default set of access rights failed for " + user.Email, ex);
                            return RedirectToPage("/Error");
                        }

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
    $"<h3 align = 'center'>Hello from Stories</h3><p>Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.</p>");

                        // If account confirmation is required, we need to show the link if we don't have a real email sender
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("./RegisterConfirmation", new { Email = Input.Email });
                        }

                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);

                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ProviderDisplayName = info.ProviderDisplayName;
            ReturnUrl = returnUrl;
            return Page();
        }
    }
}