using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stories.Data;
using Stories.Model;
using Stories.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Stories.Tools.Settings;

namespace Stories.Pages.Administration.FirstRun
{
    public class IndexModel : PageModel
    {
        private ApplicationDbContext _dbContext;
        private ILogger<IndexModel> _logger;
        private IEmailSender _emailSender;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        public IEnumerable<Country> Countries { get; set; }
        public static string Report { get; set; }

        public IndexModel(ApplicationDbContext dbContext,
                          ILogger<IndexModel> logger,
                          IEmailSender emailSender,
                          SignInManager<User> signInManager,
                          UserManager<User> userManager,
                          RoleManager<IdentityRole> roleManager,
                          IConfiguration configuration)
        {
            _dbContext = dbContext;
            _logger = logger;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string PhoneNumber { get; set; }

            [DisplayName("Administrator is male")]
            public Gender Gender { get; set; }

            public int CountryId { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Report = "";
            Report += "Entering Administrator setup.#information##";
            try
            {
                var filePath = System.IO.Path.Combine(AppContext.BaseDirectory, "FirstRun.txt");
                if (await System.IO.File.ReadAllTextAsync(filePath) == "1")
                {
                    //Seems we are where we should be - so now initialize database and run Administrator setup
                    Report += "First Run file found in proper state.#success##";
                    _logger.LogInformation("First Run page initialized. Redirecting to Database initialization and creation of Administrator's account");
                    if (await DbInit.Initialize(_dbContext, _logger))
                    {
                        Report += "Database successfully loaded with starting data.#success";
                        Console.WriteLine("Database data entered");
                        Countries = await _dbContext.Countries.ToListAsync();
                        Countries = Countries.OrderBy(pr => pr.CountryName);

                        return Page();
                    }
                    else
                    {
                        _logger.LogError("Error putting default set of the data into the database");
                        Report += "Error creating the database data.#error";
                        return Page();
                    }
                }
                else
                {
                    _logger.LogCritical("Atempt to open FirstRun page when system is already initialized");
                    return RedirectToPage("/Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error working with file", ex);
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                int count = _dbContext.Users.Count();
                if (count >= 1)
                {
                    _logger.LogError("In database users exist");
                    return Redirect("/Error");
                }
                var country = await _dbContext.Countries.Where(s => s.Id == Input.CountryId).FirstOrDefaultAsync();
                var user = new User
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    PhoneNumber = Input.PhoneNumber,
                    FirstName = "System",
                    LastName = "Administrator",
                    DisplayedName = "Lord Of The Server",
                    Gender = Input.Gender,
                    RegistrationTime = DateTime.UtcNow,
                    Country = country
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (!result.Succeeded)
                {
                    _logger.LogError("Couldn't create Administrator account", result);
                    return Page();
                }
                if (!await _roleManager.RoleExistsAsync(Settings.Subscriber))
                {
                    await _roleManager.CreateAsync(new IdentityRole(Settings.Subscriber));
                    _logger.LogInformation("Role for Subscriber was created");
                }
                if (!await _roleManager.RoleExistsAsync(Settings.Editor))
                {
                    await _roleManager.CreateAsync(new IdentityRole(Settings.Editor));
                    _logger.LogInformation("Role for Editor was created");
                }
                if (!await _roleManager.RoleExistsAsync(Settings.Moderator))
                {
                    await _roleManager.CreateAsync(new IdentityRole(Settings.Moderator));
                    _logger.LogInformation("Role for Moderator was created");
                }
                if (!await _roleManager.RoleExistsAsync(Settings.Redactor))
                {
                    await _roleManager.CreateAsync(new IdentityRole(Settings.Redactor));
                    _logger.LogInformation("Role for Redactor was created");
                }
                if (!await _roleManager.RoleExistsAsync(Settings.Administrator))
                {
                    await _roleManager.CreateAsync(new IdentityRole(Settings.Administrator));
                    _logger.LogInformation("Role for Administrator was dreated");
                }
                await _userManager.AddToRoleAsync(user, Settings.Administrator);
                _logger.LogInformation("User" + user.DisplayedName + "Added to role of Administrator");
                await _userManager.AddToRoleAsync(user, Settings.Redactor);
                _logger.LogInformation("User" + user.DisplayedName + "Added to role of Redactor");
                await _userManager.AddToRoleAsync(user, Settings.Moderator);
                _logger.LogInformation("User" + user.DisplayedName + "Added to role of Moderator");
                await _userManager.AddToRoleAsync(user, Settings.Editor);
                _logger.LogInformation("User" + user.DisplayedName + "Added to role of Editor");
                await _userManager.AddToRoleAsync(user, Settings.Subscriber);
                _logger.LogInformation("User" + user.DisplayedName + "Added to role of Subscriber");

                // We have setup Admin and all roles - now we must get his record into database

                var admin = new Administrator();
                admin.User = user;
                admin.ActivatedBy = 0;
                admin.ActivatedAt = DateTime.UtcNow;
                admin.IsActive = true;
                admin.DeactivatedBy = 0;
                try
                {
                    await _dbContext.Administrators.AddAsync(admin);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation("User " + user.DisplayedName + " Added to the database table Administrators");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Creating Administartor table record", ex.Message);
                    _logger.LogError("Error Creating Administrator table record", ex);
                    return Page();
                }

                var setting = new UserSettings();
                setting.User = user;
                try
                {
                    await _dbContext.UserSettings.AddAsync(setting);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation("User " + user.DisplayedName + " Added to the database UserSettings");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Saving UserSettings table record", ex.Message);
                    _logger.LogError("Error Saving UserSettings table record", ex);
                    return Page();
                }


                var filePath = System.IO.Path.Combine(AppContext.BaseDirectory, "FirstRun.txt");
                try
                {
                    await System.IO.File.WriteAllTextAsync(filePath, "2");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Changing the FirstRun file");
                    _logger.LogError("Error changing the FirstRun file", ex);
                    return Page();
                }
                return RedirectToPage("/");
            }
            return Page();
        }
    }
}