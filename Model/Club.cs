using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Stories.Model
{
    public class Club
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        [Display(Name = "Name of the Club")]
        public string ClubName { get; set; }

        [Required]
        [MaxLength(2560)]
        [Display(Name = "What is this club about?")]
        public string ClubDescription { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
        public DateTime ClubCreated { get; set; }


        public string OwnerId { get; set; }

        public bool isPublic { get; set; }

        public bool isActive { get; set; }

        public User Owner { get; set; }

        public List<Club_Article> Club_Articles { get; set; }

        public List<Club_Users> Club_Users { get; set; }
    }
}
