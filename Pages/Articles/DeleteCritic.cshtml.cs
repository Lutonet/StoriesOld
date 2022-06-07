using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stories.Data;
using Stories.Model;
using Stories.Tools;
using System.Linq;
using System.Threading.Tasks;

namespace Stories.Pages.Articles
{
    public class DeleteCriticModel : PageModel
    {
        private ApplicationDbContext _dbContext;
        private ILogger<DeleteCriticModel> _logger;
        public SignInManager<User> _signInManager;
        public UserManager<User> _userManager;

        public DeleteCriticModel(ApplicationDbContext dbContext, ILogger<DeleteCriticModel> logger, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("/");
            }

            User actualUser = await _dbContext.Users.Where(s => s.Email == User.Identity.Name).FirstOrDefaultAsync();
            Critic critic = await _dbContext.Critics.Where(s => s.Id == id).FirstOrDefaultAsync();
            if ((critic.User == actualUser) && User.IsInRole(Settings.Editor) && User.IsInRole(Settings.Administrator))
            {
                _dbContext.Critics.Remove(critic);
                await _dbContext.SaveChangesAsync();
                return RedirectToPage("/Articles/Read?id=" + critic.ArticleId);
            }

            return RedirectToPage("/");
        }
    }
}