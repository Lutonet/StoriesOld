using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stories.Model
{
    public class SmsMessage
    {
        public int Id { get; set; }
        [Column(TypeName = "varchar(24)")]
        public string PhoneNumber { get; set; }
        [Column(TypeName = "varchar(512)")]
        public string Message { get; set; }
        [Column(TypeName = "varchar(128)")]
        public string Status { get; set; }
        public DateTime SentTime { get; set; }
    }
}