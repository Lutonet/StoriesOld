using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static Stories.Tools.Settings;

namespace Stories.Model
{
    public class User : IdentityUser
    {
        [DisplayName("Public Name")]
        [MaxLength(256, ErrorMessage = "Max Length 256 Characters")]
        public string DisplayedName { get; set; }

        [DisplayName("First Name")]
        [MaxLength(64, ErrorMessage = "Max Length 64 Characters")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [MaxLength(64, ErrorMessage = "Max Length 64 Characters")]
        public string LastName { get; set; }

        [DisplayName("Registration Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0: dd-MM-yyyy}")]
        public DateTime RegistrationTime { get; set; }

        [DisplayName("Birth Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime BirthDate { get; set; }

        [DisplayName("Last Seen")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0: dd-MM-yyyy}")]
        public DateTime LastSeen { get; set; }

        public int? CountryId { get; set; }
        public Country Country { get; set; }
        public int? DeactivatedByAdminId { get; set; }
        public Administrator Administrator { get; set; }

        [DisplayName("Deactivated Since")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0: dd-MM-yyyy}")]
        public DateTime DeactivatedSince { get; set; }

        [DisplayName("Deactivated Until")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0: dd-MM-yyyy}")]
        public DateTime DeactivatedUntil { get; set; }

        [MaxLength(256, ErrorMessage = "Max Length 256 Characters")]
        public string Reason { get; set; } = "";

        public Gender Gender { get; set; } = Gender.Other;

        [MaxLength(10000, ErrorMessage = "Max Length 10000 Characters")]
        public string Info { get; set; }

        [MaxLength(256, ErrorMessage = "Max Length 256 Characters")]
        public string Facebook { get; set; }

        [MaxLength(256, ErrorMessage = "Max Length 256 Characters")]
        public string Twitter { get; set; }

        [MaxLength(256, ErrorMessage = "Max Length 256 Characters")]
        public string Microsoft { get; set; }

        [MaxLength(256, ErrorMessage = "Max Length 256 Characters")]
        public string Google { get; set; }
        public int? ThemeId { get; set; }
        public Theme Theme { get; set; }

        public ICollection<Article> Articles { get; set; }
        public ICollection<User> UserSender { get; set; }
        public ICollection<User> UserRecepient { get; set; }
        public ICollection<Collection> Collections { get; set; }
        public ICollection<Chat> Chats { get; set; }
        public ICollection<Chat> Chatting { get; set; }
        public ICollection<Critic> Critics { get; set; }
        public ICollection<FavoriteAuthor_Users> Authors { get; set; }
        public ICollection<FavoriteAuthor_Users> Users { get; set; }
        public ICollection<Friends> Friends { get; set; }
        public ICollection<Friends> UserFriends { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<MessageRecipient> MessageRecipients { get; set; }
        public ICollection<SpamReports> SpamReports { get; set; }
        public ICollection<Session> Sessions { get; set; }
        public ICollection<EmailLog> EmailLogs { get; set; }
        public UserSettings UserSettings { get; set; }
        public UserChatSettings UserChatSettings { get; set; }
        public ICollection<ChatRooms> ChatRooms { get; set; }
        public UserSession UserSessions { get; set; }
        public ICollection<UserInRoom> UsersInRooms { get; set; }
        public ICollection<ChattingTime> ChattingTimes { get; set; }
        public ICollection<ChatBans> UsersBanned { get; set; }
        public ICollection<ChatBans> UsersAdmins { get; set; }
        public ICollection<OnlineTime> OnlineTimes { get; set; }
        public ICollection<Club> Clubs { get; set; }
        public ICollection<Stories.Model.Club_Users> Club_Users { get; set; }
        public ICollection<Article_Read> Article_Readers { get; set; }
        public ICollection<Stories.Model.Stars> Stars { get; set; }
    }
}