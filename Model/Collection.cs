using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Stories.Model
{
    public class Collection
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Collection")]
        [MaxLength(256, ErrorMessage = "Maximum 256 characters")]
        public string CollectionName { get; set; }

        [DisplayName("Collection description")]
        [MaxLength(4096)]
        public string CollectionDescription { get; set; } = "";

        public string UserId { get; set; }
        public User User { get; set; }
        public ICollection<Article_Collection> Article_Collections { get; set; }
        public ICollection<Collection_Categories> Collection_Categories { get; set; }
    }
}