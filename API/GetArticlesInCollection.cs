using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stories.Data;
using Stories.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stories.API
{
    [Route("api/GetArticlesInCollection")]
    [ApiController]
    public class GetArticlesInCollection : ControllerBase
    {
        private ApplicationDbContext _dbContext;
        private ILogger<Pages.Clubs.IndexModel> _logger;
        public OutputJson outputJson = new();
        public List<OutputList> outputList = new List<OutputList>();

        public GetArticlesInCollection(ApplicationDbContext dbContext, ILogger<Pages.Clubs.IndexModel> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        [HttpGet]
        public async Task<string> OnGetAsync(int? id)
        {
            if (id == null)
            {
                outputJson.IsError = true;
                outputJson.ErrorMessage = "Missing Collection Id";
                outputJson.JsonData = JsonConvert.SerializeObject(null);
                return JsonConvert.SerializeObject(outputJson);
            }

            List<Article_Collection> articleCollection = await _dbContext.Article_Collections
                .Where(s => s.CollectionId == id)
                .Include(s => s.Article)
                .OrderBy(s => s.OrderInCollection)
                .ToListAsync();
            if (articleCollection == null || articleCollection.Count == 0)
            {
                outputJson.ArticleCollectionCount = 0;
                outputJson.JsonData = JsonConvert.SerializeObject(null);
            }
            else
            {
                foreach (var collection in articleCollection)
                {
                    OutputList item = new OutputList();

                    item.ArticleId = collection.Article.Id;
                    item.Title = collection.Article.Title;
                    item.CategoryName = await _dbContext.Article_Categories
                        .Include(s => s.Category)
                        .Where(s => s.ArticleId == collection.ArticleId)
                        .Select(s => s.Category.CategoryName)
                        .FirstOrDefaultAsync();
                    item.CategoryId = await _dbContext.Article_Categories
                        .Where(s => s.ArticleId == collection.ArticleId)
                        .Select(s => s.CategoryId)
                        .FirstOrDefaultAsync();
                    item.Published = collection.Article.ArticlePublished;
                    item.ReadersCount = await _dbContext.Article_Readers
                        .Where(s => s.ArticleId == collection.ArticleId)
                        .CountAsync();
                    item.Order = collection.OrderInCollection;
                    if (User.Identity.IsAuthenticated)
                    {
                        string UserId = await _dbContext.Users
                            .Where(s => s.Email == User.Identity.Name)
                            .Select(s => s.Id)
                            .FirstOrDefaultAsync();
                        item.ReadByYou = await _dbContext.Article_Readers
                            .Where(s => s.ArticleId == collection.ArticleId)
                            .Where(s => s.UserId == UserId)
                            .AnyAsync();
                    }
                    else
                    {
                        item.ReadByYou = false;
                    }
                    outputList.Add(item);
                }

                outputJson.ArticleCollectionCount = outputList.Count;
                outputJson.JsonData = JsonConvert.SerializeObject(outputList);
            }
            outputJson.CollectionName = await _dbContext.Collections
                .Where(s => s.Id == id)
                .Select(s => s.CollectionName)
                .FirstOrDefaultAsync();
            outputJson.CollectionDescription = await _dbContext.Collections
                .Where(s => s.Id == id)
                .Select(s => s.CollectionDescription)
                .FirstOrDefaultAsync();

            return JsonConvert.SerializeObject(outputJson);
        }
    }

    public class OutputList
    {
        /* Title, Category, Published, ReadersCount, ReadByYou */
        public int Order { get; set; }
        public int ArticleId { get; set; }
        public string Title { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public DateTime Published { get; set; }
        public int ReadersCount { get; set; }
        public bool ReadByYou { get; set; }
    }

    public class OutputJson
    {
        public string JsonData { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsError { get; set; }
        public string CategoryGroupId { get; set; }
        public string CollectionName { get; set; }
        public string CollectionDescription { get; set; }
        public int ArticleCollectionCount { get; set; }
    }
}