namespace Stories.Model
{
    public class FavoriteAuthor_Users
    {
        public int Id { get; set; }

        public string UserId { get; set; }


        public string AuthorId { get; set; }
        public User Author { get; set; }
        public User User { get; set; }
    }
}