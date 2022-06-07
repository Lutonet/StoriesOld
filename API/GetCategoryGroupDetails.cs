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
    [Route("api/GetCategoryGroupDetails")]
    [ApiController]
    public class GetCategoryGroupDetails : ControllerBase
    {
        private ApplicationDbContext _dbContext;
        private ILogger<Pages.Articles.GroupsModel> _logger;
        public User ActualUser { get; set; }
        public bool isRegistered { get; set; } = false;
        public string jsonList { get; set; }
        public string jsonOutput { get; set; }

        public GetCategoryGroupDetails(ApplicationDbContext dbContext, ILogger<Pages.Articles.GroupsModel> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        [HttpGet]
        public async Task<string> OnGetAsync(int id)
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

            List<int> CategoriesInGroup = await _dbContext.Categories.Where(s => s.CategoryGroupId == id).Select(s => s.Id).ToListAsync();
            List<Article_Category> articleCategories = new List<Article_Category>();
            articlesInGroupList temp = new articlesInGroupList();
            List<articlesInGroupList> outputList = new List<articlesInGroupList>();

            if (await _dbContext.Article_Categories.Where(s => s.Category.CategoryGroupId == id).AnyAsync())
                articleCategories = await _dbContext.Article_Categories.Where(s => s.Category.CategoryGroupId == id).Include(s => s.Article).Include(s => s.Category).ToListAsync();
            if (!articleCategories.Any())
                jsonList = JsonConvert.SerializeObject(null);
            else
            {
                foreach (Article_Category article in articleCategories)
                {
                    if (article.Article.IsPublished)
                    {
                        temp.AuthorId = article.Article.UserId;
                        temp.ArticleId = article.ArticleId;
                        temp.AuthorName = await _dbContext.Users.Where(s => s.Id == article.Article.UserId).Select(s => s.DisplayedName).FirstOrDefaultAsync();
                        temp.Title = article.Article.Title;
                        temp.Published = article.Article.ArticlePublished;
                        temp.CategoryId = article.CategoryId;
                        temp.CategoryName = article.Category.CategoryName;
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

            outputGroupDetails outputClass = new();
            outputClass.JsonData = jsonList;
            outputClass.IsError = false;
            outputClass.CategoryGroupId = id;
            outputClass.CategoryGroupName = await _dbContext.CategoryGroups.Where(s => s.Id == id).Select(s => s.GroupName).FirstOrDefaultAsync();

            return JsonConvert.SerializeObject(outputClass);
        }
    }

    public class articlesInGroupList
    {
        public string AuthorId { get; set; }
        public int ArticleId { get; set; }
        public string AuthorName { get; set; }
        public string Title { get; set; }
        public DateTime Published { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ReadCount { get; set; }
        public int CriticsCount { get; set; }
        public int LikesCount { get; set; }
        public int AverageStars { get; set; }
        public bool ReadByUser { get; set; }
    }

    public class outputGroupDetails
    {
        public string JsonData { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsError { get; set; }
        public int CategoryGroupId { get; set; }
        public string CategoryGroupName { get; set; }
    }
}