using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stories.Model;
using System.Linq;
using System.Threading.Tasks;

namespace Stories.Areas.Identity.Pages.Account.Manage
{
    public class ProfilePrivacy : PageModel
    {
        [TempData]
        public string StatusMessage { get; set; }

        private readonly Stories.Data.ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ProfilePrivacy> _logger;
        public string DisplayedName { get; set; }

        public ProfilePrivacy(Stories.Data.ApplicationDbContext context,
                              UserManager<User> userManager,
                              ILogger<ProfilePrivacy> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public UserSettings ProfilePrivacyModel { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            DisplayedName = user.DisplayedName;
            ProfilePrivacyModel = await (from record in _context.UserSettings where record.UserId == user.Id select record).FirstOrDefaultAsync();

            if (ProfilePrivacyModel == null)
            {
                return NotFound();
            }

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(ProfilePrivacyModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                StatusMessage = "Settings Updated!";
                return RedirectToPage();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ProfilePrivacyExists(ProfilePrivacyModel.Id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Can't update database settings for user profile privacy", ex);
                    StatusMessage = "Error updating User profile privacy settings.";
                    return RedirectToPage();
                }
            }
        }

        private bool ProfilePrivacyExists(int id)
        {
            return _context.UserSettings.Any(e => e.Id == id);
        }
    }
}