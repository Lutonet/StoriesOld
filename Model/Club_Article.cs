using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stories.Model
{
    public class Club_Article
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public int ClubId { get; set; }
        public Article Article { get; set; }
        public Club Club { get; set; }
    }
}
