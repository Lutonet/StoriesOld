using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;
using Stories.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Stories.API
{
    [Route("api/getarticlespaginated")]
    [ApiController]
    public class GetArticlesPaginated : ControllerBase
    {
        private ApplicationDbContext _dbContext;
        private Logger _logger;

        public int last_page { get; set; } // lastest page
        public int size { get; set; } // elements per page
        public int page { get; set; } // actual page to load

        public GetArticlesPaginated(ApplicationDbContext dbContext, Logger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        [HttpGet]
        public async Task<string> OnGetAsync(int? page, int? size, string? sort, string? order)
        {
            if (page != null && size != null)
            {
                if (size == 0)
                    size = 1;

                if (page == 0)
                    page = 1;

                if (sort == null)
                    sort = "Title";

                if (order == null)
                    order = "asc";

                int articleCount = await _dbContext.Articles.Where(s => s.IsPublished == true).CountAsync();

                last_page = (int)(articleCount / size) + 1;

                // possible sorting fields, Date, Author, Title, Category, Subcategory, ReadCount, Likes, Stars
                return "";
            }
            else return "KO";
        }

       

        public class ArticlesTable
        {
            public string Title { get; set; }
            public string AuthorId { get; set; }
            public int ArticleId { get; set; }
            public string AuthorName { get; set; }
            public DateTime Published { get; set; }
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public int CategoryGroupName { get; set; }
            public int ReadCount { get; set; }
            public int CriticsCount { get; set; }
            public int LikesCount { get; set; }
            public int AverageStars { get; set; }
        }
    }
}