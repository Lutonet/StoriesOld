using System;

namespace Stories.Model
{
    public class ChattingTime
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan TimeInChatSuma { get; set; }
    }
}