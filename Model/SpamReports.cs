using System;

namespace Stories.Model
{
    public class SpamReports
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public Message Message { get; set; }
        public DateTime ReportedAt { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
        public int AdminId { get; set; }
        public Administrator Administrator { get; set; }
        public bool WasSpam { get; set; }
    }
}