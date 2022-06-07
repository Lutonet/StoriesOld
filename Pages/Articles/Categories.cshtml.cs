using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stories.Data;
using Stories.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stories.Pages.Articles
{
    public class CategoriesModel : PageModel
    {
        private ApplicationDbContext _dbContext;
        private ILogger<CategoriesModel> _logger;
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;

        public User ActualUser { get; set; }
        public string UserIp { get; set; }
        public string UserClientAgent { get; set; }
        public string ActualUserId { get; set; }
        public bool userAuthenticated { get; set; }
        public int ArticlesCount { get; set; }
        public int CategoryGroupsCount { get; set; }
        public int CategoriesCount { get; set; }
        public List<Category> Categories { get; set; }

        public CategoriesModel(ApplicationDbContext dbContext, ILogger<CategoriesModel> logger, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            UserIp = HttpContext.Connection.RemoteIpAddress.ToString();
            UserClientAgent = Request.Headers["User-Agent"].ToString();

            if (User.Identity.Name == null)
            {
                userAuthenticated = false;
            }
            else
                userAuthenticated = true;
            if (userAuthenticated)
            {
                ActualUser = await _dbContext.Users.Where(s => s.Email == User.Identity.Name).FirstOrDefaultAsync();
                ActualUserId = ActualUser.Id;
            }
            ArticlesCount = await _dbContext.Articles.Where(s => s.IsPublished).CountAsync();
            CategoriesCount = await _dbContext.Categories.CountAsync();
            CategoryGroupsCount = await _dbContext.CategoryGroups.CountAsync();

            Categories = await _dbContext.Categories.OrderBy(s => s.CategoryName).ToListAsync();

            return Page();
        }
    }
}