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
    public class MessageHub : Hub
    {
        private ApplicationDbContext _dbContext;
        private ILogger<MessageHub> _logger;
        public static List<string> UsersOnline { get; set; } = new List<string>();
        public string jsonList { get; set; }
        public Message messageSent { get; set; }
        public int MessageId;
        public string senderId;
        public string jsonMessage { get; set; }
        public string RecepientId { get; set; }
        public List<DeliveryReport> deliveryReports = new List<DeliveryReport>();

        public MessageHub(ApplicationDbContext dbContext, ILogger<MessageHub> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public override async Task<Task> OnConnectedAsync()
        {
            User user = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).FirstOrDefaultAsync();
            if (user == null)
            {
                return null;
            }

            Console.WriteLine("User " + user.DisplayedName + " connected to email");
            UsersOnline.Add(user.Id);
            await RefreshNumbers();
            return base.OnConnectedAsync();
        }

        public override async Task<Task> OnDisconnectedAsync(Exception ex)
        {
            User user = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).FirstOrDefaultAsync();
            if (user == null)
            {
                return base.OnDisconnectedAsync(ex);
            }
            UsersOnline.Remove(user.Id);
            Console.WriteLine("User " + user.DisplayedName + " disconnected from email");
            return base.OnDisconnectedAsync(ex);
        }

        /* InitializeMailbox */

        public async Task InitializeMailbox(string page)
        {
            string actualUserId = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).Select(s => s.Id).FirstOrDefaultAsync();

            switch (page)
            {
                case "index":

                    /* For Table list we need Delivered time, Sender and Subject and  */
                    List<MessageToList> messages = await _dbContext.MessageRecipients.Include(s => s.Message)
                                                    .Include(s => s.Message.User)
                                                    .Include(s => s.Message.MessageRecipients)
                                                    .Where(s => s.Message.Deleted == false)
                                                    .Where(s => s.WasRead == false)
                                                    .Where(s => s.WasDeleted == false)
                                                    .Where(s => s.RecepientId == actualUserId)
                                                    .Select(s => new MessageToList()
                                                    {
                                                        MessageId = s.Message.Id,
                                                        SenderName = s.Message.User.DisplayedName,
                                                        Subject = s.Message.Subject,
                                                        Sent = s.Message.SentTime,
                                                        DeleteLink = "<div style='text-align: center'><a href=\"#\" onClick='DeleteMessage(" + s.Message.Id + ")'><span style='color: rgba(128,0,0,1); text-align: center;'><b><i class='fas fa-trash-alt'></i></b></span></a></div>",
                                                        ReadLink = "<div style='text-align:center'><a href=\"#\" onClick='ReadMessage(" + s.Message.Id + ")'><span style='color: rgba(0,128,0, 1); text-align: center;'><b><i class='fas fa-book-open'></i></b></span></a></div>",
                                                        WasRead = s.WasRead
                                                    })
                                                    .OrderByDescending(s => s.Sent)
                                                    .ToListAsync();

                    if (messages.Count > 0)
                    {
                        jsonList = JsonConvert.SerializeObject(messages);
                    }
                    else
                        jsonList = "";

                    await Clients.Caller.SendAsync("receivedMailboxData", jsonList);
                    await RefreshNumbers();

                    break;

                case "inbox":

                    List<MessageToList> msg = await _dbContext.MessageRecipients
                                                   .Include(s => s.Message)
                                                   .Include(s => s.Message.User)
                                                   .Where(s => s.Message.Deleted == false)
                                                   .Where(s => s.WasDeleted == false)
                                                   .Where(s => s.DontShowInBin == false)
                                                   .Where(s => s.RecepientId == actualUserId)
                                                   .Select(s => new MessageToList()
                                                   {
                                                       MessageId = s.Message.Id,
                                                       SenderName = s.Message.User.DisplayedName,
                                                       Subject = s.Message.Subject,
                                                       Sent = s.Message.SentTime,
                                                       DeleteLink = "<div style='text-align: center'><a href=\"#\" onClick='DeleteMessage(" + s.Message.Id + ")'><span style='color: rgba(128,0,0,1); text-align: center;'><b><i class='fas fa-trash-alt'></i></b></span></a></div>",
                                                       ReadLink = "<div style='text-align:center'><a href=\"#\" onClick='ReadMessage(" + s.Message.Id + ")'><span style='color: rgba(0,128,0, 1); text-align: center;'><b><i class='fas fa-book-open'></i></b></span></a></div>",
                                                       WasRead = s.WasRead
                                                   })
                                                   .OrderByDescending(s => s.Sent)
                                                   .ToListAsync();

                    if (msg.Count > 0)
                    {
                        jsonList = JsonConvert.SerializeObject(msg);
                    }
                    else
                        jsonList = "";

                    await Clients.Caller.SendAsync("receivedMailboxData", jsonList);
                    await RefreshNumbers();
                    break;

                case "compose":

                    jsonList = "";
                    await Clients.Caller.SendAsync("receivedMailboxData", jsonList);
                    await RefreshNumbers();
                    break;

                case "deleted":

                    List<MessageToList> deletedMessage = await _dbContext.MessageRecipients
                                                   .Include(s => s.Message)
                                                   .Include(s => s.Message.User)
                                                   .Where(s => s.Message.Deleted == false)
                                                   .Where(s => s.WasDeleted == true)
                                                   .Where(s => s.DontShowInBin == false)
                                                   .Select(s => new MessageToList()
                                                   {
                                                       MessageId = s.Message.Id,
                                                       SenderName = s.Message.User.DisplayedName,
                                                       Subject = s.Message.Subject,
                                                       Sent = s.Message.SentTime,
                                                       DeleteLink = "<div style='text-align: center'><a href=\"#\" onClick='WipeMessage(" + s.Message.MessageRecipients
                                                               .Where(r => r.MessageId == +s.MessageId)
                                                               .Where(r => r.User.Id == actualUserId)
                                                               .Select(r => r.Id)
                                                               .FirstOrDefault() + ");'><span style='color: rgba(128,0,0,1); text-align: center;'><b><i class='fas fa-trash-alt'></i></b></span></a></div>",
                                                       ReadLink = "<div style='text-align:center'><a href=\"#\" onClick='ReadMessage(" + s.Message.Id + ")'><span style='color: rgba(0,128,0, 1); text-align: center;'><b><i class='fas fa-book-open'></i></b></span></a></div>",
                                                       RestoreLink = "<div style='text-align: center'><a href=\"#\" onClick='RestoreMessage(" + s.Message.MessageRecipients
                                                               .Where(r => r.MessageId == +s.MessageId)
                                                               .Where(r => r.User.Id == actualUserId)
                                                               .Select(r => r.Id)
                                                               .FirstOrDefault() + ");'><span style='color: rgba(0,0,196, 1); text-align: center;'><b><i class='fas fa-trash-restore'></i></b></span></a></div>",
                                                       WasRead = s.WasRead
                                                   })
                                                   .OrderByDescending(s => s.Sent)
                                                   .ToListAsync();

                    if (deletedMessage.Count > 0)
                    {
                        jsonList = JsonConvert.SerializeObject(deletedMessage);
                    }
                    else
                        jsonList = "";

                    await Clients.Caller.SendAsync("receivedMailboxData", jsonList);
                    await RefreshNumbers();

                    break;

                case "sent":
                    List<SentMessages> sent = new List<SentMessages>();
                    List<int> sentMessageId = await _dbContext.Messages.Where(s => s.SenderId == actualUserId).Select(s => s.Id).ToListAsync();
                    List<MessageRecipient> recepients = new List<MessageRecipient>();
                    foreach (var id in sentMessageId)
                    {
                        recepients.AddRange(await _dbContext.MessageRecipients.Where(s => s.MessageId == id).Include(s => s.User).Include(s => s.Message).ToListAsync());
                    }
                    foreach (var recipient in recepients)
                    {
                        SentMessages tmp = new SentMessages();
                        tmp.MessageId = recipient.MessageId;
                        tmp.Sent = recipient.Message.SentTime;
                        tmp.Subject = recipient.Message.Subject;
                        tmp.RecepientName = recipient.User.DisplayedName;
                        tmp.ReadLink = "<div style='text-align:center'><a href=\"#\" onClick='ReadMessage(" + recipient.Message.Id + ")'><span style='color: rgba(0,128,0, 1); text-align: center;'><b><i class='fas fa-book-open'></i></b></span></a></div>";
                        tmp.WasRead = recipient.WasRead;
                        sent.Add(tmp);
                    }

                    if (sent.Count > 0)
                    {
                        jsonList = JsonConvert.SerializeObject(sent);
                    }
                    else
                        jsonList = "";

                    await Clients.Caller.SendAsync("receivedMailboxData", jsonList);
                    await RefreshNumbers();

                    break;

                case "settings":
                    jsonList = "";
                    await Clients.Caller.SendAsync("receivedMailboxData", jsonList);
                    await RefreshNumbers();
                    break;

                default:
                    await RefreshNumbers();
                    break;
            }
        }

        public async Task SendAsync(string recepients, string message, string subject)
        {
            senderId = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).Select(s => s.Id).FirstOrDefaultAsync();

            var recepientes = recepients.Split(';');

            if (recepientes == null)
                return;

            if (message == "")
                return;

            if (subject == "") subject = "No subject";

            Message messageSend = new Message();

            messageSend.SenderId = senderId;
            messageSend.MessageText = message;
            messageSend.SentTime = System.DateTime.UtcNow;
            messageSend.Subject = subject;

            await _dbContext.Messages.AddAsync(messageSend);
            await _dbContext.SaveChangesAsync();

            MessageId = messageSend.Id;

            foreach (string recepient in recepientes)
            {
                recepient.Trim();
                if (recepient != "")
                {
                    if (!await _dbContext.Users.Where(s => s.DisplayedName == recepient).AnyAsync())
                    {
                        DeliveryReport deliveryFailed = new DeliveryReport();
                        deliveryFailed.Success = false;
                        deliveryFailed.RecepientName = recepient;
                        deliveryFailed.ErrorMessage = "Author with the name " + recepient + " was not found";
                        deliveryReports.Add(deliveryFailed);
                    }
                    else
                    {
                        RecepientId = await _dbContext.Users.Where(s => s.DisplayedName == recepient).Select(s => s.Id).FirstOrDefaultAsync();
                        MessageRecipient msgRecepient = new MessageRecipient();
                        msgRecepient.MessageId = MessageId;
                        msgRecepient.RecepientId = RecepientId;
                        msgRecepient.WasRead = false;
                        msgRecepient.WasDeleted = false;
                        try
                        {
                            await _dbContext.MessageRecipients.AddAsync(msgRecepient);
                            await _dbContext.SaveChangesAsync();
                            DeliveryReport successfulDelivery = new();
                            successfulDelivery.RecepientName = recepient;
                            successfulDelivery.Success = true;
                            deliveryReports.Add(successfulDelivery);
                        }
                        catch (Exception ex)
                        {
                            DeliveryReport failedDeliver = new();
                            failedDeliver.RecepientName = recepient;
                            failedDeliver.Success = false;
                            failedDeliver.ErrorMessage = ex.Message;
                            failedDeliver.ErrorDetails = ex.StackTrace;
                            deliveryReports.Add(failedDeliver);
                        }
                    }

                    foreach (string temp in UsersOnline)
                        if (temp == RecepientId)
                        {
                            if (RecepientId != "")
                            {
                                MessageToList msg = new MessageToList();
                                msg.MessageId = messageSend.Id;
                                msg.Subject = messageSend.Subject;
                                msg.SenderName = await _dbContext.Users.Where(s => s.Id == messageSend.SenderId).Select(s => s.DisplayedName).FirstOrDefaultAsync();
                                msg.Sent = messageSend.SentTime;

                                string output = JsonConvert.SerializeObject(msg);

                                await Clients.User(RecepientId).SendAsync("NewMessage", output);
                                await RefreshNums(RecepientId);
                            }
                        }
                }
            }
            string jsonDeliveryReport = JsonConvert.SerializeObject(deliveryReports);
            await Clients.Caller.SendAsync("deliveryReport", jsonDeliveryReport);
            await RefreshNumbers();
        }

        public async Task ReadMessage(int id)
        {
            User actual = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).FirstOrDefaultAsync();
            Message message = await _dbContext.Messages.Where(s => s.Id == id).FirstOrDefaultAsync();
            if (await _dbContext.MessageRecipients.Where(s => s.MessageId == id).Where(s => s.RecepientId == actual.Id).AnyAsync())
            {
                int messageReceientId = await _dbContext.MessageRecipients.Where(s => s.MessageId == id).Where(s => s.RecepientId == actual.Id).Select(s => s.Id).FirstOrDefaultAsync();
                MessageToPrint msg = await _dbContext.Messages.Where(s => s.Id == id).Select(s => new MessageToPrint()
                {
                    MessageId = s.Id,
                    MessageRecepientId = messageReceientId,
                    SenderId = s.SenderId,
                    SenderName = s.User.DisplayedName,
                    Subject = s.Subject,
                    Message = s.MessageText,
                    Sent = s.SentTime
                }).FirstOrDefaultAsync();
                jsonMessage = JsonConvert.SerializeObject(msg);
            }

            await Clients.Caller.SendAsync("messageToPrint", jsonMessage);
            await RefreshNumbers();
            MessageRecipient recepient = await _dbContext.MessageRecipients.Where(s => s.MessageId == id).Where(s => s.RecepientId == actual.Id).FirstOrDefaultAsync();
            try
            {
                if (recepient.WasRead == false)
                {
                    recepient.WasRead = true;
                    recepient.ReadTime = DateTime.UtcNow;
                    await _dbContext.SaveChangesAsync();
                    await RefreshNumbers();
                    if (!await _dbContext.MessageRecipients.Where(s => s.RecepientId == actual.Id).Where(s => s.WasRead == false).AnyAsync())
                        await Clients.Caller.SendAsync("hideEnvelope");
                }
                if (UsersOnline.Where(s => s == message.SenderId).Any())
                {
                    await Clients.User(message.SenderId).SendAsync("messageDelivered", recepient.MessageId);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            await RefreshNumbers();
        }

        public async Task DeleteMessage(int id)
        {
            User actual = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).FirstOrDefaultAsync();
            if (await _dbContext.MessageRecipients.Where(s => s.MessageId == id).Where(s => s.RecepientId == actual.Id).AnyAsync())
            {
                MessageRecipient recepie = await _dbContext.MessageRecipients.Where(s => s.MessageId == id).Where(s => s.RecepientId == actual.Id).FirstOrDefaultAsync();
                recepie.DontShowInBin = false;
                recepie.WasDeleted = true;
                await _dbContext.SaveChangesAsync();

                await Clients.Caller.SendAsync("messageDeleted", id);
                await RefreshNumbers();
            }
        }

        public async Task RestoreMessage(int id)
        {
            User ActualUser = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).FirstOrDefaultAsync();
            MessageRecipient recepient = await _dbContext.MessageRecipients.Where(s => s.Id == id).FirstOrDefaultAsync();
            if (recepient.RecepientId == ActualUser.Id)
            {
                recepient.WasDeleted = false;
                await _dbContext.SaveChangesAsync();
                await RefreshNumbers();
                await Clients.Caller.SendAsync("MessageRestored", recepient.MessageId);
            }
        }

        public async Task WipeMessage(int id)
        {
            User ActualUser = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).FirstOrDefaultAsync();
            MessageRecipient recepient = await _dbContext.MessageRecipients.Where(s => s.Id == id).FirstOrDefaultAsync();
            if (recepient.RecepientId == ActualUser.Id)
            {
                recepient.DontShowInBin = true;
                await _dbContext.SaveChangesAsync();
                await Clients.Caller.SendAsync("MessageWipped", recepient.MessageId);
                await RefreshNumbers();
            }
        }

        public async Task RefreshNumbers()
        {
            string actualUserId = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).Select(s => s.Id).FirstOrDefaultAsync();

            int unreadCount = await _dbContext.MessageRecipients.Where(s => s.RecepientId == actualUserId).Where(s => s.WasRead == false).Where(s => s.WasDeleted == false).CountAsync();
            int allCount = await _dbContext.MessageRecipients.Where(s => s.RecepientId == actualUserId).Include(s => s.Message).Where(s => s.WasDeleted == false).CountAsync();
            int deletedCount = await _dbContext.MessageRecipients.Where(s => s.RecepientId == actualUserId).Where(s => s.WasDeleted == true).Where(s => s.DontShowInBin == false).CountAsync();

            await Clients.Caller.SendAsync("fill", unreadCount, allCount, deletedCount);
        }

        public async Task RefreshNums(string actualUserId)
        {
            int unreadCount = await _dbContext.MessageRecipients.Where(s => s.RecepientId == actualUserId).Where(s => s.WasRead == false).Where(s => s.WasDeleted == false).CountAsync();
            int allCount = await _dbContext.MessageRecipients.Where(s => s.RecepientId == actualUserId).Include(s => s.Message).Where(s => s.WasDeleted == false).CountAsync();
            int deletedCount = await _dbContext.MessageRecipients.Where(s => s.RecepientId == actualUserId).Where(s => s.WasDeleted == true).Where(s => s.DontShowInBin == false).CountAsync();

            await Clients.User(actualUserId).SendAsync("fill", unreadCount, allCount, deletedCount);
        }

        public async Task SendInvitation(int clubId, string userId)
        {
            // get Club Name, User Name, Sender Name
            Club selectedClub = await _dbContext.Clubs.Where(s => s.Id == clubId).Include(s => s.Owner).FirstOrDefaultAsync();
            string userName = await _dbContext.Users.Where(s => s.Id == userId).Select(s => s.DisplayedName).FirstOrDefaultAsync();
            string subject = "Invitation to the club " + selectedClub.ClubName;
            string htmlInvitation = "";
            htmlInvitation += "<h3> Invitation to the private club " + selectedClub.ClubName + "</h3>";
            htmlInvitation += "<p style='text-align:center'><b>Hello from Stories </b></p>";
            htmlInvitation += "<Administrator of the club <b>" + selectedClub.ClubName + "</b> sent you an invitation to join the club. If you join the club, you can add your articles there and club members will be able to find them easier.</p>";
            htmlInvitation += "<p><a href='/Clubs/Join?id=" + clubId.ToString() + "'>Just click to join!</a></p>";
            htmlInvitation += "<hr><div style='text-align: center; width: 100%'>Thank you for being with Stories</div>";
            await SendAsync(userName, htmlInvitation, subject);
        }

        public async Task SendFriendRequest(string UserId)
        {
            if (Context.User.Identity.IsAuthenticated)
            {   Friends newFriend = new();
                User ActualUser = await _dbContext.Users.Where(s => s.Email == Context.User.Identity.Name).FirstOrDefaultAsync();
                User Friend = await _dbContext.Users.Where(s => s.Id == UserId).FirstOrDefaultAsync();
                if (Friend != null)
                {
                    if (await _dbContext.Friends.Where(s => s.UserId == ActualUser.Id).Where(s => s.FriendId ==  Friend.Id).AnyAsync())
                    {
                        return;
                    }
                    if (await _dbContext.Friends.Where(s => s.FriendId == ActualUser.Id)
                                                .Where(s => s.UserId == Friend.Id)
                                                .Where(s => s.Confirmed == false)
                                                .Where(s => s.Blocked == false)
                                                .Where(s => s.Declined == false).AnyAsync())
                    {
                        // user requests friendship from someone who already requested his friendship 
                        await ConfirmFriendship(Friend.Id, ActualUser.Id);
                        return;
                    }
                    
                    newFriend.UserId = ActualUser.Id;
                    newFriend.FriendId = Friend.Id;
                    newFriend.RequestTime = DateTime.UtcNow;
                    
                    await _dbContext.Friends.AddAsync(newFriend);
                    await _dbContext.SaveChangesAsync();

                    // we need to send message to the user about new friend request => he shall be able to approve or decline the request
                    string MessageText = "";
                    MessageText += "<h3>Hello " + Friend.DisplayedName + "</h3>";
                    MessageText += "Author <b>" + ActualUser.DisplayedName + "</b> sent you a friendship offer.";
                    MessageText += "<br>";
                    MessageText += "Do you wish to <a href='#' onclick='ConfirmFriendship(\"" + ActualUser.Id + "\", \"" + Friend.Id + "\")'>Confirm friendship</a>";
                    MessageText += "&nbsp;or <a href='#' onclick='DeclineFriendship(\"" + ActualUser.Id + "\")'>Decline friendship</a> ";

                    await SendAsync(Friend.DisplayedName, MessageText, "New Friend request from " + ActualUser.DisplayedName);
                }
            }
            else return;
        }

        public async Task ConfirmFriendship(string user, string friend)
        {
            if (user == null || friend == null)
                return;
            User requestor = await _dbContext.Users.Where(s => s.Id == user).FirstOrDefaultAsync();
            User addedFriend = await _dbContext.Users.Where(s => s.Id == friend).FirstOrDefaultAsync();
            if (requestor == null || addedFriend == null)
            {
                _logger.LogWarning("can't confirm of frienship");
            }
            else
            {
                _logger.LogWarning("Confirming friendship from " + user + " with " + friend);
                // find existing db relationship - confirm it and create opposite one confirmed 
                Friends friendshipFirst = await _dbContext.Friends.Where(s => s.UserId == user)
                                                                      .Where(s => s.FriendId == friend)
                                                                      .Where(s => s.Confirmed == false)
                                                                      .Where(s => s.Declined == false)
                                                                      .Where(s => s.Blocked == false).FirstOrDefaultAsync();
                if (friendshipFirst == null)
                {
                    friendshipFirst = await _dbContext.Friends.Where(s => s.UserId == friend)
                                                                  .Where(s => s.FriendId == user)
                                                                  .Where(s => s.Confirmed == false)
                                                                  .Where(s => s.Declined == false)
                                                                  .Where(s => s.Blocked == false)
                                                                  .FirstOrDefaultAsync();
                    if (friendshipFirst == null)
                    {
                        _logger.LogWarning("Friendship requested was not found even reverted");
                        return;
                    }
                    else
                    {
                        // if we are here user requested friendship someone who already awaits for his approval 
                        // we must aprove original request and create opposite one - set it approved too
                        string tmp = user;
                        user = friend;
                        friend = tmp;
                    }
                }

                     // if we are here we must confirm request and create opposite one already confirmed.
                        
                        friendshipFirst.Confirmed = true;
                        
                        Friends friendshipSecond = new();
                        friendshipSecond.UserId = friend;
                        friendshipSecond.FriendId = user;
                        friendshipSecond.Confirmed = true;
                        friendshipSecond.Blocked = false;
                        friendshipSecond.Declined = false;
                        friendshipSecond.RequestTime = DateTime.UtcNow;

                        await _dbContext.Friends.AddAsync(friendshipSecond);
                        await _dbContext.SaveChangesAsync();

                        if (await _dbContext.Sessions.Where(s => s.UserId == friend).Where(s => s.IsActive == true).AnyAsync())
                {
                    await RefreshNums(friend);
                }
                await RefreshNumbers();

                await Clients.Caller.SendAsync("Refresh");
            }

        }

        public async Task RemoveDeclined(string user, string friend)
        {
            if (user == null || friend == null)
                return;
            User requestor = await _dbContext.Users.Where(s => s.Id == user).FirstOrDefaultAsync();
            User addedFriend = await _dbContext.Users.Where(s => s.Id == friend).FirstOrDefaultAsync();
            if (requestor == null || addedFriend == null)
            {
                _logger.LogWarning("can't confirm of frienship");
            }
            else
            {
                Friends friendshipFirst = await _dbContext.Friends.Where(s => s.UserId == user)
                    .Where(s => s.FriendId == friend)
                    .Where(s => s.Confirmed == false)
                    .Where(s => s.Declined == true)
                    .Where(s => s.Blocked == false).FirstOrDefaultAsync();
                if (friendshipFirst == null)
                {
                    Console.WriteLine("Friendship wasn't found");
                }
                else
                {
                    _dbContext.Friends.Remove(friendshipFirst);
                    await _dbContext.SaveChangesAsync();
                    await Clients.Caller.SendAsync("Refresh");
                }
            }
        }

        public async Task DeclineFriendship(string user, string friend)
        {
            if (user == null || friend == null)
                return;
            User requestor = await _dbContext.Users.Where(s => s.Id == user).FirstOrDefaultAsync();
            User addedFriend = await _dbContext.Users.Where(s => s.Id == friend).FirstOrDefaultAsync();
            if (requestor == null || addedFriend == null)
            {
                _logger.LogWarning("can't confirm of frienship");
            }
            else
            {
                // find existing db relationship - confirm it and create opposite one confirmed 
                Friends friendshipFirst = await _dbContext.Friends.Where(s => s.UserId == user)
                    .Where(s => s.FriendId == friend)
                    .Where(s => s.Confirmed == false)
                    .Where(s => s.Declined == false)
                    .Where(s => s.Blocked == false).FirstOrDefaultAsync();
                if (friendshipFirst == null)
                {
                    Console.WriteLine("Friendship wasn't found");
                }
                else
                {
                    friendshipFirst.Declined = true;
                    await _dbContext.SaveChangesAsync();

                    await Clients.Caller.SendAsync("Refresh");

                    if (await _dbContext.Sessions.Where(s => s.UserId == user).Where(s => s.IsActive).AnyAsync())
                        await Clients.User(user).SendAsync("Refresh");
                }

            }
        }

        public class MessageToList
        {
            public int MessageId { get; set; }
            public string Subject { get; set; }
            public string Message { get; set; }
            public string SenderName { get; set; }
            public DateTime Sent { get; set; }
            public string DeleteLink { get; set; } = "";
            public string ReadLink { get; set; }
            public bool WasRead { get; set; }
            public string RestoreLink { get; set; } // only for deleted message
        }

        public class SentMessages
        {
            public int MessageId { get; set; }
            public string Subject { get; set; }
            public string RecepientName { get; set; }
            public DateTime Sent { get; set; }
            public string ReadLink { get; set; }
            public bool WasRead { get; set; }
        }

        public class Recepient
        {
            public string UserId { get; set; }
            public string DisplayedName { get; set; }
            public bool WasRead { get; set; }
        }

        public class MessageToPrint
        {
            public int MessageId { get; set; }
            public int MessageRecepientId { get; set; }
            public string SenderId { get; set; }
            public string SenderName { get; set; }
            public string Subject { get; set; }
            public string Message { get; set; }
            public DateTime Sent { get; set; }
        }

        public class DeliveryReport
        {
            public string RecepientName { get; set; }
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public string ErrorDetails { get; set; }
        }
    }
}