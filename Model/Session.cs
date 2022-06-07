using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stories.Model
{
    public class Session
    {
        public int Id { get; set; }
        [Column(TypeName = "varchar(64)")]
        public string ConnectionId { get; set; }


        public string? UserId { get; set; }
        public User User { get; set; }
        [Column(TypeName = "varchar(16)")]
        public string IpAddress { get; set; }
        [Column(TypeName = "varchar(1024)")]
        public string ClientAgent { get; set; }
        public DateTime Connected { get; set; }
        public DateTime Disconnected { get; set; }
        public bool IsActive { get; set; }
    }
}