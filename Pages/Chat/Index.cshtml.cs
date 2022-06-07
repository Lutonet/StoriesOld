using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stories.Data;
using Stories.Model;
using Stories.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Stories.Pages.Chat
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private const string V = "OK";
        private ApplicationDbContext _dbContext;
        private ILogger<IndexModel> _logger;
        private UserManager<User> _userManager;
        public User user { get; set; }
        public UserChatSettings UserChatSettings { get; set; }
        public ICollection<ChatRooms> ChatRooms { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public string UserId { get; set; }

            [Required]
            public bool PrivateMessages { get; set; }

            [Required]
            public bool PublicMessages { get; set; }

            [Required]
            public bool SystemMessages { get; set; }

            [Required]
            public string MessageTextColor { get; set; }

            [Required]
            public string NameTextColor { get; set; }

            [Required]
            public string BackgroundColor { get; set; }
        }

        public IndexModel(ApplicationDbContext dbContext, ILogger<IndexModel> logger, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<ActionResult> OnGetAsync()
        {
            user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            // check if settings exist
            if (!_dbContext.UserChatSettings.Where(x => x.UserId == user.Id).Any())
            {
                
                // We need to add default data -> NameColor, BackgroundColor, TextColor, MessagesBeep, PrivateChatBeep
                ColorSet colors = GenerateSet.GenerateColorSet();
                UserChatSettings chatSettings = new UserChatSettings()
                {
                    User = user,
                    ChatBackgroundColor = colors.BackgroundColor,
                    ChatFontColor = colors.TextColor,
                    NameFontColor = colors.NameColor,
                    MessageNotification = true,
                    PrivateMessageNotification = true
                };

                try
                {
                    await _dbContext.UserChatSettings.AddAsync(chatSettings);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation("Chat settings saved for user " + user.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Can't save Chat settings for user " + user.Email, ex);
                    return RedirectToPage("/");
                }
            }
            if (!_dbContext.ChatRooms.Any())
                try
                {
                    ChatRooms chatRoom = new ChatRooms
                    {
                        RoomName = Settings.DefaultChatroom,
                        IsActive = true,
                        Created = DateTime.UtcNow,
                        RoomType = "public"
                    };
                    await _dbContext.ChatRooms.AddAsync(chatRoom);
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                
                    _logger.LogError("Can't create default chat room", ex);
                }
            try
            {
                UserChatSettings = await _dbContext.UserChatSettings.Where(j => j.User == user).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Can't load chat settings for user " + user.Email, ex);
                return Page();
            }
            // settings loaded let load rooms
            ChatRooms = await _dbContext.ChatRooms.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            try
            {
                UserChatSettings = await _dbContext.UserChatSettings.Where(j => j.User == user).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Can't load chat settings for user " + user.Email, ex);
                return Page();
            }
            // settings loaded let load rooms
            ChatRooms = await _dbContext.ChatRooms.ToListAsync();
           
            if (ModelState.IsValid)
            {
                if (user.Id == Input.UserId)
                {
                    var recordToUpdate = await _dbContext.UserChatSettings.Where(s => s.UserId == Input.UserId).FirstOrDefaultAsync();
                    recordToUpdate.SystemMessageNotification = Input.SystemMessages;
                    recordToUpdate.PrivateMessageNotification = Input.PrivateMessages;
                    recordToUpdate.MessageNotification = Input.PublicMessages;
                    recordToUpdate.NameFontColor = Input.NameTextColor;
                    recordToUpdate.ChatBackgroundColor = Input.BackgroundColor;
                    recordToUpdate.ChatFontColor = Input.MessageTextColor;

                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    _logger.LogCritical("Chat settings hacking attempt by User: " + user.Id + " to user" + Input.UserId);
                }
            }
            return Page();
        }
    }
}