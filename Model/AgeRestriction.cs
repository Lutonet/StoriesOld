using System.Collections.Generic;

namespace Stories.Model
{
    public class AgeRestriction
    {
        public int Id { get; set; }
        public int AgeFrom { get; set; }
        public ICollection<Article> Articles { get; set; }
    }
}