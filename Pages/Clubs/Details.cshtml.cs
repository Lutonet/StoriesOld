using Microsoft.AspNetCore.Hosting;
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Stories.Pages.Clubs
{
    public class DetailsModel : PageModel
    {
        private ApplicationDbContext _dbContext;
        private ILogger<IndexModel> _logger;
        public SignInManager<User> _signInManager;
        public UserManager<User> _userManager;
        private IWebHostEnvironment _environment;
        public string UserIp { get; set; }
        public string UserClientAgent { get; set; }
        public string ActualUserId { get; set; }
        public string SelectedClubName { get; set; }
        public string jsonArticles { get; set; } = "";
        public bool userAuthenticated { get; set; }
        public bool userIsOwner { get; set; } = false;
        public bool userIsMember { get; set; } = false;
        public User ActualUser { get; set; }
        public Club selectedClub { get; set; }
        public List<AuthorInClub> AuthorsToPublish { get; set; }
        public List<ArticleInClub> ArticlesInClub { get; set; }
        public List<Outsider> Outsider { get; set; }
        public List<Outsider> AuthorsExceptActual { get; set; }
        public List<Article> UserArticle { get; set; }
        public bool showArticleList { get; set; } = false;

        public DetailsModel(ApplicationDbContext dbContext, ILogger<IndexModel> logger, SignInManager<User> signInManager, UserManager<User> userManager, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _environment = environment;
        }

        public async Task<IActionResult> OnGetAsync(int id)
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

            int ClubId = id;
            selectedClub = _dbContext.Clubs.Where(s => s.Id == ClubId).Select(s => s).FirstOrDefault();
            SelectedClubName = selectedClub.ClubName;
            List<string> userIds = await _dbContext.Club_Users.Where(s => s.ClubId == ClubId).Select(s => s.UserId).ToListAsync();
            if (selectedClub.OwnerId == ActualUserId) userIsOwner = true;
            if (await _dbContext.Club_Users.Where(s => s.ClubId == ClubId).Where(s => s.UserId == ActualUserId).AnyAsync())
                userIsMember = true;
            /* Generate proper list of users */
            List<AuthorInClub> AuthorsInClub = new List<AuthorInClub>();
            foreach (string user in userIds)
            {
                string filePath = Path.Combine(_environment.ContentRootPath, "wwwroot", "Images", user, "ProfilePhoto_32.png");
                int ArticlesCount = await _dbContext.Articles.Where(s => s.UserId == user).CountAsync();
                int CriticsCount = await _dbContext.Critics.Where(s => s.CriticId == user).CountAsync();
                AuthorInClub authorToList = new();
                authorToList.AuthorId = user;
                authorToList.DisplayedName = await _dbContext.Users.Where(s => s.Id == user).Select(s => s.DisplayedName).FirstOrDefaultAsync();
                authorToList.ArticlesCount = ArticlesCount;
                authorToList.CriticsCount = CriticsCount;
                Console.Write(filePath);
                authorToList.hasPicture = await Task.Run(() => System.IO.File.Exists(filePath));

                AuthorsInClub.Add(authorToList);
            }
            AuthorsToPublish = AuthorsInClub;

            /* Generate list of articles */
            List<int> ArticleIds = new List<int>();
            List<ArticleInClub> temp = new List<ArticleInClub>();
            ArticleIds = await _dbContext.Club_Articles.Where(s => s.ClubId == ClubId).Select(s => s.ArticleId).ToListAsync();
            foreach (var articleId in ArticleIds)
            {
                ArticleInClub toList = new();
                Article tmpArticle = await _dbContext.Articles.Where(s => s.Id == articleId).Where(s => s.IsPublished == true).Include(s => s.User).FirstOrDefaultAsync();

                toList.ArticleId = articleId;
                toList.Title = tmpArticle.Title;
                toList.PublishedDate = tmpArticle.ArticlePublished;
                toList.AuthorId = tmpArticle.User.Id;
                toList.AuthorName = tmpArticle.User.DisplayedName;
                toList.CategoryId = await _dbContext.Article_Categories.Where(s => s.ArticleId == tmpArticle.Id).Select(s => s.CategoryId).FirstOrDefaultAsync();
                toList.CategoryName = await _dbContext.Categories.Where(s => s.Id == toList.CategoryId).Select(s => s.CategoryName).FirstOrDefaultAsync();
                toList.ReadCount = await _dbContext.Article_Readers.Where(s => s.ArticleId == articleId).CountAsync();
                toList.CriticCount = await _dbContext.Critics.Where(s => s.ArticleId == articleId).Where(s => s.Deleted != true).CountAsync();
                toList.LinkRead = "/Articles/Read?id=" + articleId;
                if ((ActualUser.Id == tmpArticle.User.Id) && (ActualUserId == selectedClub.OwnerId)) toList.LinkDelete = "/Clubs/DeleteArticle?id=" + articleId + "&clubId=" + selectedClub.Id;
                else toList.LinkDelete = "";
                Console.WriteLine("Link:" + toList.LinkDelete);
                temp.Add(toList);
            }
            ArticlesInClub = temp;
            if (ArticlesInClub.Any())
                jsonArticles = JsonConvert.SerializeObject(ArticlesInClub);
            /* Generate list of users which are not group members */

            List<Outsider> outsiders = new List<Outsider>();
            List<Outsider> filterOut = new List<Outsider>();
            outsiders = await _dbContext.Users.Select(s => new Outsider { AuthorId = s.Id, AuthorName = s.DisplayedName }).ToListAsync();
            foreach (string idl in userIds)
            {
                filterOut.Add(outsiders.Where(s => s.AuthorId == idl).FirstOrDefault());
            }
            foreach (Outsider outsider in filterOut)
            {
                outsiders.Remove(outsider);
            }

            Outsider = outsiders;

            /* List of users in group except actual user */
            List<Outsider> groupOutsider = new List<Outsider>();
            foreach (string idt in userIds)
            {
                if (idt != ActualUserId)
                    groupOutsider.Add(await _dbContext.Users.Where(s => s.Id == idt).Select(s => new Clubs.Outsider { AuthorId = s.Id, AuthorName = s.DisplayedName }).FirstOrDefaultAsync());
            }
            AuthorsExceptActual = groupOutsider;

            /* List of Article from users not shared in Group yet */

            List<int> idArticle = await _dbContext.Club_Articles.Where(s => s.ClubId == selectedClub.Id).Select(s => s.ArticleId).ToListAsync();
           
            List<Article> userArticleTemp = await _dbContext.Articles.Where(s => s.UserId == ActualUser.Id).Where(s => s.IsPublished == true).ToListAsync();
            foreach (var art in idArticle)
            {
                Article artr = userArticleTemp.Where(s => s.Id == art).FirstOrDefault();
                userArticleTemp.Remove(artr);
            }
            UserArticle = userArticleTemp.OrderBy(s => s.Title).ToList();

            if (await _dbContext.Club_Articles.Where(s => s.ClubId == selectedClub.Id).AnyAsync())
                showArticleList = true;

            return Page();
        }
    }

    public class ClubDetails
    {
        public string ClubName { get; set; }
        public int ClubId { get; set; }
        public User Owner { get; set; }
        public bool isUserOwner { get; set; }
        public bool isMember { get; set; }
        public bool isPublic { get; set; }
        public List<User> UsersInGroup { get; set; }
        public List<Article> ArticlesInClub { get; set; }
    }

    public class AuthorInClub
    {
        public string AuthorId { get; set; }
        public string DisplayedName { get; set; }
        public int ArticlesCount { get; set; }
        public int CriticsCount { get; set; }
        public bool hasPicture { get; set; }
    }

    public class ArticleInClub
    {
        public int ArticleId { get; set; }
        public string Title { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ReadCount { get; set; }
        public int CriticCount { get; set; }
        public string LinkRead { get; set; }
        public string LinkDelete { get; set; }
        public DateTime PublishedDate { get; set; }
    }

    public class Outsider
    {
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
    }
}