using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Stories.Model
{
    public class Critic
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public Article Article { get; set; }

        [DisplayName("Critic Added")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0: dd-MM-yyyy}")]
        public DateTime CriticAdded { get; set; }

        public string CriticId { get; set; }
        public User User { get; set; }


        [Required]
        [MaxLength(2560, ErrorMessage = "Maximum 2560 characters")]
        public string CriticMessage { get; set; }
        public bool Deleted { get; set; }
    }
}