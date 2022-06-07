using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using static Stories.Tools.Settings;

namespace Stories.Pages.My
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;
        private readonly IEmailService _emailSender;
        private readonly ILogger<RegisterModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        public string returnUrl;

        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<RegisterModel> logger,
            IConfiguration configuration,
            ApplicationDbContext dbContext,
            IEmailService emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _configuration = configuration;
            _dbContext = dbContext;
            _emailSender = emailSender;
        }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }
        public IEnumerable<Country> Countries { get; set; }

        public async Task<IActionResult> OnPostAsync(string ReturnUrl = null)
        {
            ReturnUrl ??= Url.Content("~/");
            Countries = await _dbContext.Countries.ToListAsync();
            Countries = Countries.OrderBy(pr => pr.CountryName);
            // On server form validation
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
                    ModelState.AddModelError("", "Minimal allowed age is 3 years.");
                    return Page();
                }

                // Validations are OK, let enter the data

                // If user entered 0 at the beginning, we will remove it and add phone prefix
                if (Input.PhoneNumber.FirstOrDefault().Equals("0"))
                {
                    Input.PhoneNumber = (Input.PhoneNumber).Remove(0, 1);
                }
                int numPrefix = await _dbContext.Countries.Where(s => s.Id == Input.CountryId).Select(s => s.PhonePrefix).FirstOrDefaultAsync();
                string prefix = numPrefix.ToString();
                string phoneNumber = prefix + Input.PhoneNumber;
                var user = new User()
                {
                    UserName = (Input.Email).Trim().ToLower(),
                    DisplayedName = (Input.DisplayedName).Trim(),
                    FirstName = (Input.FirstName).Trim(),
                    LastName = (Input.LastName).Trim(),
                    Email = (Input.Email).Trim().ToLower(),
                    RegistrationTime = DateTime.UtcNow,
                    Gender = Input.Gender,
                    CountryId = Input.CountryId,
                    BirthDate = Input.BirthDate,
                    PhoneNumber = Input.PhoneNumber

                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (!result.Succeeded)
                {
                    _logger.LogError("Couldn't create User Account for " + user.Email, result);
                    return RedirectToPage("/");
                }

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

                // User is Added new set for him default profile security

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

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    $"<h3 align = 'center'>Hello from Stories</h3><p>Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.</p>");

                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                }
                else
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToPage("RegisterConfirmation");
                }
            }
            else return Page();
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            Countries = await _dbContext.Countries.ToListAsync();
            Countries = Countries.OrderBy(pr => pr.CountryName);
            ReturnUrl = returnUrl;
        }

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
    }
}