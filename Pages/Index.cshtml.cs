using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stories.Data;
using Stories.Model;
using Stories.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Stories.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _dbContext;
        public SignInManager<User> _signInManager;
        public UserManager<User> _userManager;
        private ICookieService _cookieService;
        public string UserIp { get; set; }
        public string UserClientAgent { get; set; }
        public string ActualUserId { get; set; }
        public int StarsGivers { get; set; } = 0;
        public string jsonNew { get; set; } = "";
        public string jsonPopular { get; set; } = "";
        public bool ArticleExists { get; set; } = false;

        public bool IsAuthenticated { get; set; } = false;
        public bool hasActualLikedArticle { get; set; } = false;
        public bool viewerIsAuthor { get; set; } = false;
        public User ActualUser { get; set; }
        public LastArticle LatestArticle { get; set; } 
        public CategoryGroup CategoryGroup { get; set; }
        public List<Article> Articles { get; set; }
        public List<Article> PopularArticles { get; set; }

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext dbContext, UserManager<User> userManager, SignInManager<User> signInManager, ICookieService cookieService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _cookieService = cookieService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ActionResult> OnGetAsync()
        {

            var filePath = Path.Combine(AppContext.BaseDirectory, "FirstRun.txt");
            try
            {
                if (!System.IO.File.Exists(filePath))
                    await System.IO.File.WriteAllTextAsync(filePath, "1");
                if (await System.IO.File.ReadAllTextAsync(filePath) == "1")
                {
                    _logger.LogInformation("First Run page initialized. Redirecting to Database initialization and creation of Administrator's account");
                    if (HttpContext != null)
                        UserIp = HttpContext.Connection.RemoteIpAddress.ToString();
                    return RedirectToPage("/Administration/FirstRun/Index");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when opening the file: ", ex.Message);
                _logger.LogError("Error when opening the file:", ex);
                return Redirect("/Error");
            }

            UserIp = HttpContext.Connection.RemoteIpAddress.ToString();
            UserClientAgent = Request.Headers["User-Agent"].ToString();
            if (User.Identity != null)
            {
                ActualUserId = await _dbContext.Users.Where(s => s.Email == User.Identity.Name).Select(s => s.Id).FirstOrDefaultAsync();

                ActualUser = await _dbContext.Users.Where(s => s.Id == ActualUserId).FirstOrDefaultAsync();
                IsAuthenticated = true;
            }
            else ActualUser = null;
            /* First will be last 15 articles published     everyone */
           


            List<NewArticleTable> NewArticles = new List<NewArticleTable>();
            if (await _dbContext.Articles.Where(s => s.IsPublished == true).AnyAsync())
            { 
            ArticleExists = true;
            Articles = await _dbContext.Articles.Include(s => s.User).Include(s => s.Stars).Where(s => s.IsPublished == true).OrderByDescending(s => s.ArticlePublished).Take(20).ToListAsync();
            
                foreach (var article in Articles)
                {
                    NewArticleTable articleTable = new();
                    articleTable.AuthorName = article.User.DisplayedName;
                    articleTable.Published = article.ArticlePublished;
                    articleTable.Title = article.Title;
                    articleTable.AuthorId = article.UserId;
                    articleTable.ArticleId = article.Id;
                    if (article.Stars.Count > 0)
                        articleTable.AverageStars = (int)article.Stars.Select(s => s.StarsCount).Average();
                    else
                        articleTable.AverageStars = 0;
                    articleTable.CategoryId = await _dbContext.Article_Categories.Where(s => s.ArticleId == article.Id).Select(s => s.CategoryId).FirstOrDefaultAsync();
                    articleTable.CategoryName = await _dbContext.Categories.Where(s => s.Id == articleTable.CategoryId).Select(s => s.CategoryName).FirstOrDefaultAsync();
                    articleTable.ReadCount = await _dbContext.Article_Readers.Where(s => s.ArticleId == article.Id).CountAsync();
                    articleTable.CriticsCount = await _dbContext.Critics.Where(s => s.ArticleId == article.Id)
                        .Where(s => s.Deleted == false)
                        .CountAsync();
                    articleTable.LikesCount = await _dbContext.Likes.Where(s => s.ArticleId == article.Id).CountAsync();

                    NewArticles.Add(articleTable);
                }
                if (NewArticles != null)
                    jsonNew = JsonConvert.SerializeObject(NewArticles);
                else
                    jsonNew = JsonConvert.SerializeObject(null);
                /* News Frame  (last article) using "read more" (width 90% HR up and down)        everyone */

                Article lastArticle = await _dbContext.Articles.Include(s => s.User).OrderByDescending(s => s.ArticlePublished).FirstOrDefaultAsync();

                if (lastArticle != null)
                {
                    LastArticle LastArticle = new();
                    LastArticle.AuthorName = lastArticle.User.DisplayedName;
                    LastArticle.Published = lastArticle.ArticlePublished;
                    LastArticle.Title = lastArticle.Title;
                    LastArticle.AuthorId = lastArticle.UserId;
                    LastArticle.ArticleId = lastArticle.Id;
                    if (lastArticle.Stars.Count > 0)
                        LastArticle.AverageStars = (int)lastArticle.Stars.Select(s => s.StarsCount).Average();
                    else
                        LastArticle.AverageStars = 0;
                    LastArticle.CategoryId = await _dbContext.Article_Categories.Where(s => s.ArticleId == lastArticle.Id).Select(s => s.CategoryId).FirstOrDefaultAsync();
                    LastArticle.CategoryName = await _dbContext.Categories.Where(s => s.Id == LastArticle.CategoryId).Select(s => s.CategoryName).FirstOrDefaultAsync();
                    LastArticle.ReadCount = await _dbContext.Article_Readers.Where(s => s.ArticleId == lastArticle.Id).CountAsync();
                    LastArticle.CriticsCount = await _dbContext.Critics.Where(s => s.ArticleId == lastArticle.Id).Where(s => s.Deleted == false).CountAsync();
                    LastArticle.LikesCount = await _dbContext.Likes.Where(s => s.ArticleId == lastArticle.Id).CountAsync();
                    if (lastArticle.Prolog == "" || LastArticle.Prolog == "")
                    {
                        LastArticle.displayProlog = false;
                        LastArticle.Prolog = "";
                    }
                    else
                    {
                        LastArticle.displayProlog = true;
                        LastArticle.Prolog = lastArticle.Prolog;
                    }
                    if (lastArticle.Epilog == null || LastArticle.Epilog == "")
                    {
                        LastArticle.displayEpilog = false;
                        LastArticle.Epilog = "";
                    }
                    else
                    {
                        LastArticle.displayEpilog = true;
                        LastArticle.Epilog = lastArticle.Epilog;
                    }
                    LastArticle.ArticleBody = lastArticle.Body;
                    CategoryGroup = await _dbContext.Categories.Where(s => s.Id == LastArticle.CategoryId).Select(s => s.CategoryGroup).FirstOrDefaultAsync();
                    LatestArticle = LastArticle;

                    if (await _dbContext.Likes.Where(s => s.ArticleId == LatestArticle.ArticleId).Where(s => s.UserId == ActualUserId).AnyAsync())
                        hasActualLikedArticle = true;

                    if (LastArticle.AuthorId == ActualUserId)
                        viewerIsAuthor = true;

                    StarsGivers = await _dbContext.Stars.Where(s => s.ArticleId == LastArticle.ArticleId).CountAsync();
                }

                /* Second will be most read articles            everyone */

                List<NewArticleTable> PopularArticles = new List<NewArticleTable>();

                List<Article> PopularArt = await _dbContext.Articles.Include(s => s.User).Include(s => s.Article_readers).Include(s => s.Stars).Where(s => s.IsPublished == true).OrderByDescending(s => s.Article_readers.Count()).Take(20).ToListAsync();
                if (PopularArt != null)
                {
                    foreach (var article in PopularArt)
                    {
                        NewArticleTable articleTable = new();
                        articleTable.AuthorName = article.User.DisplayedName;
                        articleTable.Published = article.ArticlePublished;
                        articleTable.Title = article.Title;
                        articleTable.AuthorId = article.UserId;
                        articleTable.ArticleId = article.Id;
                        if (article.Stars.Count > 0)
                            articleTable.AverageStars = (int)article.Stars.Select(s => s.StarsCount).Average();
                        else
                            articleTable.AverageStars = 0;
                        articleTable.CategoryId = await _dbContext.Article_Categories.Where(s => s.ArticleId == article.Id).Select(s => s.CategoryId).FirstOrDefaultAsync();
                        articleTable.CategoryName = await _dbContext.Categories.Where(s => s.Id == articleTable.CategoryId).Select(s => s.CategoryName).FirstOrDefaultAsync();
                        articleTable.ReadCount = await _dbContext.Article_Readers.Where(s => s.ArticleId == article.Id).CountAsync();
                        articleTable.CriticsCount = await _dbContext.Critics.Where(s => s.ArticleId == article.Id)
                            .Where(s => s.Deleted == false).CountAsync();
                        articleTable.LikesCount = await _dbContext.Likes.Where(s => s.ArticleId == article.Id).CountAsync();

                        PopularArticles.Add(articleTable);
                    }

                    jsonPopular = JsonConvert.SerializeObject(PopularArticles);
                }
                else jsonPopular = JsonConvert.SerializeObject(null);
                /* if (is in clubs) new articles                everyone */
                return Page();
            }

            else
            { 
                ArticleExists = false;
                return Page();
            }
        }
    }

    public class NewArticleTable
    {
        public string Title { get; set; }
        public string AuthorId { get; set; }
        public int ArticleId { get; set; }
        public string AuthorName { get; set; }
        public DateTime Published { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ReadCount { get; set; }
        public int CriticsCount { get; set; }
        public int LikesCount { get; set; }
        public int AverageStars { get; set; }
    }

    public class LastArticle : NewArticleTable
    {
        public bool displayProlog { get; set; } = false;
        public bool displayEpilog { get; set; } = false;
        public string Prolog { get; set; }
        public string ArticleBody { get; set; }
        public string Epilog { get; set; }
    }

    public class ArticleWithReadCounts : Article
    {
        public int ReadAuthors { get; set; }
        public int ReadAnonymous { get; set; }
    }
}