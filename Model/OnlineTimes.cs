using System;

namespace Stories.Model
{
    public class OnlineTime
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan TimeOnline { get; set; }
        public int Days { get; set; }
    }
}