using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Stories.Model
{
    public class Friends
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string FriendId { get; set; }
        public bool Confirmed { get; set; } = false;
        public bool Declined { get; set; } = false;
        public bool Blocked { get; set; } = false;

        [DisplayName("Friendship request sent")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0: dd-MM-yyyy}")]
        public DateTime RequestTime { get; set; }

        [DisplayName("Friendship request resolved")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0: dd-MM-yyyy}")]
        public DateTime AnsweringAction { get; set; }

        public User User { get; set; }
        public User Friend { get; set; }
    }
}