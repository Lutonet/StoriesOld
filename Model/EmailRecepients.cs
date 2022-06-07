using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stories.Model
{
    public class EmailRecepients
    {
        public int Id { get; set; }
        public int EmailLogId { get; set; }
        public EmailLog EmailLog { get; set; }
        public bool Sent { get; set; }
        public DateTime SentAt { get; set; }
        [Column(TypeName = "varchar(1024)")]
        public string Error { get; set; } = "";
    }
}