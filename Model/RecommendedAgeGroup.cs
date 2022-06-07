using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Stories.Model
{
    public class RecommendedAgeGroup
    {
        public int Id { get; set; }
        public int LowestAge { get; set; } = 3;
        public int HighestAge { get; set; } = 100;

        [Required]
        [MaxLength(64, ErrorMessage = "Max Length 64 Characters")]
        [DisplayName("Recommended Age group name")]

        public string AgeGroupName { get; set; }

        public ICollection<Article> Articles { get; set; }
    }
}