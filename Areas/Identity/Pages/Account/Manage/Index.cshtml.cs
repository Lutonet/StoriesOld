using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stories.Data;
using Stories.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Stories.Tools.Settings;

namespace Stories.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _dbContext;
        private ILogger<IndexModel> _logger;

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ApplicationDbContext dbContext,
            ILogger<IndexModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _logger = logger;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public IEnumerable<Country> Countries { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Required]
            [DataType(DataType.Date)]
            [Display(Name = "Date Of Birth")]
            public DateTime BirthDate { get; set; }

            public int? CountryId { get; set; }

            [Required]
            [StringLength(64, ErrorMessage = "The {0} must be at least {2} and max {1} character long.", MinimumLength = 2)]
            [Display(Name = "Public Pseudonym")]
            public string DisplayedName { get; set; }

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

            public string Facebook { get; set; }
            public string Twitter { get; set; }
            public string Google { get; set; }
            public string Microsoft { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            Countries = await _dbContext.Countries.ToListAsync();
            Countries = Countries.OrderBy(pr => pr.CountryName);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (user == null)
            {
                _logger.LogInformation("Profile page opened by unknown user");
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            Input = new InputModel();
            Username = user.UserName;
            Input.PhoneNumber = phoneNumber;
            Input.FirstName = user.FirstName;
            Input.LastName = user.LastName;
            Input.DisplayedName = user.DisplayedName;
            Input.Gender = user.Gender;
            Input.BirthDate = user.BirthDate;
            Input.CountryId = user.CountryId;
            Input.Microsoft = user.Microsoft;
            Input.Google = user.Google;
            Input.Twitter = user.Twitter;
            Input.Facebook = user.Facebook;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogInformation("Profile update page called by method POST without valid user");
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    _logger.LogError("Error changing phone number", setPhoneResult.Errors);
                    return RedirectToPage();
                }
            }
            var oldUser = await _dbContext.Users.FindAsync(Username);
            user.PhoneNumber = Input.PhoneNumber.Trim();
            user.FirstName = Input.FirstName.Trim();
            user.LastName = Input.LastName.Trim();
            user.DisplayedName = Input.DisplayedName.Trim();
            user.Gender = Input.Gender;
            user.BirthDate = Input.BirthDate;
            user.CountryId = Input.CountryId;
            if (Input.Microsoft == null) Input.Microsoft = "";
            user.Microsoft = Input.Microsoft.Trim();
            if (Input.Google == null) Input.Google = "";
            user.Google = Input.Google.Trim();
            if (Input.Twitter == null) Input.Twitter = "";
            user.Twitter = Input.Twitter.Trim();
            if (Input.Facebook == null) Input.Facebook = "";
            user.Facebook = Input.Facebook.Trim();
            if (user.Equals(oldUser))
            {
                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "No changes were made!";
                RedirectToPage();
            }
            else
                try
                {
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogTrace("User " + user.Email + " changed profile settings");
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error Updating User", ex);
                    StatusMessage = "Error Updating User";
                    return RedirectToPage();
                }
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}