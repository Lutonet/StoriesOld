using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stories.Data;
using Stories.Model;
using Stories.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stories.Hubs
{
    /* Created 30/6/2021 */

    public class IndexHub : Hub
    {
        private ApplicationDbContext _dbContext;
        private ILogger<ChatHub> _logger;

        public IndexHub(ApplicationDbContext dbContext, ILogger<ChatHub> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public override async Task<Task> OnConnectedAsync()
        {
            User user = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).FirstOrDefaultAsync();
            if (user == null)
            {
                AnonymousOnline anonymous = new AnonymousOnline();
                anonymous.Connected = DateTime.UtcNow;
                anonymous.ConnectionId = Context.ConnectionId;
            }

            return base.OnConnectedAsync();
        }

        public override async Task<Task> OnDisconnectedAsync(Exception ex)
        {
            User user = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).FirstOrDefaultAsync();
            if (user == null)
            {
                // remove from the database and send to everyone message that anonymous user left

                AnonymousOnline actualClient = await _dbContext.AnonymousOnline.Where(s => s.ConnectionId == Context.ConnectionId).FirstOrDefaultAsync();
                if (actualClient == null) { }
                else
                {
                    actualClient.IsActive = false;
                    await _dbContext.SaveChangesAsync();
                    await Clients.Others.SendAsync("AnonymousDisconnected");
                }
            }
            else
            {
                Session actualSession = await _dbContext.Sessions.Where(s => s.ConnectionId == Context.ConnectionId).Where(s => s.IsActive).FirstOrDefaultAsync();
                // remove from the database, send info about leaving user to everyone, count chatting hours and add them to onlineTimes
                if (actualSession == null)
                {
                    string clientId = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).Select(s => s.Id).FirstOrDefaultAsync();
                    actualSession = await _dbContext.Sessions.Where(s => s.UserId == clientId).Where(s => s.IsActive == true).OrderByDescending(s => s.Connected).FirstOrDefaultAsync();
                }
                if (actualSession != null)
                {
                    actualSession.Disconnected = DateTime.UtcNow;

                    TimeSpan timeOnline = actualSession.Disconnected.Subtract(actualSession.Connected);

                    if (!await _dbContext.OnlineTimes.Where(s => s.UserId == user.Id).AnyAsync())
                    {
                        OnlineTime time = new OnlineTime();
                        time.StartTime = actualSession.Connected;
                        time.TimeOnline = timeOnline;
                        time.UserId = user.Id;
                        time.Days = 0;

                        await _dbContext.OnlineTimes.AddAsync(time);
                        await _dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        OnlineTime time = await _dbContext.OnlineTimes.Where(s => s.UserId == user.Id).FirstOrDefaultAsync();
                        time.StartTime = actualSession.Connected;
                        time.TimeOnline = time.TimeOnline.Add(timeOnline);
                        if (time.TimeOnline.TotalHours > 24)
                        {
                            time.TimeOnline = time.TimeOnline.Subtract(TimeSpan.FromDays(1.0));
                           
                            if (time.Days == null) time.Days = 0;
                            time.Days = time.Days + 1;
                            await _dbContext.SaveChangesAsync();
                        }
                       
                        await _dbContext.SaveChangesAsync();
                    }
                    await Clients.Others.SendAsync("UserDisconnected", user.Id);
                    actualSession.IsActive = false;
                    await _dbContext.SaveChangesAsync();
                    List<string> UserFriendsId = await _dbContext.Friends.Where(s => s.UserId == user.Id)
                                                                         .Where(s => s.Confirmed == true)
                                                                         .Where(s => s.Declined == false)
                                                                         .Where(s => s.Blocked == false)
                                                                         .Select(s => s.FriendId)
                                                                         .ToListAsync();
                    List<Session> SessionCache = await _dbContext.Sessions.Where(s => s.IsActive).ToListAsync();
                    foreach (var friend in UserFriendsId)
                    {
                        if (SessionCache.Where(s => s.UserId == friend).Any())
                        {
                            await Clients.User(friend).SendAsync("FriendDisconnected");
                        }
                    }

                }
            }
            return base.OnDisconnectedAsync(ex);
        }

        /* Created 30/6/2021 */

        public async Task RegisterUser(string userIp, string userClient)
        {
            if (Context.User.Identity.Name == null)
            {
                AnonymousOnline anonymous = new AnonymousOnline();

                anonymous.Connected = DateTime.UtcNow;
                anonymous.ConnectionId = Context.ConnectionId;
                anonymous.IpAddress = userIp;
                anonymous.ClientAgent = userClient;
                anonymous.IsActive = true;

                await _dbContext.AnonymousOnline.AddAsync(anonymous);
                await _dbContext.SaveChangesAsync();
               
                await Clients.Others.SendAsync("annonymousConnected");
            }
            else
            {
                User user = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).FirstOrDefaultAsync();
                Session session = new Session();

                session.UserId = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).Select(s => s.Id).FirstOrDefaultAsync();
                session.Connected = DateTime.UtcNow;
                session.ConnectionId = Context.ConnectionId;
                session.IpAddress = userIp;
                session.ClientAgent = userClient;
                session.IsActive = true;

                await _dbContext.Sessions.AddAsync(session);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("User ID: " + session.UserId + " logged into system from IP " + session.IpAddress + " via " + userClient);
                await Clients.Others.SendAsync("userConnected", user.Id, user.DisplayedName);
                List<string> UserFriendsId = await _dbContext.Friends.Where(s => s.UserId == user.Id)
                                                                       .Where(s => s.Confirmed == true)
                                                                       .Where(s => s.Declined == false)
                                                                       .Where(s => s.Blocked == false)
                                                                       .Select(s => s.FriendId)
                                                                       .ToListAsync();
                List<Session> SessionCache = await _dbContext.Sessions.Where(s => s.IsActive).ToListAsync();
                foreach (var friend in UserFriendsId)
                {
                    if (SessionCache.Where(s => s.UserId == friend).Any())
                    {
                        await Clients.User(friend).SendAsync("FriendConnected");
                    }
                }
            }
        }

        /* Created 30/06/2021 */

        public async Task RequestOnlineUsers()
        {
            string JsonFriends="";
            int FriendsCount = 0;
            int RequestsCount = 0;
            List<string> FriendsId;
            List<Session> sessions = new List<Session>();
            List<string> cashedUsers = await _dbContext.Sessions.Where(s => s.IsActive == true).Select(s => s.UserId).ToListAsync();
            if (Context.User.Identity.IsAuthenticated)
            {
                string UserId = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).Select(s => s.Id).FirstOrDefaultAsync();
                FriendsId = await _dbContext.Friends.Where(s => s.UserId == UserId).Where(s => s.Confirmed == true).Where(s => s.Declined == false).Where(s => s.Blocked == false).Select(s => s.FriendId).ToListAsync();
                if (FriendsId == null)
                {
                    JsonFriends = "";
                    FriendsCount = 0;
                }    
                else
                {
                JsonFriends = JsonConvert.SerializeObject(FriendsId);
                
                    foreach (string friend in FriendsId)
                    {
                        if (await _dbContext.Sessions.Where(s => s.UserId == friend).Where(s => s.IsActive).AnyAsync())
                            FriendsCount++;
                    }
                    
                }
                RequestsCount = await _dbContext.Friends.Where(s => s.FriendId == UserId).Where(s => s.Confirmed == false).Where(s => s.Declined == false).Where(s => s.Blocked == false).CountAsync();
            }

            List<Session> output = new List<Session>();

            foreach (var session in sessions)
            {
                bool existsInList = false;
                foreach (var cashedUser in cashedUsers)
                {
                    if (cashedUser == session.UserId) existsInList = true;
                }
                if (!existsInList)
                {
                    cashedUsers.Add(session.UserId);
                }
            }
            List<ListOfUsers> outputList = new List<ListOfUsers>();
            foreach (string record in cashedUsers)
            {
                string userId = record;
                string displayedName = await _dbContext.Users.Where(s => s.Id == userId).Select(s => s.DisplayedName).FirstOrDefaultAsync();
                ListOfUsers user = new ListOfUsers();
                user.UserId = userId;
                user.DisplayedName = displayedName;
                outputList.Add(user);
            }
            string final = JsonConvert.SerializeObject(outputList);
         
            int anonymousCount = await _dbContext.AnonymousOnline.Where(s => s.IsActive == true).CountAsync();
            int registeredCount = outputList.Count();
            

            await Clients.Caller.SendAsync("connectedUsers", final, anonymousCount, registeredCount, FriendsCount, JsonFriends, RequestsCount);
        }

        public async Task LoadFriendsPage()
        {
            /* initiate whole friends page. 
             * Here we must generate three tables 
             * 1. Table of Received Friend Requests
             * 2. Table of Friends
             * 3. Table of Sent Friend Requests
             */

            await GetFriendsList(1);
            await GetFriendsList(3);
            await GetFriendsList(4);
            await GetFriendsList(2);

        }


        public async Task GetFriendsList(int? Filter)
        {
            ReturnResult newResult = new();
            List<Friends> userFriends = new List<Friends>();
            if (!Context.User.Identity.IsAuthenticated) return;

            User ActualUser = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).FirstOrDefaultAsync();

            if (Filter == null || Filter == 0) // Show all Friendship except blocked contacts
            {
                userFriends = await _dbContext.Friends.Where(s => s.UserId == ActualUser.Id).Include(s => s.Friend).Where(s => s.Blocked == false).ToListAsync();
                Filter = 0;
            }

            if (Filter == 1) // Show Waiting for approval 
            {
                userFriends = await _dbContext.Friends.Where(s => s.UserId == ActualUser.Id).Include(s => s.Friend).Where(s => s.Confirmed == false).Where(s => s.Blocked == false).Where(s => s.Declined == false).ToListAsync();
            }

            if (Filter == 2) // Show Declined ones
            {
                userFriends = await _dbContext.Friends.Where(s => s.FriendId == ActualUser.Id).Include(s => s.User).Where(s => s.Declined == true).Where(s => s.Blocked == false).ToListAsync();
            }

            if (Filter == 3) // Show only confirmed friends
            {
                userFriends = await _dbContext.Friends.Where(s => s.UserId == ActualUser.Id).Include(s => s.Friend).Where(s => s.Confirmed == true).Where(s => s.Blocked == false).ToListAsync();
            }
            if (Filter == 4) // Show received non confirmed friends request
            {
                userFriends = await _dbContext.Friends.Where(s => s.FriendId == ActualUser.Id).Include(s => s.User).Where(s => s.Confirmed == false).Where(s => s.Declined == false).Where(s => s.Blocked == false).ToListAsync();

            }

            if (userFriends.Any())
            {
                List<FriendsList> outputList = new List<FriendsList>();
                foreach (var item in userFriends)
                {
                    FriendsList friend = new FriendsList();

                    if (Filter != 4 || Filter != 2)
                    {
                        friend.FriendId = item.FriendId;
                        friend.FriendName = item.Friend.DisplayedName;
                    }
                    if (Filter == 4 || Filter == 2)
                    {
                        friend.FriendId = item.UserId;
                        friend.FriendName = item.User.DisplayedName;
                    }

                    if (item.Confirmed)
                        friend.FriendshipStatus = 1;

                    if (item.Confirmed == false)
                        friend.FriendshipStatus = 2;

                    if (item.Declined == true)
                        friend.FriendshipStatus = 3;

                    Settings.AccessRights pictureAccess = await _dbContext.UserSettings.Where(s => s.UserId == item.FriendId).Select(s => s.PictureFilePrivacy).FirstOrDefaultAsync();

                    if (pictureAccess.Equals(Settings.AccessRights.Friends) || pictureAccess.Equals(Settings.AccessRights.Registered) || pictureAccess.Equals(Settings.AccessRights.Public))
                    {
                        friend.ShowPicture = true;
                    }

                    outputList.Add(friend);
                }
        

            newResult.IsError = false;
            newResult.ErrorMessage = "";
            newResult.ErrorId = 0; // 0 = no error
            newResult.JsonResult = JsonConvert.SerializeObject(outputList);
            }            

            else
            {
                newResult.JsonResult = null;
                newResult.IsError = true;
                newResult.ErrorMessage = "No records Found";
                newResult.ErrorId = 1; // 1 = no records
            }
            string jsonAnswer = JsonConvert.SerializeObject(newResult);
           

            await Clients.Caller.SendAsync("FriendsListReceived", jsonAnswer, Filter);

        }

        public async Task Unfriend(string id)
        {
            User ActualUser = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name)
                .FirstOrDefaultAsync();
            User ToUnfriend = await _dbContext.Users.Where(s => s.Id == id).FirstOrDefaultAsync();

            if (ActualUser == null || ToUnfriend == null)
                return;

            Friends original = await _dbContext.Friends.Where(s => s.UserId == ActualUser.Id)
                .Where(s => s.FriendId == ToUnfriend.Id).FirstOrDefaultAsync();
            Friends second = await _dbContext.Friends.Where(s => s.FriendId == ActualUser.Id)
                .Where(s => s.UserId == ToUnfriend.Id).FirstOrDefaultAsync();
            if (original == null || second == null)
                return;

            _dbContext.Friends.Remove(original);
            _dbContext.Friends.Remove(second);
            await _dbContext.SaveChangesAsync();

            // check if unfriended user is online - send him friend logged off message

            if (await _dbContext.Sessions.Where(s => s.UserId == ToUnfriend.Id).Where(s => s.IsActive).AnyAsync())
            {
                await Clients.User(ToUnfriend.Id).SendAsync("FriendDisconnected");
            }

            await Clients.Caller.SendAsync("FriendDisconnected");
            await LoadFriendsPage();

        }
               
        public class ListOfUsers
        {
            public string UserId { get; set; }
            public string DisplayedName { get; set; }
        }

        // Need DateTime published, Article name, Author Name, Category, Critics, Likes
        public class ArticleList
        {
            public string ArticleName { get; set; }
            public DateTime dateTime { get; set; }
            public string AuthorDateTime { get; set; }
            public string AuthorId { get; set; }
            public string Category { get; set; }
            public string CategoryId { get; set; }
            public int CriticsCount { get; set; }
            public int Likes { get; set; }
        }

        public async Task LoadCategories(string categoryGroupId)
        {
            int CategoryGroupId = int.Parse(categoryGroupId);
            List<Category> categories = await _dbContext.Categories.Where(s => s.CategoryGroupId == CategoryGroupId).OrderByDescending(s => s.Id).ToListAsync();
                        string output = JsonConvert.SerializeObject(categories);
           
            await Clients.Caller.SendAsync("receivedCategories", output);
        }


        public class FriendsList
        {
            public bool ShowPicture { get; set; } = false;
            public string FriendId { get; set; }
            public string FriendName { get; set; }
            public int FriendshipStatus { get; set; }  // 0 = not confirmed 1 = confirmed 2 = declined 
        }
               
        public class ReturnResult
        {
            public string JsonResult { get; set; }
            public bool IsError { get; set; }
            public string ErrorMessage { get; set; }
            public int ErrorId { get; set; }
        }
    }
}