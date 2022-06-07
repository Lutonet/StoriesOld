namespace Stories.Model
{
    public class UserSession
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
        public int ActiveSessionsCount { get; set; } = 0;
    }
}