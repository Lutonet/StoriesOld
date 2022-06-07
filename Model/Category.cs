using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Stories.Model
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Category")]
        [MaxLength(256, ErrorMessage = "Maximum 256 characters")]
        public string CategoryName { get; set; }

        public int CategoryGroupId { get; set; }
        public ICollection<Collection_Categories> Collection_Categories { get; set; }
        public ICollection<Article_Category> Article_Categories { get; set; }
        public CategoryGroup CategoryGroup { get; set; }
    }
}