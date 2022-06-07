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
    [Route("api/GetCategoryDetails")]
    [ApiController]
    public class GetCategoryDetails : ControllerBase
    {
        private ApplicationDbContext _dbContext;
        private ILogger<Pages.Articles.CategoriesModel> _logger;
        public User ActualUser { get; set; }
        public bool isRegistered { get; set; } = false;
        public string jsonList { get; set; }
        public string jsonOutput { get; set; }

        public GetCategoryDetails(ApplicationDbContext dbContext, ILogger<Pages.Articles.CategoriesModel> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<string> OnGetAsync(int? id)
        {
            if (id == null)
            {
                outputGroupDetails outputJson = new();
                outputJson.IsError = true;
                outputJson.ErrorMessage = "Missing Collection Id";
                outputJson.JsonData = JsonConvert.SerializeObject(null);
                return JsonConvert.SerializeObject(outputJson);
            }

            if (User.Identity.Name != null)
            {
                ActualUser = await _dbContext.Users.Where(s => s.Email == User.Identity.Name).FirstOrDefaultAsync();
                isRegistered = true;
            }

            if (await _dbContext.Article_Categories.Where(s => s.CategoryId == id).AnyAsync())
            {
                List<articlesInCategoryList> outputList = new List<articlesInCategoryList>();
                foreach (Article_Category article in await _dbContext.Article_Categories.Where(s => s.CategoryId == id).Include(s => s.Article).ToListAsync())
                {
                    articlesInCategoryList temp = new articlesInCategoryList();
                    Article? tmpArticle = await _dbContext.Articles.Where(s => s.Id == article.ArticleId).FirstOrDefaultAsync();

                    if (tmpArticle.IsPublished == true)
                    {
                        temp.AuthorId = tmpArticle.UserId;
                        temp.ArticleId = article.ArticleId;
                        temp.AuthorName = await _dbContext.Users.Where(s => s.Id == article.Article.UserId).Select(s => s.DisplayedName).FirstOrDefaultAsync();
                        temp.Title = tmpArticle.Title;
                        temp.Published = tmpArticle.ArticlePublished;
                        temp.ReadCount = await _dbContext.Article_Readers.Where(s => s.ArticleId == article.ArticleId).CountAsync();
                        temp.CriticsCount = await _dbContext.Critics.Where(s => s.ArticleId == article.ArticleId).Where(s => s.Deleted == false).CountAsync();
                        temp.LikesCount = await _dbContext.Likes.Where(s => s.ArticleId == article.ArticleId).CountAsync();
                        if (await _dbContext.Stars.Where(s => s.ArticleId == article.ArticleId).CountAsync() > 0)
                            temp.AverageStars = (int)_dbContext.Stars.Where(s => s.ArticleId == article.ArticleId).Select(s => s.StarsCount).Average();
                        else
                            temp.AverageStars = 0;
                        if (isRegistered)
                        {
                            temp.ReadByUser = await _dbContext.Article_Readers.Where(s => s.UserId == ActualUser.Id).AnyAsync();
                        }
                        else
                            temp.ReadByUser = false;

                        outputList.Add(temp);
                    }
                }
                jsonList = JsonConvert.SerializeObject(outputList);
            }
            else
                jsonList = JsonConvert.SerializeObject(null);

            outputCategoryDetails outputing = new();
            outputing.JsonData = jsonList;
            outputing.IsError = false;
            outputing.CategoryId = (int)id;
            outputing.Category = await _dbContext.Categories.Where(s => s.Id == id).Select(s => s.CategoryName).FirstOrDefaultAsync();

            return JsonConvert.SerializeObject(outputing);
        }
    }

    public class articlesInCategoryList
    {
        public string AuthorId { get; set; }
        public int ArticleId { get; set; }
        public string AuthorName { get; set; }
        public string Title { get; set; }
        public DateTime Published { get; set; }
        public int ReadCount { get; set; }
        public int CriticsCount { get; set; }
        public int LikesCount { get; set; }
        public int AverageStars { get; set; }
        public bool ReadByUser { get; set; }
    }

    public class outputCategoryDetails
    {
        public string JsonData { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsError { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
    }
}