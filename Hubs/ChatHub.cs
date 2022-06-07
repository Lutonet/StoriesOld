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
    public class ChatHub : Hub
    {
        private ApplicationDbContext _dbContext;
        private ILogger<ChatHub> _logger;
        private bool isChatAdmin = false;

        public ChatHub(ApplicationDbContext dbContext, ILogger<ChatHub> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /* When users connect to the hub */

        public override async Task<Task> OnConnectedAsync()
        {
            User user = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).FirstOrDefaultAsync();

            string UserName = user.Email;
            string DisplayedName = user.DisplayedName;
            string UserId = user.Id;
            string ConnectionId = Context.ConnectionId;
            DateTime Connected = DateTime.UtcNow;

            //Check if user is moderator or administrator
            if (Context.User.IsInRole(Tools.Settings.Administrator) || Context.User.IsInRole(Tools.Settings.Moderator))
            {
                if (!await _dbContext.ChatElevated.Where(s => s.UserId == UserId).AnyAsync())
                {
                    ChatElevated elevatedUser = new ChatElevated();
                    elevatedUser.UserId = UserId;
                    await _dbContext.ChatElevated.AddAsync(elevatedUser);
                    isChatAdmin = true;
                }
            }

            //Check if user is banned from the chat

            // Get count of active session for user - if it is zero start the counter
            int sessionsForUser = await _dbContext.UsersInRooms.Where(s => s.UserId == UserId).CountAsync();
            if (sessionsForUser == 0) await StartCountingTime();
            await ChangeRoom(Settings.DefaultChatroom, "", ConnectionId);

            return base.OnConnectedAsync();
        }

        // When user Disconnects

        public override async Task<Task> OnDisconnectedAsync(Exception ex)
        {
            User user = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).FirstOrDefaultAsync();
            string UserName = user.Email;
            string DisplayedName = user.DisplayedName;
            string UserId = user.Id;

            string LastRoom = await _dbContext.UsersInRooms.Where(s => s.ConnectionId == Context.ConnectionId).Select(s => s.RoomName).FirstOrDefaultAsync();
            await LeaveRoom(LastRoom, Context.ConnectionId);

            // check if it is admin and remove him
            if (await _dbContext.ChatRoomAdmins.Where(s => s.UserId == UserId).AnyAsync())
            {
                var toDelete = await _dbContext.ChatRoomAdmins.Where(s => s.UserId == UserId).ToListAsync();
                foreach (var del in toDelete)
                {
                    _dbContext.ChatRoomAdmins.Remove(del);
                }

                await _dbContext.SaveChangesAsync();
            }

            if (await _dbContext.ChatElevated.Where(s => s.UserId == UserId).AnyAsync())
            {
                List<ChatElevated> tobeDeleted = await _dbContext.ChatElevated.Where(s => s.UserId == UserId).ToListAsync();
                foreach (var del in tobeDeleted)
                {
                    _dbContext.ChatElevated.Remove(del);
                }
                await _dbContext.SaveChangesAsync();
            }

            if (await _dbContext.UsersInRooms.Where(s => s.UserId == UserId).AnyAsync())
            {
                List<UserInRoom> usersInRooms = await _dbContext.UsersInRooms.Where(s => s.UserId == UserId).ToListAsync();
                foreach (var del in usersInRooms)
                {
                    _dbContext.UsersInRooms.Remove(del);
                }
                await _dbContext.SaveChangesAsync();
            }

            await StopCountingTime(UserId);
            var records = await (_dbContext.UsersInRooms.Where(s => s.ConnectionId == Context.ConnectionId).ToListAsync());
            foreach (var record in records)
            {
                _dbContext.UsersInRooms.Remove(record);
            }
            await _dbContext.SaveChangesAsync();
            return base.OnDisconnectedAsync(ex);
        }

        // Changing the room or entering first room (add "" instead of the room name)

        public async Task ChangeRoom(string NewRoomName, string OldRoomName, string connectionId)
        {
            string UserId = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).Select(s => s.Id).FirstOrDefaultAsync();
            User user = await _dbContext.Users.Where(s => s.Id == UserId).FirstOrDefaultAsync();
            string Email = user.Email;
            string UserName = user.UserName;
            string DisplayedName = user.DisplayedName;
            DateTime ChangeTime = DateTime.UtcNow;
            if (NewRoomName == null) NewRoomName = "Living room";

            isChatAdmin = false;
            if (await _dbContext.ChatElevated.Where(s => s.UserId == UserId).AnyAsync())
                isChatAdmin = true;

            if (OldRoomName != "")
            {
                await LeaveRoom(OldRoomName, Context.ConnectionId);
                /* We are changing the room - so we must first leave previous one */
            }

            /* modified 28/06/2021 */
            if (!isChatAdmin)
            {
                if ((await _dbContext.ChatBans.Where(s => s.UserId == user.Id).Where(s => s.GroupName == NewRoomName).Where(s => s.BannedUntil.CompareTo(DateTime.UtcNow) > 0).AnyAsync())
                    || (await _dbContext.ChatBans.Where(s => s.UserId == user.Id).Where(s => s.BanForAllChat == true).Where(s => s.BannedUntil.CompareTo(DateTime.UtcNow) > 0).AnyAsync()))
                {
                    TimeSpan bannedTimeLeft = (await _dbContext.ChatBans.Where(s => s.UserId == user.Id).Where(s => s.GroupName == NewRoomName).Where(s => s.BannedUntil.CompareTo(DateTime.UtcNow) > 0).OrderByDescending(s => s.BannedUntil).Select(s => s.BannedUntil).FirstOrDefaultAsync()).Subtract(DateTime.UtcNow);
                    Model.ChatBans ban = await _dbContext.ChatBans.Where(s => s.UserId == user.Id).Where(s => s.GroupName == NewRoomName).Where(s => s.BannedUntil > DateTime.UtcNow).FirstOrDefaultAsync();
                    if (ban == null)
                        ban = await _dbContext.ChatBans.Where(s => s.UserId == user.Id).Where(s => s.BanForAllChat).Where(s => s.BannedUntil > DateTime.UtcNow).FirstOrDefaultAsync();
                    await SendBannedMessage(user.Id, ban.ReasonForBan, ban.BannedUntil, NewRoomName);
                    return;
                }
            }

            int usersInRoom = await _dbContext.UsersInRooms.Where(s => s.RoomName == NewRoomName).CountAsync();
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, NewRoomName);
                await UserJoinedRoom(UserId, NewRoomName, DisplayedName);
            }
            catch (Exception exc)
            {
                return;
            }
            /* User is in the group - record it to the database */
            // Update UserInRoom
            UserInRoom createdRecord = new UserInRoom()
            {
                ConnectionId = Context.ConnectionId,
                IsAdmin = isChatAdmin,
                DisplayedName = DisplayedName,
                IsOwner = false,
                UserId = UserId,
                JoiningTime = ChangeTime,
                RoomName = NewRoomName
            };
            await _dbContext.UsersInRooms.AddAsync(createdRecord);
            await _dbContext.SaveChangesAsync();

            // Check if room has an admin
            if (!await _dbContext.ChatRoomAdmins.Where(s => s.RoomName == NewRoomName).AnyAsync())
            {
            
                await SetAdmin(NewRoomName, UserId, isChatAdmin);
            }
            else
                if (isChatAdmin) await SetAdmin(NewRoomName, UserId, isChatAdmin);
            // CheckRoomType
            string RoomType = await _dbContext.ChatRooms.Where(s => s.RoomName == NewRoomName).Select(s => s.RoomType).FirstOrDefaultAsync();
            await Clients.Caller.SendAsync("receivedSwitchGroup", NewRoomName, RoomType);

            await RequestUserList(NewRoomName);
            if (RoomType == "private")
            {
                await Clients.Caller.SendAsync("receivedJoinedPrivateRoom", NewRoomName);
            }
            await ListRooms();
        }

        // Send the message
        public async Task SendMessage(string message, string roomName, string sendTo, string userColours)
        {
            User user = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).FirstOrDefaultAsync();
            string UserName = user.Email;
            string DisplayedName = user.DisplayedName;
            string SendToDisplayed = await _dbContext.Users.Where(s => s.Id == sendTo).Select(s => s.DisplayedName).FirstOrDefaultAsync();
            string UserId = user.Id;
            if (sendTo == "everyone")
            {
                // send to group
                await Clients.Group(roomName).SendAsync("receivedMessage", UserId, DisplayedName, message, roomName, userColours);
            }
            else
            {
                //send to user
                await Clients.User(sendTo).SendAsync("receivedPrivateMessage", UserId, DisplayedName, message, userColours);
                await Clients.Caller.SendAsync("receivedPrivateMessage", UserId, "<u>From You to:</u> " + SendToDisplayed, message, userColours);
            }
            // record to the database
            Chat chatMessage = new Chat()
            {
                IsAdminMessage = false,
                SentAt = DateTime.UtcNow,
                SenderId = user.Id,
                RecepientId = await _dbContext.Users.Where(s => s.Email == sendTo).Select(s => s.Id).FirstOrDefaultAsync(),
                ChatroomName = roomName,
                Message = message
            };
            await _dbContext.Chats.AddAsync(chatMessage);
            await _dbContext.SaveChangesAsync();
        }

        // Send Admin message to everyone

        public async Task SendAdminMessage(string message)
        {
            if (!(Context.User.IsInRole(Settings.Administrator)) || (Context.User.IsInRole(Settings.Moderator)))
            {
                _logger.LogCritical("User:" + Context.User.Identity.Name + "tries to send global message as administrator!!");
                return;
            }
            User user = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).FirstOrDefaultAsync();
            string UserName = user.Email;
            string DisplayedName = user.DisplayedName;
            string UserId = user.Id;

            await Clients.All.SendAsync("receivedAdminMessage", user.Id, message);

            Chat chatMessage = new Chat()
            {
                IsAdminMessage = true,
                SentAt = DateTime.UtcNow,
                SenderId = user.Id,
                Message = message
            };
            await _dbContext.Chats.AddAsync(chatMessage);
            await _dbContext.SaveChangesAsync();
        }

        // Send admin message to particular user

        public async Task SendAdminMessageToUser(string userId, string message)
        {
            User user = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).FirstOrDefaultAsync();
            string UserName = user.Email;
            string DisplayedName = user.DisplayedName;
            string UserId = user.Id;
            string Recepient = await _dbContext.Users.Where(s => s.Id == userId).Select(s => s.Email).FirstOrDefaultAsync();
            if (await _dbContext.ChatRoomAdmins.Where(s => s.UserId == user.Id).CountAsync() == 0)
            {
                _logger.LogCritical("User" + user.Id + "tries to send admin message without being admin");
                await BanUserFromChat(user.Id, "Trying to fake Administrator identity", DateTime.UtcNow.AddHours(2.0));
            }
            await Clients.User(userId).SendAsync("receivedAdminMessage", message);

            if (await _dbContext.ChatRoomAdmins.Where(s => s.UserId == user.Id).AnyAsync())
            {
                Chat chatMessage = new Chat()
                {
                    IsAdminMessage = true,
                    SentAt = DateTime.UtcNow,
                    SenderId = user.Id,
                    RecepientId = userId,
                    Message = message
                };
                await _dbContext.Chats.AddAsync(chatMessage);
                await _dbContext.SaveChangesAsync();
            }
        }

        //Send admin message to the group

        public async Task SendAdminGroupMessage(string GroupName, string message)
        {
            User user = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).FirstOrDefaultAsync();
            string UserName = user.Email;
            string DisplayedName = user.DisplayedName;
            string UserId = user.Id;
            string RoomName = GroupName;
            if (await _dbContext.ChatRoomAdmins.Where(s => s.UserId == user.Id).CountAsync() < 1)
            {
                _logger.LogCritical("User" + user.Id + "tries to send admin message without being admin");
                await BanUserFromChat(user.Id, "Trying to fake Administrator identity", DateTime.UtcNow.AddHours(2.0));
            }
            await Clients.Group(GroupName).SendAsync("receivedAdminMessage", message);

            if (await _dbContext.ChatRoomAdmins.Where(s => s.UserId == user.Id).AnyAsync())
            {
                Chat chatMessage = new Chat()
                {
                    IsAdminMessage = true,
                    SentAt = DateTime.UtcNow,
                    SenderId = UserId,
                    RecepientId = UserId,
                    ChatroomName = RoomName,
                    Message = message
                };
                await _dbContext.Chats.AddAsync(chatMessage);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task CreateRoom(string roomName, string roomType) //user created room - join the room, make user owner and admin - public, private
        {
            User user = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).FirstOrDefaultAsync();
            string UserName = user.Email;
            string DisplayedName = user.DisplayedName;
            string UserId = user.Id;

            Model.ChatRooms newRoom = new Model.ChatRooms()
            {
                RoomName = roomName,
                Created = DateTime.UtcNow,
                IsActive = true,
                UserId = user.Id,
                RoomType = roomType
            };
            await _dbContext.ChatRooms.AddAsync(newRoom);
            await _dbContext.SaveChangesAsync();
            await ChangeRoom(roomName, await _dbContext.UsersInRooms.Where(s => s.ConnectionId == Context.ConnectionId).Select(s => s.RoomName).FirstOrDefaultAsync(), Context.ConnectionId);
            await SetAdmin(roomName, UserId);
            await ListRooms(true);
            await RequestUserList(roomName);
            await RequestAdminsList(roomName);
        }

        public async Task LeaveRoom(string LastRoom, string connectionId)
        {
            if (LastRoom == null) return;
            User user = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).FirstOrDefaultAsync();
            string UserName = user.Email;
            string DisplayedName = user.DisplayedName;
            string UserId = user.Id;
            // Check if it is an admin
            if (await _dbContext.ChatRoomAdmins.Where(s => s.RoomName == LastRoom).Where(s => s.UserId == UserId).AnyAsync())
            {
                List<ChatRoomAdmins> chatRoomAdmins = await _dbContext.ChatRoomAdmins.Where(s => s.UserId == UserId).ToListAsync();
                foreach (var admin in chatRoomAdmins)
                {
                    _dbContext.ChatRoomAdmins.Remove(admin);
                }
                await _dbContext.SaveChangesAsync();
            }
            //let count if any user is in room
            int userCount = await _dbContext.UsersInRooms.Where(s => s.RoomName == LastRoom).CountAsync();
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, LastRoom);
      
            if ((await _dbContext.ChatRoomAdmins.Where(s => s.RoomName == LastRoom).CountAsync() < 1) && (userCount > 0))
                await SetAdmin(LastRoom, "");
            //leave the room and record it

            //check if user has only one ConnectionId related to the room
            if (await _dbContext.UsersInRooms.Where(s => s.RoomName == LastRoom).Where(s => s.UserId == UserId).CountAsync() == 1)
            {
                await UserLeftRoom(user.Id, LastRoom, DisplayedName);
                //remove user from room
            }
            // user has more connection from the room - without messaging users or changing user in room list, remove Connection from the DB

            UserInRoom useToRemove = await _dbContext.UsersInRooms.Where(s => s.UserId == UserId).FirstOrDefaultAsync();
            if (useToRemove != null) _dbContext.UsersInRooms.Remove(useToRemove);
            await _dbContext.SaveChangesAsync();

            if (await _dbContext.UsersInRooms.Where(s => s.RoomName == LastRoom).CountAsync() == 0)
            {
                if (LastRoom != "Living room")
                    await RemoveRoom(LastRoom);
                await ListRooms();
            }
        }

        /* Owner deletes the room - kick everyone out */

        private async Task RemoveRoom(string roomName)
        {
            var roomToDelete = await _dbContext.ChatRooms.Where(s => s.RoomName == roomName).FirstOrDefaultAsync();
            _dbContext.ChatRooms.Remove(roomToDelete);
            await _dbContext.SaveChangesAsync();
            await ListRooms(true);
        }

        public async Task DeleteRoom(string roomName) // kick out everyone in room except sender to the room 0
        {
            User user = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).FirstOrDefaultAsync();
            string UserName = user.Email;
            string DisplayedName = user.DisplayedName;
            string UserId = user.Id;

            //Check if the user is owner of the room
            if (await _dbContext.ChatRooms.Where(s => s.RoomName == roomName).Where(s => s.UserId == UserId).Select(s => s.UserId).FirstOrDefaultAsync() != UserId)
            {
                _logger.LogCritical("User" + user.Id + "tries to delete room which doesn't owns");
                await BanUserFromChat(user.Id, "Trying to delete foreigner room", DateTime.UtcNow.AddHours(1.0));
                return;
            }

            IList<string> usersInRoom = await _dbContext.UsersInRooms.Where(s => s.RoomName == roomName).Select(s => s.UserId).ToListAsync();
            List<userIdToEmail> listOfEmails = new List<userIdToEmail>();
            foreach (var useroom in usersInRoom)
            {
                userIdToEmail UserIDCls = new userIdToEmail()
                {
                    UserId = useroom,
                    Email = await _dbContext.Users.Where(s => s.Id == useroom).Select(s => s.Email).FirstOrDefaultAsync()
                };
                listOfEmails.Add(UserIDCls);
            }
            foreach (var userInRoom in listOfEmails)
            {
                await ChangeRoom(Settings.DefaultChatroom, roomName, userInRoom.Email);
            }
        }

        public class userIdToEmail
        {
            public string UserId { get; set; }
            public string Email { get; set; }
        }

        // give user info about actual rooms, Type of room (public)
        public async Task ListRooms(bool forEveryone = false)
        {
            List<Stories.Model.ChatRooms> chatRooms = await _dbContext.ChatRooms.Where(s => s.IsActive).Where(s => s.RoomType == "public").ToListAsync();
            string serialized = JsonConvert.SerializeObject(chatRooms);
      
            if (!forEveryone)
                await Clients.Caller.SendAsync("receivedRoomsList", serialized);
            else await Clients.All.SendAsync("receivedRoomsList", serialized);
        }

        public async Task RequestKickUser(string groupName, string userId, int adminId, string adminHash)
        {
            if (await _dbContext.UsersInRooms.Where(s => s.RoomName == groupName).Where(s => s.UserId == userId).AnyAsync())
            {
                string requestorId = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).Select(s => s.Id).FirstOrDefaultAsync();
                ChatRoomAdmins requestor = await _dbContext.ChatRoomAdmins.Where(s => s.UserId == requestorId).FirstOrDefaultAsync();
                if ((requestor.UserId != requestorId) || (requestor.AdminHash != adminHash) || (requestor.Id != adminId))
                {
                    return;
                }
                else
                {
                    await Clients.User(userId).SendAsync("receivedKick");
                }
            }
        }

        public async Task RequestAddAdmin(string groupName, string userId, int adminId, string adminHash)
        {
            string requestorId = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).Select(s => s.Id).FirstOrDefaultAsync();
            ChatRoomAdmins requestor = await _dbContext.ChatRoomAdmins.Where(s => s.Id == adminId).FirstOrDefaultAsync();
            if ((requestor.UserId != requestorId) || (requestor.AdminHash != adminHash) || (requestor.Id != adminId))
            {
                return;
            }
            else
            {
                await SetAdmin(groupName, userId, false);
                await Clients.Caller.SendAsync("refreshAdmin");
            }
        }

        public async Task SetAdmin(string groupName, string userId, bool isChatAdmin = false)
        {
      
            // if userId "" we must set user from group and make him admin
            if (userId == "")
            {
                string requestorId = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).Select(s => s.Id).FirstOrDefaultAsync();
                if (await _dbContext.UsersInRooms.Where(s => s.RoomName == groupName).Where(s => s.UserId != requestorId).Select(s => s.UserId).AnyAsync())
                    userId = await _dbContext.UsersInRooms.Where(s => s.RoomName == groupName).Where(s => s.UserId != requestorId).Select(s => s.UserId).FirstOrDefaultAsync();
                else userId = requestorId;
            }
            ChatRoomAdmins newAdmin = new ChatRoomAdmins()
            {
                RoomName = groupName,
                UserId = userId,
                AdminHash = GenerateRandomString.RandomString(8),
                isElevated = isChatAdmin
            };
            if (await _dbContext.ChatRoomAdmins.Where(s => s.UserId == newAdmin.UserId).AnyAsync())
            {
                List<ChatRoomAdmins> temp = await _dbContext.ChatRoomAdmins.Where(s => s.UserId == newAdmin.UserId).ToListAsync();
                foreach (var field in temp)
                {
                    _dbContext.ChatRoomAdmins.Remove(field);
                }
                await _dbContext.SaveChangesAsync();
            }
            await _dbContext.ChatRoomAdmins.AddAsync(newAdmin);
            await _dbContext.SaveChangesAsync();

      
            bool isOwner = await _dbContext.ChatRooms.Where(s => s.UserId == userId).AnyAsync();
            string userEmail = await _dbContext.Users.Where(s => s.Id == userId).Select(s => s.Email).FirstOrDefaultAsync();
            await Clients.User(userId).SendAsync("receivedYouAreAdmin",
                               newAdmin.AdminHash, isChatAdmin, newAdmin.Id, isOwner);
        }

        public async Task RemoveAdmin(string groupName, string userId)
        {
            ChatRoomAdmins admin = await _dbContext.ChatRoomAdmins.Where(s => s.RoomName == groupName).Where(s => s.UserId == userId).FirstOrDefaultAsync();
            _dbContext.ChatRoomAdmins.Remove(admin);
            await _dbContext.SaveChangesAsync();
            await Clients.Group(groupName).SendAsync("receivedAdminRemoved", userId);
            await Clients.User(userId).SendAsync("youDemoted");
        }

        public async Task RequestRemoveAdmin(string groupName, string userId, int adminId, string adminHash)
        {
            string requestorId = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).Select(s => s.Id).FirstOrDefaultAsync();
            ChatRoomAdmins requestor = await _dbContext.ChatRoomAdmins.Where(s => s.Id == adminId).FirstOrDefaultAsync();
            if ((requestor.UserId != requestorId) || (requestor.AdminHash != adminHash) || (requestor.Id != adminId))
            {
                return;
            }
            else
            {
                ChatRoomAdmins admin = await _dbContext.ChatRoomAdmins.Where(s => s.UserId == userId).FirstOrDefaultAsync();
                if (admin == null) return;

                if (!admin.isElevated && requestor.isElevated)
                {
                    if (await _dbContext.ChatRoomAdmins.Where(s => s.RoomName == groupName).CountAsync() == 1)
                        await Clients.Caller.SendAsync("noOtherAdmin");
                    await RemoveAdmin(groupName, userId);
                    await Clients.Caller.SendAsync("refreshAdmin");
                }
            }
        }

        public async Task RequestUserList(string groupName, bool addForeignerGroups = false)
        {
            //We need UserId, DisplayedName, PictureLink

            List<string> connectionsList = await _dbContext.UsersInRooms.Where(s => s.RoomName == groupName).OrderBy(s => s.UserId).Select(s => s.UserId).ToListAsync();
            List<InitialUserList> listToPublish = new List<InitialUserList>();
            List<string> filteredConnectionList = new List<string>();
            List<User> userListCache = await _dbContext.Users.ToListAsync();
            List<string> nameCache = new List<string>();
            foreach (string connection in connectionsList)
            {
                bool recordExists = false;
                foreach (var name in nameCache)
                {
                    if (name == connection)
                    {
                        recordExists = true;
                    }
                }

                if (!recordExists)
                {
                    filteredConnectionList.Add(connection);
                    nameCache.Add(connection);
                }
            }

            foreach (string connection in filteredConnectionList)
            {
                InitialUserList initialUser = new InitialUserList();

                initialUser.UserId = connection;
                initialUser.DisplayedName = userListCache.Where(s => s.Id == connection).Select(s => s.DisplayedName).FirstOrDefault();
                initialUser.PictureAddress = "/Images/" + connection + "ProfilePhoto_32.png";
                if (await _dbContext.ChatElevated.Where(s => s.UserId == connection).AnyAsync())
                    initialUser.IsElevated = true;
                listToPublish.Add(initialUser);
            }
            listToPublish = listToPublish.OrderBy(s => s.DisplayedName).ToList();
            string jsonList = JsonConvert.SerializeObject(listToPublish);
      
            string adminList = JsonConvert.SerializeObject(await _dbContext.ChatRoomAdmins.Where(s => s.RoomName == groupName).Select(s => s.UserId).ToListAsync());
            await Clients.Caller.SendAsync("receiveListOfUsers", jsonList, adminList);
        }

        public async Task RequestUsersFromOthers(string groupName)
        {
            List<UsersFromOtherGroups> usersFromOtherGroups = new List<UsersFromOtherGroups>();

            List<string> Rooms = await _dbContext.ChatRooms.Where(s => s.RoomType != "private").Where(s => s.RoomName != groupName).Select(s => s.RoomName).ToListAsync();
            List<string> IdUsers = new List<string>();
            foreach (var room in Rooms)
            {
                IdUsers.AddRange(await _dbContext.UsersInRooms.Where(s => s.RoomName == room).Select(s => s.UserId).ToListAsync());
            }

            List<User> userCache = await _dbContext.Users.ToListAsync();
            foreach (var user in IdUsers)
            {
                User userFromCache = userCache.Where(s => s.Id == user).FirstOrDefault();
                UsersFromOtherGroups userToGroup = new UsersFromOtherGroups();
                userToGroup.UserId = user;
                userToGroup.DisplayedName = userFromCache.DisplayedName;
                userToGroup.isElevated = await _dbContext.ChatElevated.Where(s => s.UserId == user).AnyAsync();
                userToGroup.RoomName = await _dbContext.UsersInRooms.Where(s => s.UserId == user).Select(s => s.RoomName).FirstOrDefaultAsync();
                userToGroup.isAdmin = await _dbContext.ChatRoomAdmins.Where(s => s.RoomName == userToGroup.RoomName).Where(s => s.UserId == user).AnyAsync();
                usersFromOtherGroups.Add(userToGroup);
            }
            usersFromOtherGroups = usersFromOtherGroups.OrderBy(s => s.RoomName).OrderBy(s => s.isElevated).OrderBy(s => s.isAdmin).OrderBy(s => s.DisplayedName).ToList();
            string output = JsonConvert.SerializeObject(usersFromOtherGroups);
            await Clients.Caller.SendAsync("receiveOtherGroupsUsers", output);
        }

        public class UsersFromOtherGroups
        {
            public string UserId { get; set; }
            public string DisplayedName { get; set; }
            public string RoomName { get; set; }
            public bool isElevated { get; set; }
            public bool isAdmin { get; set; }
        }

        public async Task RequestSendInvite(string userId, string RoomName)
        {
            string administratorName = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).Select(s => s.DisplayedName).FirstOrDefaultAsync();
            string administratorId = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).Select(s => s.Id).FirstOrDefaultAsync();
            await Clients.User(userId).SendAsync("receivedInvite", administratorId, administratorName, RoomName);
            await Clients.Caller.SendAsync("receivedSystemMessage", "Invitation for <b><u>" + administratorName + "</u></b> was sent.");
        }

        public async Task RequestAdminsList(string groupName)
        {
            string serialized = JsonConvert.SerializeObject(await _dbContext.ChatRoomAdmins.Where(s => s.RoomName == groupName).Select(s => s.UserId).ToListAsync());
      
            await Clients.Caller.SendAsync("receivedAdminsList", serialized);
        }

        /**** Here is list of functions for hub (not for user)             ****/

        public class InitialUserList
        {
            // we need UserId, Displayed name, picture address, is Admin
            public string UserId { get; set; }

            public string DisplayedName { get; set; }
            public string PictureAddress { get; set; }
            public bool IsElevated { get; set; } = false;
        }

        public async Task SendErrorMessage(string userId, string Message, string SenderName)
        {
            string userEmail = await _dbContext.Users.Where(s => s.Id == userId).Select(s => s.Email).FirstOrDefaultAsync();
            await Clients.User(userEmail).SendAsync("receivedErrorMessage", Message);
        }

        public async Task UserLeftRoom(string userId, string lastRoomName, string displayedName)
        {
            await Clients.Groups(lastRoomName).SendAsync("receivedUserLeft", userId, displayedName);
        }

        public async Task UserJoinedRoom(string userId, string roomName, string displayedName)
        {
            await Clients.Groups(roomName).SendAsync("receivedUserJoined", userId, displayedName);
        }

        public async Task StartCountingTime()
        {
            string userId = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).Select(s => s.Id).FirstOrDefaultAsync();

            if (await _dbContext.ChattingTimes.Where(s => s.UserId == userId).CountAsync() == 0)
            {
                ChattingTime chattingTime = new ChattingTime()
                {
                    UserId = userId,
                    StartTime = DateTime.UtcNow,
                    TimeInChatSuma = DateTime.UtcNow.AddSeconds(0.1).Subtract(DateTime.UtcNow)
                };
                await _dbContext.ChattingTimes.AddAsync(chattingTime);
                await _dbContext.SaveChangesAsync();
            }

            ChattingTime time = await _dbContext.ChattingTimes.Where(s => s.UserId == userId).Select(s => s).FirstOrDefaultAsync();
            time.StartTime = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }

        public async Task StopCountingTime(string userId)
        {
            ChattingTime chattingTime = await _dbContext.ChattingTimes.Where(s => s.UserId == userId).FirstOrDefaultAsync();
            TimeSpan thisChat = DateTime.UtcNow.Subtract(chattingTime.StartTime);
            TimeSpan allTogether = thisChat.Add(chattingTime.TimeInChatSuma);
            chattingTime.TimeInChatSuma = allTogether;
            await _dbContext.SaveChangesAsync();
        }

        private async Task SendBannedMessage(string UserId, string reason, DateTime until, string roomName)
        {
            TimeSpan banLength = until.Subtract(DateTime.UtcNow);
            double banSeconds = banLength.TotalSeconds;

            await Clients.User(UserId).SendAsync("receivedBan", reason, (int)banSeconds, roomName);
        }

        private async Task SendBannedFromChatMessage(string UserId, string reason, DateTime until)
        {
            string userName = await _dbContext.Users.Where(s => s.Id == UserId).Select(s => s.Email).FirstOrDefaultAsync();
            TimeSpan banLength = until.Subtract(DateTime.UtcNow);
            double banSeconds = banLength.TotalSeconds;
            await Clients.User(UserId).SendAsync("receivedChatBan", reason, (int)banSeconds);
        }

        /* modified 27/06/2021 */

        public async Task RequestBanUser(string roomName, string userId, string reason, string banLength, bool chatBan, int adminId, string adminKey)
        {
            // check if user has right to perform ban
            string requestorId = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).Select(s => s.Id).FirstOrDefaultAsync();
            ChatRoomAdmins requestor = await _dbContext.ChatRoomAdmins.Where(s => s.Id == adminId).FirstOrDefaultAsync();
            bool bannedUserIsElevated = await _dbContext.ChatElevated.Where(s => s.UserId == userId).AnyAsync();
            bool requestorIsElevated = await _dbContext.ChatElevated.Where(s => s.UserId == requestorId).AnyAsync();
         
            if ((requestor.UserId != requestorId) || (requestor.AdminHash != adminKey) || (requestor.Id != adminId))
            {
                return;
            }
            else
            {
                if (!requestor.isElevated || bannedUserIsElevated)
                {
                    return;
                }
                else
                {
                    if (!chatBan)
                        await BanUser(userId, reason, roomName, DateTime.UtcNow.AddSeconds(int.Parse(banLength)));
                    else
                        await BanUserFromChat(userId, reason, DateTime.UtcNow.AddSeconds(int.Parse(banLength)));
                }
            }
        }

        // user banned from some room
        public async Task BanUser(string UserId, string reason, string roomName, DateTime until)
        {
            // get actual user ID =
            string adminId = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).Select(s => s.Id).FirstOrDefaultAsync();
            // this is ban from the room - can be done either by moderator or roomadmin
            if (Context.User.IsInRole(Settings.Moderator) || await _dbContext.ChatRoomAdmins.Where(s => s.UserId == adminId).AnyAsync())
            {
                ChatBans ban = new ChatBans()
                {
                    AdminUserId = adminId,
                    UserId = UserId,
                    GroupName = roomName,
                    ReasonForBan = reason,
                    BannedUntil = until,
                    BanForAllChat = false
                };
                await _dbContext.ChatBans.AddAsync(ban);
                await _dbContext.SaveChangesAsync();
                string bannedEmail = await _dbContext.Users.Where(s => s.Id == UserId).Select(s => s.Email).FirstOrDefaultAsync();
                await SendBannedMessage(UserId, reason, until, roomName);
            }
            else
            {
                await BanUserFromChat(adminId, "Faking identity while trying to ban other user", DateTime.UtcNow.AddHours(3));
            }
        }

        //user banned from whole chat
        public async Task BanUserFromChat(string UserId, string reason, DateTime until)
        {
            // get actual user ID =
            string adminId = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).Select(s => s.Id).FirstOrDefaultAsync();
            // this is ban from the chat system - can be done only by moderator
            if (Context.User.IsInRole(Settings.Moderator))
            {
                ChatBans ban = new ChatBans()
                {
                    AdminUserId = adminId,
                    UserId = UserId,
                    BannedUntil = until,
                    ReasonForBan = reason,
                    BanForAllChat = true
                };
                await _dbContext.ChatBans.AddAsync(ban);
                await _dbContext.SaveChangesAsync();
                string bannedEmail = await _dbContext.Users.Where(s => s.Id == UserId).Select(s => s.Email).FirstOrDefaultAsync();
                await SendBannedFromChatMessage(UserId, reason, until);
            }
            else
            {
                await BanUserFromChat(adminId, "Faking identity while trying to ban other user", DateTime.UtcNow.AddHours(3));
                _logger.LogCritical("User faking identity while trying to ban other user: " + Context.User.Identity.Name);
            }
        }

        //During startup - list of public rooms

        public class ActiveConnectionInfo
        {
            public string ConnectionId { get; set; }
            public string UserId { get; set; }
            public string DisplayedName { get; set; }
            public string Email { get; set; }
        }
    }
}