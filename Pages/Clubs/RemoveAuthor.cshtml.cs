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
    public class RemoveAuthorModel : PageModel
    {
        private ApplicationDbContext _dbContext;
        private ILogger<RemoveAuthorModel> _logger;
        public SignInManager<User> _signInManager;
        public UserManager<User> _userManager;

        public RemoveAuthorModel(ApplicationDbContext dbContext, ILogger<RemoveAuthorModel> logger, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string id, int? clubId)
        {
            if (clubId == null || User.Identity.Name == null)
                return NotFound();

            bool isActualUser = false;
            bool isOwner = false;

            User ActualUser = await _dbContext.Users.Where(s => s.Email == User.Identity.Name).FirstOrDefaultAsync();
            Club ActualClub = await _dbContext.Clubs.Where(s => s.Id == clubId).FirstOrDefaultAsync();
            if (ActualUser.Id == id)
            {
                isActualUser = true;
            }

            if (ActualUser.Id == ActualClub.OwnerId)
            {
                isOwner = true;
            }

            // can remove user if he is owner or user himself

            if (isActualUser || isOwner)
            {
                Club_Users toDelete = await _dbContext.Club_Users.Where(s => s.UserId == id).Where(s => s.ClubId == clubId).FirstOrDefaultAsync();
                _dbContext.Club_Users.Remove(toDelete);
                await _dbContext.SaveChangesAsync();

                return Redirect("/Clubs/Details?id=" + clubId);
            }
            else
                return NotFound();
        }
    }
}