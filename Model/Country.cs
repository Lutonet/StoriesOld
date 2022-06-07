using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Stories.Model
{
    public class Country
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(256, ErrorMessage = "Maximum 256 characters")]
        public string CountryName { get; set; }
        [MaxLength(512)]
        public string FlagFilePath { get; set; } = "";

        [Required]
        public int PhonePrefix { get; set; }

        public int Timezone { get; set; }
        public ICollection<User> Users { get; set; }
    }
}