using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Stories.Model
{
    public class Message
    {
        public int Id { get; set; }

        public string SenderId { get; set; }
        public User User { get; set; }

        [Required]
        [MaxLength(256, ErrorMessage = "Maximum 256 characters")]
        public string Subject { get; set; }

        [Required]
        [DisplayName("Message")]
        [MaxLength(4096, ErrorMessage = "Maximum 4096 Characters")]
        public string MessageText { get; set; }

        [DisplayName("Sent at")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0: dd-MM-yyyy}")]
        public DateTime SentTime { get; set; }

        public bool Deleted { get; set; } = false;
        public IList<MessageRecipient> MessageRecipients { get; set; }
        public IList<SpamReports> SpamReports { get; set; }
    }
}