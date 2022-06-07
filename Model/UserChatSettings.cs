using System.ComponentModel.DataAnnotations.Schema;

namespace Stories.Model
{
    public class UserChatSettings
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
        [Column(TypeName = "varchar(64)")]
        public string NameFontColor { get; set; }
        [Column(TypeName = "varchar(64)")]
        public string ChatFontColor { get; set; }
        [Column(TypeName = "varchar(64)")]
        public string ChatBackgroundColor { get; set; }

        public bool MessageNotification { get; set; }
        public bool PrivateMessageNotification { get; set; }
        public bool SystemMessageNotification { get; set; }
    }
}