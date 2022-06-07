using System;

namespace Stories.Model
{
    public class MessageRecipient
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public Message Message { get; set; }

        public string RecepientId { get; set; } = "";
        public User User { get; set; }
        public bool WasRead { get; set; } = false;
        public DateTime ReadTime { get; set; }
        public bool WasDeleted { get; set; } = false;
        public bool DontShowInBin { get; set; } = false;
    }
}