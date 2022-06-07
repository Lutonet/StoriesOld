using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stories.Data;
using Stories.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Stories.Pages.Clubs
{
    public class AddArticleModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private ILogger<AddArticleModel> _logger;
        public SignInManager<User> _signInManager;
        public UserManager<User> _userManager;

        public AddArticleModel(ApplicationDbContext dbContext, ILogger<AddArticleModel> logger, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(int? id, int? groupId)
        {
            if (id == null || groupId == null)
                return NotFound();
            User actualUser = await _dbContext.Users.Where(s => s.Email == User.Identity.Name).FirstOrDefaultAsync();
            if (actualUser == null)
                return NotFound();
            if (!await _dbContext.Club_Users.Where(s => s.ClubId == groupId).Where(s => s.UserId == actualUser.Id).AnyAsync())
                return Redirect("/");

         
         
            Club_Article article = new Club_Article();
            article.ArticleId = (int)id;
            article.ClubId = (int)groupId;
            await _dbContext.Club_Articles.AddAsync(article);
            await _dbContext.SaveChangesAsync();

            return Redirect("/Clubs/Details?id=" + groupId);
        }
    }
}