using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stories.Data;
using Stories.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Stories.Pages.Clubs
{
    public class IndexModel : PageModel
    {
        private ApplicationDbContext _dbContext;
        private ILogger<IndexModel> _logger;
        public SignInManager<User> _signInManager;
        public UserManager<User> _userManager;
        public string UserIp { get; set; }
        public string UserClientAgent { get; set; }
        public string ActualUserId { get; set; }
        public string SelectedClubName { get; set; }
        public bool userAuthenticated { get; set; }
        public User ActualUser { get; set; }
        public List<Club> Clubs { get; set; }
        public List<Club_Article> ClubArticles { get; set; }
        public List<Club_Users> ClubUsers { get; set; }
        public List<Critic> Critics { get; set; }
        public int ClubsCount { get; set; }
        public int ClubId { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public IndexModel(ApplicationDbContext dbContext, ILogger<IndexModel> logger, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string idclub)
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

            /* Here we display details of some club */

            Clubs = await _dbContext.Clubs.Where(s => s.isActive == true).Where(s => s.isPublic == true).Include(s => s.Owner).ToListAsync();
            ClubArticles = await _dbContext.Club_Articles.Include(s => s.Article).ToListAsync();
            ClubUsers = await _dbContext.Club_Users.Include(s => s.User).ToListAsync();
            Clubs.AddRange(await _dbContext.Clubs.Where(s => s.isActive == false).Where(s => s.OwnerId == ActualUserId).ToListAsync());
            Critics = await _dbContext.Critics.ToListAsync();
            List<int> clubId = await _dbContext.Club_Users.Where(s => s.UserId == ActualUser.Id).Select(s => s.ClubId).ToListAsync();
            foreach (int club in clubId)
            {
                if (await _dbContext.Clubs.Where(s => s.Id == club).Where(s => s.isPublic == false).AnyAsync())
                {
                    Clubs.Add(await _dbContext.Clubs.Where(s => s.Id == club).FirstOrDefaultAsync());
                }
            }
            ClubsCount = Clubs.Count();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(InputModel Input)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            else
            {
                ActualUser = await _dbContext.Users.Where(s => s.Email == User.Identity.Name).FirstOrDefaultAsync();
                Club club = new Club();
                club.ClubName = Input.ClubName;
                club.ClubDescription = Input.ClubDescription;
                club.ClubCreated = System.DateTime.UtcNow;
                club.Owner = ActualUser;
                club.isActive = true;
                club.isPublic = Input.IsPublic;

                await _dbContext.Clubs.AddAsync(club);
                await _dbContext.SaveChangesAsync();

                Club_Users clubuser = new Club_Users();
                clubuser.ClubId = club.Id;
                clubuser.UserId = ActualUser.Id;

                await _dbContext.Club_Users.AddAsync(clubuser);
                await _dbContext.SaveChangesAsync();

                return RedirectToPage();
            }
        }

        public class InputModel
        {
            [Required]
            public string ClubName { get; set; }

            [Required]
            public string ClubDescription { get; set; }

            public bool IsPublic { get; set; } = true;
        }
    }
}