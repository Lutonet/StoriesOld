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

namespace Stories.Pages.My.Collections
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
        public IList<Collection> userCollections { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public IndexModel(ApplicationDbContext dbContext, ILogger<IndexModel> logger, SignInManager<User> signInManager, UserManager<User> userManager)
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

            if (User.Identity.Name != null) ActualUser = await _dbContext.Users.Where(s => s.Email == User.Identity.Name).FirstOrDefaultAsync();
            if (ActualUser == null) return RedirectToPage("/");

            // get collections for this user and article in them
            userCollections = await _dbContext.Collections.Where(s => s.User == ActualUser).ToListAsync();

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
}