using System;

namespace Stories.Model
{
    public class Stars
    {
        public int Id { get; set; }

        public int ArticleId { get; set; }

        public Article Article { get; set; }


        public string UserId { get; set; }

        public User User { get; set; }

        public int StarsCount { get; set; }

        public DateTime Change { get; set; }
    }
}
