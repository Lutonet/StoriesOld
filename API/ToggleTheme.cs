using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stories.Data;
using Stories.Model;
using Stories.Services;
using Stories.Tools;
using System.Linq;
using System.Threading.Tasks;

namespace Stories.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToggleTheme : ControllerBase
    {

        private ApplicationDbContext _dbContext;
        private ICookieService _cookieService;

        public ToggleTheme(ApplicationDbContext dbContext, ICookieService cookieService)
        {
            _dbContext = dbContext;
            _cookieService = cookieService;
        }

        [HttpGet]
        public async Task<string> OnGetAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return await _cookieService.ToggleTheme();
            }
            if (User.Identity.IsAuthenticated)
            {
                User ActualUser = await _dbContext.Users.Where(s => s.Email == User.Identity.Name).FirstOrDefaultAsync();

                return await _cookieService.ToggleTheme(ActualUser.Email);
            }
            return await _cookieService.ToggleTheme();
        }

    }
}