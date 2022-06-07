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
    public class DeleteArticleModel : PageModel
    {
        private ApplicationDbContext _dbContext;
        private ILogger<DeleteArticleModel> _logger;
        public SignInManager<User> _signInManager;
        public UserManager<User> _userManager;

        public DeleteArticleModel(ApplicationDbContext dbContext, ILogger<DeleteArticleModel> logger, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(int? id, int? clubId)
        {
            if (id == null || clubId == null || !User.Identity.IsAuthenticated)
                return NotFound();

            bool isAuthor = false;
            bool isOwner = false;

            User ActualUser = await _dbContext.Users.Where(s => s.Email == User.Identity.Name).FirstOrDefaultAsync();

            if (await _dbContext.Clubs.Where(s => s.Id == clubId).Select(s => s.OwnerId).FirstOrDefaultAsync() == ActualUser.Id)
            {
                isAuthor = true;
            }
            if (await _dbContext.Articles.Where(s => s.Id == id).Select(s => s.UserId).FirstOrDefaultAsync() == ActualUser.Id)
            {
                isOwner = true;
            }

            if (isAuthor && isOwner)
            {
                Club_Article toDelete = await _dbContext.Club_Articles.Where(s => s.ArticleId == id).Where(s => s.ClubId == clubId).FirstOrDefaultAsync();
                _dbContext.Club_Articles.Remove(toDelete);
                await _dbContext.SaveChangesAsync();

                return Redirect("/Clubs/Details?id=" + clubId);
            }

            return NotFound();
        }
    }
}