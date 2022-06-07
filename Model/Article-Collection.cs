namespace Stories.Model
{
    public class Article_Collection
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public int CollectionId { get; set; }
        public int OrderInCollection { get; set; }
        public Article Article { get; set; }
        public Collection Collection { get; set; }
    }
}