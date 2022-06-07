using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stories.Data;
using Stories.Model;

namespace Stories.Pages.Authors
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
        public bool IsAuthenticated { get; set; } = false;
        public string JsonList { get; set; }
        public FullUserProfile FullProfile { get; set; }
        public bool showProfile { get; set; } = false;
        public string showProfileId { get; set; }

        public IndexModel(ApplicationDbContext dbContext, ILogger<IndexModel> logger, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }


        public async Task<IActionResult> OnGetAsync(string? id)
        {
            UserIp = HttpContext.Connection.RemoteIpAddress.ToString();
            UserClientAgent = Request.Headers["User-Agent"].ToString();

            if (User.Identity.Name != null)
            {
                ActualUser = await _dbContext.Users.Where(s => s.Email == User.Identity.Name).FirstOrDefaultAsync();
                IsAuthenticated = true;
            }
            List<User> tempAuthors = await _dbContext.Users.ToListAsync();
            List<TableAuthors> tableAuthors  = new List<TableAuthors>();
            foreach (User author in tempAuthors)
            {
                TableAuthors listAuthor = new();
                listAuthor.AuthorId = author.Id;
                listAuthor.AuthorName = author.DisplayedName;
                listAuthor.Registered = author.RegistrationTime;
                listAuthor.ArticlesCount = await _dbContext.Articles.Where(s => s.UserId == author.Id).Where(s => s.IsPublished).CountAsync();
                listAuthor.CriticsCount = await _dbContext.Critics.Where(s => s.CriticId == author.Id).Where(s => s.Deleted == false).CountAsync();
                listAuthor.LikesCount = await _dbContext.Likes.Where(s => s.UserId == author.Id).CountAsync();

                TimeSpan onlineTime = await _dbContext.OnlineTimes.Where(s => s.UserId == author.Id).Select(s => s.TimeOnline).FirstOrDefaultAsync();
                TimeSpan daysOnline = new TimeSpan(await _dbContext.OnlineTimes.Where(s => s.UserId == author.Id).Select(s => s.Days).FirstOrDefaultAsync(), 0, 0, 0);
                listAuthor.OnlineTime = onlineTime.Add(daysOnline);
                listAuthor.LastSeen = await _dbContext.Sessions.Where(s => s.UserId == author.Id).OrderByDescending(s => s.Connected).Select(s => s.Connected).FirstOrDefaultAsync();

                tableAuthors.Add(listAuthor);
            }
            JsonList = JsonConvert.SerializeObject(tableAuthors);

            if (id == null)
            {
                return Page();
            }
            else
            {
                showProfile = true;
                if (await _dbContext.Users.Where(s => s.Id == id).AnyAsync())
                {
                    User author = await _dbContext.Users.Where(s => s.Id == id).FirstOrDefaultAsync();
                }
                showProfileId = id;
                return Page();
            }



         
        }
        public class TableAuthors
        {
            public string AuthorId { get; set; }
            public string AuthorName { get; set; }
            public DateTime Registered { get; set; }
            public int ArticlesCount { get; set; }
            public int CriticsCount { get; set; }
            public int LikesCount { get; set; }
            public TimeSpan OnlineTime { get; set; }
            public DateTime LastSeen { get; set; }
        }

        public class ResultReport
        {
            private bool isError { get; set; } = false;
            private string errorText { get; set; } = "";
        }
    }
}