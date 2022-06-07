using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stories.Data;
using Stories.Model;
using Stories.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stories.Pages.Clubs
{
    public class DeleteModel : PageModel
    {
        private ApplicationDbContext _dbContext;
        private ILogger<DeleteModel> _logger;
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;

        public DeleteModel(ApplicationDbContext dbContext, ILogger<DeleteModel> logger, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (User.Identity.Name == null)
                return NotFound();
            User ActualUser = await _dbContext.Users.Where(s => s.Email == User.Identity.Name).FirstOrDefaultAsync();
            Club ActualClub = await _dbContext.Clubs.Where(s => s.Id == id).FirstOrDefaultAsync();

            if (User.IsInRole(Settings.Administrator) || User.IsInRole(Settings.Redactor) || User.IsInRole(Settings.Editor) || ActualClub.OwnerId == ActualUser.Id)
            {
                List<Club_Article> articleToRemove = await _dbContext.Club_Articles.Where(s => s.ClubId == id).ToListAsync();
                List<Club_Users> usersToRemove = await _dbContext.Club_Users.Where(s => s.ClubId == id).ToListAsync();

                _dbContext.Club_Articles.RemoveRange(articleToRemove);
                _dbContext.Club_Users.RemoveRange(usersToRemove);
                _dbContext.Clubs.Remove(ActualClub);

                await _dbContext.SaveChangesAsync();

                return Redirect("/Clubs");
            }
            return Redirect("/Clubs?id=" + id);
        }
    }
}