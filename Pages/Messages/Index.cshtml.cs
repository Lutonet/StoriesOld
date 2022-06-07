using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stories.Data;
using Stories.Model;
using System.Linq;
using System.Threading.Tasks;

namespace Stories.Pages.Messages
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
            return Page();
        }
    }
}