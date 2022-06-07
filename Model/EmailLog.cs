using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stories.Model
{
    public class EmailLog
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
        public IList<EmailRecepients> EmailRecepients { get; set; }
        [Column(TypeName = "varchar(512)")]
        public string Subject { get; set; }
    }
}