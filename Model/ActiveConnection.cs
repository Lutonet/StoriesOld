using System;
using System.ComponentModel.DataAnnotations;

namespace Stories.Model
{
    public class ActiveConnection
    {
        public int Id { get; set; }
        public string ConnectionId { get; set; }
        public string UserId { get; set; }
        [MaxLength(64)]
        public string DisplayedName { get; set; }
        public DateTime ConnectedAt { get; set; }
        public string User { get; set; }
    }
}