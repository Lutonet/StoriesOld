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
    public class JoinModel : PageModel
    {
        private ApplicationDbContext _dbContext;
        private ILogger<JoinModel> _logger;
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;

        public JoinModel(ApplicationDbContext dbContext, ILogger<JoinModel> logger, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null && User.Identity.Name == null)
                return NotFound();

            User ActualUser = await _dbContext.Users.Where(s => s.Email == User.Identity.Name).FirstOrDefaultAsync();
            if (!await _dbContext.Club_Users.Where(s => s.UserId == ActualUser.Id).Where(s => s.ClubId == id).AnyAsync())
            {
                Club_Users newUser = new();
                newUser.UserId = ActualUser.Id;
                newUser.ClubId = (int)id;

                await _dbContext.Club_Users.AddAsync(newUser);
                await _dbContext.SaveChangesAsync();
            }

            return Redirect("/Clubs/Details?id=" + id);
        }
    }
}