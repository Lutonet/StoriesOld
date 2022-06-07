using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NuGet.Configuration;
using Stories.Data;
using Stories.Model;
using Settings = Stories.Tools.Settings;

namespace Stories.Pages.Profile
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private ILogger<IndexModel> _logger;
        public bool isAuthenticated { get; set; } = false;
        public bool isAdmin { get; set; } = false;
        public bool isActualUser { get; set; } = false;
        public bool userRecognized { get; set; } = false;
        public bool isFriendWithActual { get; set; } = false;
        public User ActualUser { get; set; } = null;
        public User RequestedProfile { get; set; } = null;
        public FullUserProfile UserProfile { get; set; } = new();
        public string UserArticlesJson { get; set; } = "";
        public List<FriendsToList> FriendsList { get; set; } = new List<FriendsToList>();
        public bool DisplayAddToFriendsButton { get; set; } = false;
        public bool DisplayWaitingApproval { get; set; } = false;
        public bool AreFriends { get; set; } = false;
   

        public IndexModel(ApplicationDbContext dbContext, ILogger<IndexModel> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }


        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id != null)
            {
                // find out if actual user is A) admin, B) friend, C) user himself D) annonymous visitor
                if (User.Identity.Name != null)
                {
                    ActualUser = await _dbContext.Users.Where(s => s.Email == User.Identity.Name).Include(s => s.Friends).FirstOrDefaultAsync();
                    if (ActualUser != null)
                    {
                        isAuthenticated = true;
                        if (User.IsInRole(Tools.Settings.Administrator) && User.IsInRole(Tools.Settings.Redactor))
                        {
                            isAdmin = true;
                        }

                        if (ActualUser.Id == id)
                        {
                            isActualUser = true;
                            if (await _dbContext.Friends.Where(s => s.UserId == ActualUser.Id).Where(s => s.FriendId == id)
                                .Where(s => s.Confirmed)
                                .AnyAsync())
                            {
                                isFriendWithActual = true;
                            }
                        }

                        if (await _dbContext.Friends.Where(s => s.UserId == ActualUser.Id)
                                                    .Where(s => s.FriendId == id)
                                                    .Where(s => s.Confirmed)
                                                    .Where(s => s.Blocked == false).AnyAsync())
                        {
                            AreFriends = true;
                        }
                        if (await _dbContext.Friends.Where(s => s.UserId == ActualUser.Id)
                                                    .Where(s => s.FriendId == id)
                                                    .Where(s => s.Confirmed == false)
                                                    .Where(s => s.Blocked == false).AnyAsync())
                        {
                            DisplayWaitingApproval = true;
                        }
                        if (!await _dbContext.Friends.Where(s => s.UserId == ActualUser.Id)
                                                      .Where(s => s.FriendId == id).AnyAsync() && isActualUser == false)
                        {
                            DisplayAddToFriendsButton = true;
                        }

                    }
                }

                // try to get profile of the user
                RequestedProfile = await _dbContext.Users.Where(s => s.Id == id).Include(s => s.Friends).Include(s => s.Country).FirstOrDefaultAsync();
                if (RequestedProfile != null)
                {
                    userRecognized = true;
                    // get privacy settings 
                    UserSettings setting = await _dbContext.UserSettings.Where(s => s.UserId == id).FirstOrDefaultAsync();
                    
                    if (setting == null)
                    {
                        setting.UserId = id;
                        setting.EmailPrivacy = Settings.AccessRights.Private;
                        setting.NamePrivacy = Settings.AccessRights.Private;
                        setting.FrindsPrivacy = Settings.AccessRights.Public;
                        setting.PhonePrivacy = Settings.AccessRights.Private;
                        setting.SocialMediaLinksPrivacy = Settings.AccessRights.Public;
                        setting.CountryPrivacy = Settings.AccessRights.Public;
                        setting.BirthDatePrivacy = Settings.AccessRights.Public;
                        setting.PictureFilePrivacy = Settings.AccessRights.Public;
                        setting.LastSeenPrivacy = Settings.AccessRights.Public;

                        await _dbContext.UserSettings.AddAsync(setting);
                        await _dbContext.SaveChangesAsync();
                    }
                    /* Fill UserProfile from ReqestedProfile and generated data based on privacy settings */
                    UserProfile = new();
                    UserProfile.JsonFriendsList = "";
                    UserProfile.UserId = RequestedProfile.Id;
                    UserProfile.DisplayedName = RequestedProfile.DisplayedName;
                    // Names 
                    if (isAdmin
                        || isActualUser
                        || (isFriendWithActual && setting.NamePrivacy == Settings.AccessRights.Friends)
                        || (isAuthenticated && setting.NamePrivacy == Settings.AccessRights.Registered)
                        || setting.NamePrivacy == Settings.AccessRights.Public)
                    {
                        UserProfile.FirstName = RequestedProfile.FirstName;
                        UserProfile.LastName = RequestedProfile.LastName;
                        UserProfile.ShowName = true;
                    }
                    
                    UserProfile.RegistrationTime = RequestedProfile.RegistrationTime;
                    // BirthDate
                    if (isAdmin
                        || isActualUser
                        || (isFriendWithActual && setting.BirthDatePrivacy == Settings.AccessRights.Friends)
                        || (isAuthenticated && setting.BirthDatePrivacy == Settings.AccessRights.Registered)
                        || setting.BirthDatePrivacy == Settings.AccessRights.Public)
                    {
                        UserProfile.BirthDate = RequestedProfile.BirthDate;
                        UserProfile.Age = DateTime.UtcNow.Year - RequestedProfile.BirthDate.Year;
                        UserProfile.ShowBirthDate = true;
                    }
                    // LastSeen
                    if (isAdmin
                        || isActualUser
                        || (isFriendWithActual && setting.LastSeenPrivacy == Settings.AccessRights.Friends)
                        || (isAuthenticated && setting.LastSeenPrivacy == Settings.AccessRights.Registered)
                        || setting.LastSeenPrivacy == Settings.AccessRights.Public)
                    {
                        UserProfile.LastSeen = await _dbContext.Sessions.Where(s => s.UserId == RequestedProfile.Id)
                            .OrderByDescending(s => s.Connected).Select(s => s.Connected).FirstOrDefaultAsync();
                        UserProfile.ShowLastSeen = true;
                    }

                    if (isAdmin
                        || isActualUser
                        || (isFriendWithActual && setting.CountryPrivacy == Settings.AccessRights.Friends)
                        || (isAuthenticated && setting.CountryPrivacy == Settings.AccessRights.Registered)
                        || setting.CountryPrivacy == Settings.AccessRights.Public)
                    {
                        UserProfile.CountryName = RequestedProfile.Country.CountryName;
                        UserProfile.ShowCountry = true;
                    }

                    if (isAdmin
                        || isActualUser
                        || (isFriendWithActual && setting.PhonePrivacy == Settings.AccessRights.Friends)
                        || (isAuthenticated && setting.PhonePrivacy == Settings.AccessRights.Registered)
                        || setting.PhonePrivacy == Settings.AccessRights.Public)
                    {
                        UserProfile.PhoneNumber = RequestedProfile.PhoneNumber;
                        UserProfile.ShowPhone = true;
                    }

                    UserProfile.Gender = RequestedProfile.Gender;
                    UserProfile.Info = RequestedProfile.Info;

                    if (isAdmin
                        || isActualUser
                        || (isFriendWithActual && setting.SocialMediaLinksPrivacy == Settings.AccessRights.Friends)
                        || (isAuthenticated && setting.SocialMediaLinksPrivacy == Settings.AccessRights.Registered)
                        || setting.SocialMediaLinksPrivacy == Settings.AccessRights.Public)
                    {
                        UserProfile.Facebook = RequestedProfile.Facebook;
                        UserProfile.Twitter = RequestedProfile.Twitter;
                        UserProfile.Google = RequestedProfile.Google;
                        UserProfile.Microsoft = RequestedProfile.Microsoft;

                        UserProfile.ShowSocialMedia = true;
                    }

                    UserProfile.Articles = await _dbContext.Articles.Where(s => s.UserId == id)
                        .Where(s => s.IsPublished).CountAsync();

                    UserProfile.Critics = await _dbContext.Critics.Where(s => s.User == RequestedProfile)
                        .Where(s => s.Deleted == false).CountAsync();

                    if (isAdmin
                        || isActualUser
                        || (isFriendWithActual && setting.PictureFilePrivacy == Settings.AccessRights.Friends)
                        || (isAuthenticated && setting.PictureFilePrivacy == Settings.AccessRights.Registered)
                        || setting.PictureFilePrivacy == Settings.AccessRights.Public)
                    {
                        UserProfile.PictureUrl = "/Images/" + id + "/ProfilePhoto_180.png";
                        UserProfile.ShowPicture = true;
                    }

                    if (isAdmin
                        || isActualUser
                        || (isFriendWithActual && setting.EmailPrivacy == Settings.AccessRights.Friends)
                        || (isAuthenticated && setting.EmailPrivacy == Settings.AccessRights.Registered)
                        || setting.EmailPrivacy == Settings.AccessRights.Public)
                    {
                        UserProfile.Email = RequestedProfile.Email;
                        UserProfile.ShowEmail = true;
                    }

                    if ((isAuthenticated) && (isAdmin
                        || isActualUser
                        || (isFriendWithActual && setting.FrindsPrivacy == Settings.AccessRights.Friends)
                        || (isAuthenticated && setting.FrindsPrivacy == Settings.AccessRights.Registered)
                        || setting.FrindsPrivacy == Settings.AccessRights.Public))
                    {
                        if (await _dbContext.Friends.Where(s => s.UserId == id).Where(s => s.Confirmed)
                            .AnyAsync())
                        {
                            List<string> FriendsId = await _dbContext.Friends.Where(s => s.UserId == id)
                                .Where(s => s.Confirmed).Select(s => s.FriendId).ToListAsync();

                            foreach (string friend in FriendsId)
                            {
                                User FriendCard = await _dbContext.Users.Where(s => s.Id == friend)
                                    .FirstOrDefaultAsync();
                                FriendsToList FriendTo = new FriendsToList();
                                FriendTo.AuthorId = FriendCard.Id;
                                FriendTo.AuthorName = FriendCard.DisplayedName;
                                FriendTo.Registered = FriendCard.RegistrationTime;
                                FriendTo.ArticlesCount =
                                    await _dbContext.Articles.Where(s => s.UserId == friend).CountAsync();
                                FriendTo.CriticsCount = await _dbContext.Critics.Where(s => s.CriticId == friend)
                                    .Where(s => s.Deleted == false).CountAsync();
                                FriendTo.LikesCount = await _dbContext.Likes.Where(s => s.UserId == friend).CountAsync();
                                FriendTo.OnlineTime = await _dbContext.OnlineTimes.Where(s => s.UserId == friend)
                                    .Select(s => s.TimeOnline).FirstOrDefaultAsync();
                                FriendTo.OnlineTime = FriendTo.OnlineTime.Add(TimeSpan.FromDays(await _dbContext
                                    .OnlineTimes
                                    .Where(s => s.UserId == friend).Select(s => s.Days).FirstOrDefaultAsync()));
                                bool isFriendOfFriend = await _dbContext.Friends.Where(s => s.UserId == ActualUser.Id)
                                    .Where(s => s.FriendId == RequestedProfile.Id).Where(s => s.Confirmed).AnyAsync();

                                if (isAdmin
                                    || isFriendOfFriend && setting.FrindsPrivacy == Settings.AccessRights.Friends
                                    || (isAuthenticated && setting.FrindsPrivacy == Settings.AccessRights.Registered)
                                    || setting.FrindsPrivacy == Settings.AccessRights.Public)
                                {
                                    FriendTo.LastSeen = await _dbContext.Sessions
                                        .Where(s => s.UserId == RequestedProfile.Id).OrderByDescending(s => s.Connected)
                                        .Select(s => s.Connected).FirstOrDefaultAsync();
                                    FriendTo.ShowLastSeen = true;

                                    
                                }
                                FriendsList.Add(FriendTo);

                            }
                                UserProfile.JsonFriendsList = JsonConvert.SerializeObject(FriendsList);
                                
                        }
                    }
                    //
                    if (await _dbContext.Articles.Where(s => s.UserId == id).AnyAsync())
                    {
                        List<UserArticlesTable> ArticlesTemp = new List<UserArticlesTable>();

                        List<Article> selectedArticles = await _dbContext.Articles.Where(s => s.UserId == id).Where(s => s.IsPublished).ToListAsync();
                        UserArticlesTable article;
                        foreach (Article art in selectedArticles)
                        {
                            article = new();
                            article.ArticleId = art.Id;
                            article.LikesCount = await _dbContext.Likes.Where(s => s.ArticleId == art.Id).CountAsync();
                            if (await _dbContext.Stars.Where(s => s.ArticleId == art.Id).AnyAsync())
                            {
                                article.StarsGiversCount = await _dbContext.Stars.Where(s => s.ArticleId == art.Id).CountAsync();
                            }
                            else
                            {
                                article.StarsGiversCount = 0;
                                article.Stars = 0;
                            }

                            article.CriticsCount = await _dbContext.Critics.Where(s => s.ArticleId == art.Id).CountAsync();
                            article.ArticleTitle = art.Title;
                            article.CategoryId = await _dbContext.Article_Categories.Where(s => s.ArticleId == art.Id).Select(s => s.Id).FirstOrDefaultAsync();
                            article.CategoryName = await _dbContext.Categories.Where(s => s.Id == article.CategoryId).Select(s => s.CategoryName).FirstOrDefaultAsync();
                            article.CategoryGroupId = await _dbContext.Categories.Where(s => s.Id == article.CategoryId).Select(s =>s.CategoryGroupId).FirstOrDefaultAsync();
                            article.CategoryGroupName = await _dbContext.CategoryGroups.Where(s => s.Id == article.CategoryGroupId).Select(s => s.GroupName).FirstOrDefaultAsync();
                            article.Published = art.ArticlePublished;
                            ArticlesTemp.Add(article);
                        }
                        UserArticlesJson = JsonConvert.SerializeObject(ArticlesTemp);
                    }

                }

            }
            return Page();
        }

       
            public class FriendsToList
            {
                public string AuthorId { get; set; }
                public string AuthorName { get; set; }
                public DateTime Registered { get; set; }
                public int ArticlesCount { get; set; }
                public int CriticsCount { get; set; }
                public int LikesCount { get; set; }
                public TimeSpan OnlineTime { get; set; }
                public DateTime LastSeen { get; set; }
                public bool ShowLastSeen { get; set; } = false;
            }

            public class UserArticlesTable
            {
                public int ArticleId { get; set; }
                public int LikesCount { get; set; }
                public int Stars { get; set; }
                public int StarsGiversCount { get; set; }
                public int CriticsCount { get; set; }
                public string ArticleTitle { get; set; }
                public int CategoryGroupId { get; set; }
                public string CategoryGroupName { get; set; }
                public int CategoryId { get; set; }
                public string CategoryName { get; set; }
                public DateTime Published { get; set; }
            }
        }
    }

