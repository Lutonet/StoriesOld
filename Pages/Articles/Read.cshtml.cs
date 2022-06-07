using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stories.Data;
using Stories.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Stories.Pages.Articles
{
    public class ReadModel : PageModel
    {
        private ApplicationDbContext _dbContext;
        private ILogger<ReadModel> _logger;
        public SignInManager<User> _signInManager;
        public UserManager<User> _userManager;

        [BindProperty]
        public InputModel Input { get; set; }

        public bool displayProlog { get; set; }
        public bool displayEpilog { get; set; }
        public bool isPublished { get; set; }
        public bool viewerIsAuthor { get; set; }
        public bool hasActualLikedArticle { get; set; }
        public bool hasActualGiveStars { get; set; }
        public int LikesCount { get; set; }
        public int stars { get; set; }
        public int StarsGivers { get; set; }
        public int CriticsCount { get; set; }
        public int ReadCount { get; set; }
        public int ArticlesCount { get; set; }
        public int CategoriesCount { get; set; }
        public int CategoryGroupsCount { get; set; }
        public string UserIp { get; set; }
        public string UserClientAgent { get; set; }
        public string ActualUserId { get; set; }
        public User Author { get; set; }
        public User actualUser { get; set; }
        public Article Article { get; set; }
        public Category Category { get; set; }
        public CategoryGroup CategoryGroup { get; set; }
        public List<Critic> Critics { get; set; }

        public ReadModel(ApplicationDbContext dbContext, ILogger<ReadModel> logger, SignInManager<User> signInManager, UserManager<User> userManager)
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
                return NotFound();
            }

            UserIp = HttpContext.Connection.RemoteIpAddress.ToString();
            UserClientAgent = Request.Headers["User-Agent"].ToString();
            Article_Read articleRead = new Article_Read();
            ArticlesCount = await _dbContext.Articles.Where(s => s.IsPublished == true).CountAsync();
            CategoriesCount = await _dbContext.Categories.CountAsync();
            CategoryGroupsCount = await _dbContext.CategoryGroups.CountAsync();


            Article = await _dbContext.Articles.Where(s => s.Id == id).FirstOrDefaultAsync();
            if (Article == null) Console.WriteLine("Article Not found");
            
            if (Article.Prolog == null || Article.Prolog == "") displayProlog = false; else displayProlog = true;
            if (Article.Epilog == null || Article.Epilog == "") displayEpilog = false; else displayEpilog = true;
            LikesCount = await _dbContext.Likes.Where(s => s.ArticleId == Article.Id).CountAsync();
            Critics = await _dbContext.Critics.Include(s => s.User).Where(s => s.ArticleId == Article.Id).Where(s => s.Deleted == false).OrderByDescending(s => s.CriticAdded).ToListAsync();
            CriticsCount = Critics.Count();
            StarsGivers = await _dbContext.Stars.Where(s => s.ArticleId == Article.Id).CountAsync();

            if (await _dbContext.Stars.Where(s => s.ArticleId == Article.Id).AnyAsync())
            {
                double starsAverage = (await _dbContext.Stars.Where(s => s.ArticleId == Article.Id).Select(s => s.StarsCount).ToListAsync()).Average();
                stars = (int)Math.Round(starsAverage);
            }
            else stars = 0;

            if (!User.Identity.IsAuthenticated)
            {
                actualUser = null;
            }
            else
            {
                actualUser = await _dbContext.Users.Where(s => s.Email == User.Identity.Name).FirstOrDefaultAsync();
            
                hasActualGiveStars = false;
                hasActualLikedArticle = false;
            }

            Author = await _dbContext.Users.Where(s => s.Id == Article.UserId).FirstOrDefaultAsync();
            if (actualUser == null) viewerIsAuthor = false;
            else if (Author == actualUser) viewerIsAuthor = true;
            else viewerIsAuthor = false;
            

            if (!viewerIsAuthor)
            {
                if (!Article.IsPublished)
                    return RedirectToPage("/Index");
            }

            int CategoryId = await _dbContext.Article_Categories.Where(s => s.ArticleId == Article.Id).Select(s => s.CategoryId).FirstOrDefaultAsync();
            Category = await _dbContext.Categories.Where(s => s.Id == CategoryId).FirstOrDefaultAsync();
            CategoryGroup = await _dbContext.CategoryGroups.Where(s => s.Id == Category.CategoryGroupId).FirstAsync();
            

            if (User.Identity.IsAuthenticated)
            {
                string usId = await _dbContext.Users.Where(s => s.Email == User.Identity.Name).Select(s => s.Id).FirstOrDefaultAsync();
                articleRead.UserId = usId;
                articleRead.isAuthenticated = true;
            }
            else
            {
                articleRead.isAuthenticated = false;
            }
            articleRead.IPAddress = UserIp;
            articleRead.ReadAt = DateTime.UtcNow;
            articleRead.ArticleId = (int)id;
            if (User.Identity.IsAuthenticated)
            {
                if (!await _dbContext.Article_Readers.Where(s => s.UserId == actualUser.Id).Where(s => s.ArticleId == (int)id).AnyAsync())
                {
                    await _dbContext.Article_Readers.AddAsync(articleRead);
                    await _dbContext.SaveChangesAsync();
                }
            }
            else
            {
                await _dbContext.Article_Readers.AddAsync(articleRead);
                await _dbContext.SaveChangesAsync();
            }
            ReadCount = await _dbContext.Article_Readers.Where(s => s.ArticleId == id).CountAsync();

            return Page();
        }

        public async Task<ActionResult> OnPostAsync(InputModel Input)
        {
            UserIp = HttpContext.Connection.RemoteIpAddress.ToString();
            UserClientAgent = Request.Headers["User-Agent"].ToString();
            actualUser = await _dbContext.Users.Where(s => s.Email == User.Identity.Name).FirstOrDefaultAsync();
            Article = await _dbContext.Articles.Where(s => s.Id == Input.ArticleId).FirstOrDefaultAsync();
            if (Article == null) Console.WriteLine("Article Not found");
            
            if (Article.Prolog == null || Article.Prolog == "") displayProlog = false; else displayProlog = true;
            if (Article.Epilog == null || Article.Epilog == "") displayEpilog = false; else displayEpilog = true;
            LikesCount = await _dbContext.Likes.Where(s => s.ArticleId == Article.Id).CountAsync();
            Critics = await _dbContext.Critics.Include(s => s.User).Where(s => s.ArticleId == Article.Id).Where(s => s.Deleted == false).OrderByDescending(s => s.CriticAdded).ToListAsync();
            CriticsCount = Critics.Count();
            StarsGivers = await _dbContext.Stars.Where(s => s.ArticleId == Article.Id).CountAsync();

            if (!ModelState.IsValid)
                return Page();

            if ((Input.Text == null) || (Input.Text.Length < 5))
            {
                ModelState.AddModelError("", "Too short text");
                return Page();
            }
            Critic critic = new Critic();
            critic.ArticleId = Input.ArticleId;
            critic.CriticAdded = DateTime.UtcNow;
            critic.User = actualUser;
            Input.Text = Input.Text.Replace("\n", "<br>");
            critic.CriticMessage = Input.Text;
            await _dbContext.Critics.AddAsync(critic);
            await _dbContext.SaveChangesAsync();

            return RedirectToPage("/Articles/Read?id=" + critic.ArticleId);
        }

        public class InputModel
        {
            public int ArticleId { get; set; }

            [Required]
            public string Text { get; set; }
        }
    }
}