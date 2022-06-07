using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stories.Model
{
    public class ChatBans
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User BannedUser { get; set; }

        public string AdminUserId { get; set; }
        public User AdminUser { get; set; }
        [Column(TypeName = "varchar(256)")]
        public string GroupName { get; set; }
        public bool BanForAllChat { get; set; } = false;
        public DateTime BannedUntil { get; set; }
        [MaxLength(512)]
        public string ReasonForBan { get; set; }
    }
}