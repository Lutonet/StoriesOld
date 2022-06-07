using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Stories.Model
{
    public class Article
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(256, ErrorMessage = "Maximum 256 characters")]
        public string Title { get; set; }

        public string Prolog { get; set; } = "";

        [Required]
        public string Body { get; set; }

        public string Epilog { get; set; } = "";
        public string UserId { get; set; }
        public User User { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0: dd-MM-yyyy HH:mm}")]
        [DisplayName("Article added at: ")]
        public DateTime ArticleAdded { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0: dd-MM-yyyy HH:mm}")]
        [DisplayName("Article published at:")]
        public DateTime ArticlePublished { get; set; }

        public bool IsPublished { get; set; } = false;
        public bool IsBanned { get; set; } = true;
        public int? BannedBy { get; set; } = 0;
        public int AgeRestrictionId { get; set; }
        public AgeRestriction AgeRestriction { get; set; }
        public int? RecommendedAgeGroupId { get; set; }
        public RecommendedAgeGroup RecommendedAgeGroup { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0: dd-MM-yyyy HH:mm}")]
        [DisplayName("Last Change:")]
        public DateTime LastChange { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0: dd-MM-yyyy HH:mm}")]
        [DisplayName("First Published: ")]
        public DateTime FirstPublished { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0: dd-MM-yyyy HH:mm}")]
        [DisplayName("Banned at")]
        public DateTime BannedAt { get; set; }

        public ICollection<Article_Category> Article_Categories { get; set; }
        public ICollection<Article_Collection> Article_Collections { get; set; }
        public ICollection<Critic> Critics { get; set; }
        public ICollection<Club_Article> Club_Articles { get; set; }
        public ICollection<Article_Read> Article_readers { get; set; }
        public ICollection<Stars> Stars { get; set; }
        public ICollection<Like> Likes { get; set; }
    }
}