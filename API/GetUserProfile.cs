using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    [Route("api/getuserprofile")]
    [ApiController]
    public class GetUserProfile : ControllerBase
    {
        private ApplicationDbContext _dbContext;
        private ILogger<Index> _logger;

        public GetUserProfile(ApplicationDbContext dbContext, ILogger<Index> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<string> OnGetAsync(string Id)
        {
            if (Id == null)
                return "Error";
            else
            {
                string userId = await _dbContext.Users.Where(s => s.Email == User.Identity.Name).Select(s => s.Id).FirstOrDefaultAsync();
                //add conditions if friends...
                var profile = await _dbContext.Users.Where(s => s.Id == Id).Include(s => s.Country).FirstOrDefaultAsync();
                int articles = await _dbContext.Articles.Where(s => s.UserId == Id).Where(s => s.IsPublished).CountAsync();
                int critics = await _dbContext.Critics.Where(s => s.CriticId == Id).CountAsync();

                if (!await _dbContext.UserSettings.Where(s => s.UserId == Id).AnyAsync())
                {
                    UserSettings temp = new UserSettings();
                    temp.UserId = Id;
                    await _dbContext.UserSettings.AddAsync(temp);
                    await _dbContext.SaveChangesAsync();
                }
                UserSettings userSettings = await _dbContext.UserSettings.Where(s => s.UserId == Id).FirstOrDefaultAsync();
                bool areFriends = await _dbContext.Friends.Where(s => s.UserId == Id).Where(s => s.FriendId == userId).AnyAsync();

                FullUserProfile fullUser = new FullUserProfile();
                if (User.IsInRole(Tools.Settings.Administrator))
                {
                    fullUser.Email = profile.Email;
                    fullUser.PhoneNumber = profile.PhoneNumber;
                    fullUser.FirstName = profile.FirstName;
                    fullUser.LastName = profile.LastName;
                    fullUser.Google = profile.Google;
                    fullUser.Microsoft = profile.Microsoft;
                    fullUser.Facebook = profile.Facebook;
                    fullUser.Twitter = profile.Twitter;
                    fullUser.Gender = profile.Gender;
                    TimeSpan Age = DateTime.UtcNow.Subtract(profile.BirthDate);
                    fullUser.Age = Age.Days / 365;
                    fullUser.CountryName = await _dbContext.Countries.Where(s => s.Id == profile.CountryId).Select(s => s.CountryName).FirstOrDefaultAsync();
                    var friends = await _dbContext.Friends.Include(s => s.Friend).Where(s => s.UserId == Id).Select(s => new
                    {
                        friendsList = s,
                        Id = s.FriendId,
                        DisplayedName = s.Friend.DisplayedName
                    }).ToListAsync();

                    List<Friendslist> friendslist = new List<Friendslist>();
                    foreach (var friend in friends)
                    {
                        friendslist.Add(new Friendslist { FriendId = friend.Id, FriendDisplayedName = friend.DisplayedName });
                    }
                    fullUser.Friends = friendslist;
                    fullUser.PictureUrl = "/Images/" + Id + "/ProfilePhoto_180.png";
                }
                fullUser.DisplayedName = profile.DisplayedName;
                fullUser.Info = profile.Info;
                fullUser.RegistrationTime = profile.RegistrationTime;

                if ((userSettings.BirthDatePrivacy == Tools.Settings.AccessRights.Public)
                    || ((userSettings.BirthDatePrivacy == Tools.Settings.AccessRights.Friends) && areFriends))
                {
                    fullUser.BirthDate = profile.BirthDate;
                    TimeSpan Age = DateTime.UtcNow.Subtract(profile.BirthDate);
                    fullUser.Age = Age.Days / 365;
                }

                if ((userSettings.NamePrivacy == Tools.Settings.AccessRights.Public)
                    || ((userSettings.NamePrivacy == Tools.Settings.AccessRights.Friends) && areFriends))
                {
                    fullUser.FirstName = profile.FirstName;
                    TimeSpan Age = DateTime.UtcNow.Subtract(profile.BirthDate);
                    fullUser.LastName = profile.LastName;
                }

                if ((userSettings.EmailPrivacy == Tools.Settings.AccessRights.Public)
                    || ((userSettings.EmailPrivacy == Tools.Settings.AccessRights.Friends) && areFriends))
                {
                    fullUser.Email = profile.Email;
                }

                if ((userSettings.CountryPrivacy == Tools.Settings.AccessRights.Public)
                    || ((userSettings.CountryPrivacy == Tools.Settings.AccessRights.Friends) && areFriends))
                {
                    fullUser.CountryName = await _dbContext.Countries.Where(s => s.Id == profile.CountryId).Select(s => s.CountryName).FirstOrDefaultAsync();
                }

                if ((userSettings.FrindsPrivacy == Tools.Settings.AccessRights.Public)
                    || ((userSettings.FrindsPrivacy == Tools.Settings.AccessRights.Friends) && areFriends))
                {
                    var friends = await _dbContext.Friends.Include(s => s.Friend).Where(s => s.UserId == Id).Select(s => new
                    {
                        friendsList = s,
                        Id = s.FriendId,
                        DisplayedName = s.Friend.DisplayedName
                    }).ToListAsync();

                    List<Friendslist> friendslist = new List<Friendslist>();
                    foreach (var friend in friends)
                    {
                        friendslist.Add(new Friendslist { FriendId = friend.Id, FriendDisplayedName = friend.DisplayedName });
                    }
                    fullUser.Friends = friendslist;
                }

                if ((userSettings.PictureFilePrivacy == Tools.Settings.AccessRights.Public)
                    || ((userSettings.PictureFilePrivacy == Tools.Settings.AccessRights.Friends) && areFriends))
                {
                    fullUser.PictureUrl = "/Images/" + Id + "/ProfilePhoto_180.png";
                }
                else fullUser.PictureUrl = "/img/admin.png";

                string Serialized = JsonConvert.SerializeObject(fullUser);
                return Serialized;
            }
        }

        public class Friendslist
        {
            public string FriendId { get; set; }
            public string FriendDisplayedName { get; set; }
        }

        [HttpGet]
        public async Task<ActionResult> OnGetAsync()
        {
            return NotFound();
        }
    }
}