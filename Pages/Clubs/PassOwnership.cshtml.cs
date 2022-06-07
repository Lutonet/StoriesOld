using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stories.Data;
using Stories.Model;
using System.Linq;
using System.Threading.Tasks;

namespace Stories.Pages.Clubs
{
    public class PassOwnershipModel : PageModel
    {
        private ApplicationDbContext _dbContext;
        private ILogger<PassOwnershipModel> _logger;
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;

        public PassOwnershipModel(ApplicationDbContext dbContext, ILogger<PassOwnershipModel> logger, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnGetAsync(string id, int clubId)
        {
            string ActualUserId = await _dbContext.Users.Where(s => s.Email == User.Identity.Name).Select(s => s.Id).FirstOrDefaultAsync();
            if (await _dbContext.Clubs.Where(s => s.Id == clubId).Select(s => s.OwnerId).FirstOrDefaultAsync() == ActualUserId)
            {
                Club actual = await _dbContext.Clubs.Where(s => s.Id == clubId).FirstOrDefaultAsync();
                actual.OwnerId = id;
                await _dbContext.SaveChangesAsync();
                return Redirect("/Clubs/Details?id=" + clubId);
            }
            return Redirect("/");
        }
    }
}