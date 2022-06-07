using System;
using System.ComponentModel.DataAnnotations;

namespace Stories.Model
{
    public class Chat
    {
        public int Id { get; set; }

        public string SenderId { get; set; }
        [MaxLength(64)]
        [Required()]
        public string ChatroomName { get; set; } = "allRoomsInTheSystem";
        public User Sender { get; set; }

        public string RecepientId { get; set; } = "everyone";
        [MaxLength(4000)]
        public string Message { get; set; }
        public bool IsAdminMessage { get; set; } = false;
        public User Recepient { get; set; }
        public DateTime SentAt { get; set; }
    }
}