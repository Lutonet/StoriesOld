using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Stories.Model
{
    public class CategoryGroup
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(128, ErrorMessage = "Maximum 128 Characters")]
        [DisplayName("Category group name")]
        public string GroupName { get; set; }

        public ICollection<Category> Categories { get; set; }
    }
}