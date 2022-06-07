namespace Stories.Model
{
    public class Collection_Categories
    {
        public int Id { get; set; }
        public int CollectionId { get; set; }
        public int CategoryId { get; set; }
        public Collection Collection { get; set; }
        public Category Category { get; set; }
    }
}