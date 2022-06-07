using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Stories.Data;
using Stories.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Stories.Areas.Identity.Pages.Account.Manage
{
    public class InfoModel : PageModel
    {
        [TempData]
        public string StatusMessage { get; set; }

        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<InfoModel> _logger;
        public string UserName { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public InfoModel(
            UserManager<User> userManager,
            ILogger<InfoModel> logger,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _logger = logger;
            _dbContext = dbContext;
        }

        public class InputModel
        {
            [Display(Name = "Tell us something about you: ")]
            public string InfoData { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Input = new InputModel();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (user.Info != null) Input.InfoData = user.Info;
            else Input.InfoData = "";
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (!ModelState.IsValid) return Page();

            user.Info = Input.InfoData;
            try
            {
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Can't save profile change for user'{_userManager.GetUserId(User)}'.", ex);
                StatusMessage = "Update Can't be completed";
                return RedirectToPage();
            }
            StatusMessage = "Update Successful";
            return RedirectToPage();
        }
    }
}