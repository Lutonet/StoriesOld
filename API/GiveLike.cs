using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stories.Data;
using Stories.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Stories.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiveLike : ControllerBase
    {
        private readonly ILogger<Stories.Pages.Administration.Admin.IndexModel> _logger;
        private ApplicationDbContext _dbContext;

        public GiveLike(ILogger<Pages.Administration.Admin.IndexModel> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<string>> OnGetAsync(int ArticleId)
        {

            if (ArticleId < 1)
            {
                return NotFound();
            }

            User ActualUser = await _dbContext.Users.Where(s => s.Email == User.Identity.Name).FirstOrDefaultAsync();

            if (ActualUser == null)
            {
                return Unauthorized();
            }

            if (!await _dbContext.Articles.Where(s => s.Id == ArticleId).AnyAsync())
            {
                return NotFound();
            }

            Article articleToLike = await _dbContext.Articles.Where(s => s.Id == ArticleId).FirstOrDefaultAsync();

            if (ActualUser.Id == articleToLike.UserId)
            {
                return Unauthorized();
            }

            Like alike = new();
            alike.ArticleId = ArticleId;
            alike.UserId = ActualUser.Id;
            try
            {
                await _dbContext.Likes.AddAsync(alike);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return NoContent();
            }

            return JsonConvert.SerializeObject(alike);

        }
    }
}