using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stories.Model
{
    public class AnonymousOnline
    {
        public int Id { get; set; }
        [Column(TypeName = "varchar(30)")]
        public string ConnectionId { get; set; }
        public string IpAddress { get; set; }
        public string ClientAgent { get; set; }
        public DateTime Connected { get; set; }
        public DateTime Disconnected { get; set; }
        public bool IsActive { get; set; }
    }
}