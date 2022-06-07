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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Stories.Pages.My
{
    public class IndexModel : PageModel
    {
        private ApplicationDbContext _dbContext;
        private ILogger<IndexModel> _logger;
        public SignInManager<User> _signInManager;
        public UserManager<User> _userManager;
        public string UserIp { get; set; }
        public string UserClientAgent { get; set; }
        public User ActualUser { get; set; }
        public string jsonConcepts { get; set; }
        public string jsonArticles { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public IndexModel(ApplicationDbContext dbContext, ILogger<IndexModel> logger, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id != null)
                return Redirect("/Authors?id=" + id);
            UserIp = HttpContext.Connection.RemoteIpAddress.ToString();
            UserClientAgent = Request.Headers["User-Agent"].ToString();

            if (User.Identity.Name != null) ActualUser = await _dbContext.Users.Where(s => s.Email == User.Identity.Name).FirstOrDefaultAsync();
            if (ActualUser == null) return RedirectToPage("/");

            List<Article> articles = await _dbContext.Articles.Where(s => s.UserId == ActualUser.Id).ToListAsync();
            List<ListForAuthor> tempConcepts = new List<ListForAuthor>();
            List<ListForAuthor> tempArticles = new List<ListForAuthor>();
            foreach (Article article in articles)
            {
                int articleCategoryId = await _dbContext.Article_Categories.Where(s => s.ArticleId == article.Id).Select(s => s.CategoryId).FirstOrDefaultAsync();
                Category articleCategory = await _dbContext.Categories.Where(s => s.Id == articleCategoryId).FirstOrDefaultAsync();
                List<Critic> critics = await _dbContext.Critics.Where(s => s.ArticleId == article.Id).Select(s => s).ToListAsync();
                Console.Write(critics.ToString());
                Console.Write(critics.Count());
                string categoryGroup = await _dbContext.CategoryGroups.Where(s => s.Id == articleCategory.CategoryGroupId).Select(s => s.GroupName).FirstOrDefaultAsync();
                int fullStars = 0;
                ListForAuthor articleToList = new ListForAuthor();
                articleToList.ArticleId = article.Id;
                articleToList.ArticleTitle = article.Title;
                articleToList.CategoryGroupName = categoryGroup;
                articleToList.CategoryName = articleCategory.CategoryName;
                articleToList.LikesCount = await _dbContext.Likes.Where(s => s.ArticleId == article.Id).CountAsync();
                articleToList.StarsGiversCount = await _dbContext.Stars.Where(s => s.ArticleId == article.Id).CountAsync();
                if (articleToList.StarsGiversCount > 0)
                {
                    double averageStars = await _dbContext.Stars.Where(s => s.ArticleId == article.Id).Select(s => s.StarsCount).AverageAsync();
                    fullStars = (int)Math.Round(averageStars, 0);
                }
                articleToList.Stars = fullStars;
                articleToList.CriticsCount = critics.Count();
                articleToList.Written = article.ArticleAdded;
                articleToList.linkEdit = "/Articles/Edit?id=" + article.Id;
                articleToList.LinkRead = "/Articles/Read?id=" + article.Id;

                if (article.IsPublished)
                    tempArticles.Add(articleToList);
                else tempConcepts.Add(articleToList);
            }
            if (tempConcepts.Any())
                jsonConcepts = JsonConvert.SerializeObject(tempConcepts);
            else
                jsonConcepts = "";

            if (tempArticles.Any())
                jsonArticles = JsonConvert.SerializeObject(tempArticles);
            else
                jsonArticles = "";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(InputModel Input)
        {
            if (ModelState.IsValid)
            {
                User actualUser = await _dbContext.Users.Where(s => s.Email == User.Identity.Name).FirstOrDefaultAsync();
                Collection newCollection = new();
                newCollection.CollectionName = Input.Name;
                newCollection.CollectionDescription = Input.Description;
                newCollection.User = actualUser;

                await _dbContext.Collections.AddAsync(newCollection);
                await _dbContext.SaveChangesAsync();

                return Redirect("/My/Collections");
            }
            else return Page();
        }
    }

    public class ListForAuthor
    {
        public int ArticleId { get; set; }
        public int LikesCount { get; set; }
        public int Stars { get; set; }
        public int StarsGiversCount { get; set; }
        public int CriticsCount { get; set; }
        public string ArticleTitle { get; set; }
        public string CategoryGroupName { get; set; }
        public string CategoryName { get; set; }
        public string linkEdit { get; set; }
        public string LinkRead { get; set; }
        public DateTime Written { get; set; }
    }

    public class InputModel
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}