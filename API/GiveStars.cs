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
    public class GiveStars : ControllerBase
    {
        private readonly ILogger<Stories.Pages.Administration.Admin.IndexModel> _logger;
        private ApplicationDbContext _dbContext;

        public GiveStars(ILogger<Pages.Administration.Admin.IndexModel> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<string>> OnGetAsync(int articleId, int stars)
        {
            JsonResponse jsonResponse = new();
            StarGive starGive = new();
            if (articleId == null || stars <= 0)
            {
                return NotFound();
            }

            string authorId = await _dbContext.Articles.Where(s => s.Id == starGive.ArticleId).Select(s => s.UserId).FirstOrDefaultAsync();
            if (authorId == null)
            {
                return NotFound();
            }
            Stars star = new();

            star.ArticleId = articleId;
            star.UserId = authorId;
            star.StarsCount = stars;

            if (await _dbContext.Stars.Where(s => s.UserId == authorId).Where(s => s.ArticleId == articleId).AnyAsync())
            {
                return NoContent();
            }
            try
            {
                await _dbContext.Stars.AddAsync(star);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Can't log stars to the database");
                return NoContent();
            }

            return JsonConvert.SerializeObject(new StarGive() { ArticleId = articleId, Stars = stars });
        }

        public class StarGive
        {
            public int ArticleId { get; set; }
            public int Stars { get; set; }
        }
    }
}