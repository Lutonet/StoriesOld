using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stories.Model
{
    public class Article_Read
    {
        public int Id { get; set; }

        public int ArticleId { get; set; }

        public string? UserId { get; set; }

        public bool isAuthenticated { get; set; }

        public string IPAddress { get; set; }

        public DateTime ReadAt { get; set; }

        public Article Article { get; set; }

        public User User { get; set; }
    }
}
