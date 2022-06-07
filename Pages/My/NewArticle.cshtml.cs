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

namespace Stories.Pages.My
{
    public class NewArticleModel : PageModel
    {
        private ApplicationDbContext _dbContext;
        private ILogger<NewArticleModel> _logger;
        public SignInManager<User> _signInManager;
        public UserManager<User> _userManager;
        public string UserIp { get; set; }
        public string UserClientAgent { get; set; }
        public string ActualUserId { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public List<Category> category { get; set; }
        public List<CategoryGroup> categoryGroups { get; set; }
        public List<AgeRestriction> ageRestriction { get; set; }

        public NewArticleModel(ApplicationDbContext dbContext, ILogger<NewArticleModel> logger, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            UserIp = HttpContext.Connection.RemoteIpAddress.ToString();
            UserClientAgent = Request.Headers["User-Agent"].ToString();
            if (User.Identity.Name != null) ActualUserId = await _dbContext.Users.Where(s => s.Email == User.Identity.Name).Select(s => s.Id).FirstOrDefaultAsync();
            category = await _dbContext.Categories.ToListAsync();
            categoryGroups = await _dbContext.CategoryGroups.OrderByDescending(s => s.Id).ToListAsync();
            ageRestriction = await _dbContext.AgeRestrictions.OrderByDescending(s => s.Id).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            UserIp = HttpContext.Connection.RemoteIpAddress.ToString();
            UserClientAgent = Request.Headers["User-Agent"].ToString();
            if (User.Identity.Name != null) ActualUserId = await _dbContext.Users.Where(s => s.Email == User.Identity.Name).Select(s => s.Id).FirstOrDefaultAsync();
            category = await _dbContext.Categories.ToListAsync();
            categoryGroups = await _dbContext.CategoryGroups.OrderByDescending(s => s.Id).ToListAsync();
            ageRestriction = await _dbContext.AgeRestrictions.OrderByDescending(s => s.Id).ToListAsync();


            if (ModelState.IsValid)
            {
                Article article = new Article();
                article.AgeRestrictionId = Input.AgeRestrictionId;
                article.ArticleAdded = DateTime.UtcNow;
                article.Body = Input.Body;
                article.Prolog = Input.Prolog;
                article.Epilog = Input.Epilog;
                article.LastChange = DateTime.UtcNow;
                article.IsBanned = false;
                article.UserId = ActualUserId;
                article.Title = Input.ArticleName;

                if (Input.Publish)
                {
                    article.FirstPublished = DateTime.UtcNow;
                    article.ArticlePublished = DateTime.UtcNow;
                    article.IsPublished = true;
                }

                await _dbContext.Articles.AddAsync(article);
                await _dbContext.SaveChangesAsync();

                Article_Category artcategory = new Article_Category();
                artcategory.ArticleId = article.Id;
                artcategory.CategoryId = Input.CategoryGroupId;

                await _dbContext.Article_Categories.AddAsync(artcategory);
                await _dbContext.SaveChangesAsync();

                return Redirect("./");
            }
            else
            {
                return Page();
            }
        }

        public class InputModel
        {
            [Required]
            public string ArticleName { get; set; }

            public int CategoryGroupId { get; set; }

            [Required]
            public int CategoryId { get; set; }

            public string Prolog { get; set; }

            [Required]
            public string Body { get; set; }

            public string Epilog { get; set; }

            [Required]
            public int AgeRestrictionId { get; set; }

            [Required]
            public bool Publish { get; set; }
        }
    }
}