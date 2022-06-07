using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stories.Data;
using Stories.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stories.Hubs
{
    public class CollectionHub : Hub
    {
        private ApplicationDbContext _dbContext;
        private ILogger<CollectionHub> _logger;

        public CollectionHub(ApplicationDbContext dbContext, ILogger<CollectionHub> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task GetArticlesInCollection(int CollectionId)
        {
 
            User ActualUser = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).FirstOrDefaultAsync();
 
            string CollectionName = await _dbContext.Collections.Where(s => s.Id == CollectionId).Select(s => s.CollectionName).FirstOrDefaultAsync();
            string jsonArticles = null;
            List<ArticlesToList> notInList = new List<ArticlesToList>();
            List<Article> userArticles = await _dbContext.Articles.Where(s => s.User == ActualUser).OrderBy(s => s.Title).ToListAsync();
            List<int> idsInCollection = null;
 
            if (userArticles.Count() > 0)
            {
                Console.WriteLine("Article Found");
                if (await _dbContext.Article_Collections.Where(s => s.CollectionId == CollectionId).AnyAsync())
                {
                    idsInCollection = await _dbContext.Article_Collections.Where(s => s.CollectionId == CollectionId).Select(s => s.ArticleId).ToListAsync();
                    foreach (int id in idsInCollection)
                    {
                        Article tempArticle = userArticles.Where(s => s.Id == id).FirstOrDefault();

                        userArticles.Remove(tempArticle);
                    }
                }
                notInList = userArticles.Select(s => new ArticlesToList
                {
                    ArticleId = s.Id,
                    ArticleName = s.Title
                }).ToList();
                jsonArticles = JsonConvert.SerializeObject(notInList);
            }

            List<Article_Collection> articles = await _dbContext.Article_Collections.Include(s => s.Article)
                .Where(s => s.CollectionId == CollectionId).ToListAsync();
            List<ArticleInList> outcome = articles.Select(s => new ArticleInList
            {
                ACId = s.Id,
                ArticleId = s.ArticleId,
                ArticleName = s.Article.Title,
                OrderNumber = s.OrderInCollection
            }).OrderBy(s => s.OrderNumber).ToList();
            if (outcome.Any())
            {
                string jsonArticle = JsonConvert.SerializeObject(outcome);
                await Clients.Caller.SendAsync("articleListReceived", jsonArticle, jsonArticles, CollectionId, CollectionName);
            }
            else await Clients.Caller.SendAsync("articleListReceived", null, jsonArticles, CollectionId, CollectionName);
        }

        public async Task SaveChanges(string json)
        {
 
            tmp tmp = JsonConvert.DeserializeObject<tmp>(json);
 
            int articleId = int.Parse(tmp.ArticleId);
            int collectionId = int.Parse(tmp.CollectionId);
            // check if already not in the collection
            if (!await _dbContext.Article_Collections.Where(s => s.ArticleId == articleId).Where(s => s.CollectionId == collectionId).AnyAsync())
            {
                int order = await _dbContext.Article_Collections.Where(s => s.CollectionId == collectionId).CountAsync() + 1;

                Article_Collection toSave = new();
                toSave.ArticleId = articleId;
                toSave.CollectionId = collectionId;
                toSave.OrderInCollection = order;
                await _dbContext.Article_Collections.AddAsync(toSave);
                await _dbContext.SaveChangesAsync();

                await Clients.Caller.SendAsync("Reload");
            }
        }

        public async Task DeleteCollection(int CollectionId)
        {
            User ActualUser = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).FirstOrDefaultAsync();
            Collection temp = await _dbContext.Collections.Where(s => s.Id == CollectionId).FirstOrDefaultAsync();

            if (temp != null && ActualUser.Id == temp.UserId)
            {
                List<Article_Collection> toDelete = await _dbContext.Article_Collections.Where(s => s.CollectionId == CollectionId).ToListAsync();

                _dbContext.Article_Collections.RemoveRange(toDelete);
                _dbContext.Collections.Remove(temp);

                await _dbContext.SaveChangesAsync();
                await Clients.Caller.SendAsync("Reload");
            }
        }

        public async Task RemoveArticle(int CollectionId, int Order)
        {
            User ActualUser = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).FirstOrDefaultAsync();
            List<Article_Collection> articlesList = await _dbContext.Article_Collections.Where(s => s.CollectionId == CollectionId).ToListAsync();
            Article_Collection temp = articlesList.Where(s => s.OrderInCollection == Order).FirstOrDefault();
            int maxNumber = articlesList.Count();
            if (maxNumber > Order)
            {
                var j = Order + 1;
                while (j <= maxNumber)
                {
                    Article_Collection tmp = articlesList.Where(s => s.OrderInCollection == j).FirstOrDefault();
                    tmp.OrderInCollection--;
                    j++;
                }
            }
            _dbContext.Article_Collections.Remove(temp);
            await _dbContext.SaveChangesAsync();
            await Clients.Caller.SendAsync("Reload");
        }

        public async Task ChangePosition(int originalPosition, int newPosition, int collectionId)
        {
 

            User ActualUser = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).FirstOrDefaultAsync();
            List<Article_Collection> articlesList = await _dbContext.Article_Collections.Where(s => s.CollectionId == collectionId).ToListAsync();

            if (originalPosition < newPosition)
            {
                Article_Collection temp = articlesList.Where(s => s.OrderInCollection == originalPosition).FirstOrDefault();
                int i = originalPosition + 1;
                while (i <= newPosition)
                {
                    Article_Collection tmp = articlesList.Where(s => s.OrderInCollection == i).FirstOrDefault();
                    tmp.OrderInCollection--;
                    i++;
                }
                temp.OrderInCollection = newPosition;
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                Article_Collection temp = articlesList.Where(s => s.OrderInCollection == originalPosition).FirstOrDefault();
                int i = originalPosition - 1;
                while (i >= newPosition)
                {
                    Article_Collection tmp = articlesList.Where(s => s.OrderInCollection == i).FirstOrDefault();
                    tmp.OrderInCollection++;
                    i--;
                }
                temp.OrderInCollection = newPosition;
                await _dbContext.SaveChangesAsync();
            }
            await GetArticlesInCollection(collectionId);
        }
    }

    public class ArticlesToList
    {
        public int ArticleId { get; set; }
        public string ArticleName { get; set; }
    }

    public class ArticleInList
    {
        public int ACId { get; set; }
        public int ArticleId { get; set; }
        public string ArticleName { get; set; }
        public int OrderNumber { get; set; }
    }

    public class tmp
    {
        public string CollectionId { get; set; }
        public string ArticleId { get; set; }
    }
}