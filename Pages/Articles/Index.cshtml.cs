using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stories.Data;
using Stories.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stories.Pages.Articles
{
    public class IndexModel : PageModel
    {
        private ApplicationDbContext _dbContext;
        private ILogger<IndexModel> _logger;
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;

        public User ActualUser { get; set; }
        public string UserIp { get; set; }
        public string UserClientAgent { get; set; }
        public string ActualUserId { get; set; }
        public bool userAuthenticated { get; set; }
        public string jsonNew { get; set; }
        public int ArticlesCount { get; set; }
        public int CategoriesCount { get; set; }
        public int CategoryGroupsCount { get; set; }
        public List<Collection> Collections { get; set; }

        public IndexModel(ApplicationDbContext dbContext, ILogger<IndexModel> logger, UserManager<User> userManager, SignInManager<User> signInManager)
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
            else ActualUserId = "";
            // get counts
            ArticlesCount = await _dbContext.Articles.Where(s => s.IsPublished).CountAsync();
            CategoriesCount = await _dbContext.Categories.CountAsync();
            CategoryGroupsCount = await _dbContext.CategoryGroups.CountAsync();
            Collections = await _dbContext.Collections.ToListAsync();

            // Get all data from Article Table
            List<Article_Category> localArticleCategory = await _dbContext.Article_Categories.ToListAsync();
            List<Article> localList = await _dbContext.Articles.Where(s => s.IsPublished).Include(s => s.User).ToListAsync();
            List<ArticlesTable> localTable = new List<ArticlesTable>();
            List<Article_Category> articleInCategories;
            foreach (Article article in localList)
            {
                articleInCategories = localArticleCategory.Where(s => s.ArticleId == article.Id).ToList();
                foreach (Article_Category articleCategory in articleInCategories)
                {
                    ArticlesTable newRecord = new();
                    newRecord.Title = article.Title;
                    newRecord.AuthorId = article.UserId;
                    newRecord.ArticleId = article.Id;
                    newRecord.AuthorName = article.User.DisplayedName;
                    newRecord.Published = article.ArticlePublished;
                    newRecord.CategoryId = articleCategory.CategoryId;
                    newRecord.CategoryName = await _dbContext.Categories.Where(s => s.Id == articleCategory.CategoryId).Select(s => s.CategoryName).FirstOrDefaultAsync();
                    newRecord.CategoryGroupId = await _dbContext.Categories.Where(s => s.Id == articleCategory.CategoryId).Select(s => s.CategoryGroupId).FirstOrDefaultAsync();
                    newRecord.CategoryGroupName = await _dbContext.CategoryGroups.Where(s => s.Id == newRecord.CategoryGroupId).Select(s => s.GroupName).FirstOrDefaultAsync();
                    newRecord.ReadCount = await _dbContext.Article_Readers.Where(s => s.ArticleId == article.Id).CountAsync();
                    newRecord.CriticsCount = await _dbContext.Critics.Where(s => s.ArticleId == article.Id).Where(s => !s.Deleted).CountAsync();
                    newRecord.LikesCount = await _dbContext.Likes.Where(s => s.ArticleId == article.Id).CountAsync();
                    if (await _dbContext.Stars.Where(s => s.ArticleId == article.Id).AnyAsync())
                        newRecord.AverageStars = (int)Math.Ceiling(await _dbContext.Stars.Where(s => s.ArticleId == article.Id).Select(s => s.StarsCount).AverageAsync());
                    else newRecord.AverageStars = 0;
                    newRecord.ReadByUser = await _dbContext.Article_Readers.Where(s => s.ArticleId == article.Id).Where(s => s.UserId == ActualUserId).AnyAsync();

                    localTable.Add(newRecord);
                }
            }
            jsonNew = JsonConvert.SerializeObject(localTable);
            return Page();
        }

        public class ArticlesTable
        {
            public string Title { get; set; }
            public string AuthorId { get; set; }
            public int ArticleId { get; set; }
            public string AuthorName { get; set; }
            public DateTime Published { get; set; }
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public int CategoryGroupId { get; set; }
            public string CategoryGroupName { get; set; }
            public int ReadCount { get; set; }
            public int CriticsCount { get; set; }
            public int LikesCount { get; set; }
            public int AverageStars { get; set; }
            public bool ReadByUser { get; set; }
        }
    }
}