using System;
using System.ComponentModel.DataAnnotations;

namespace Stories.Model
{
    public class Log
    {
        [Key]
        public int Id { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0: dd/MM/yyyy HH:mm}")]
        [Required]
        public DateTime Logged { get; set; }

        [Required]

        public string MachineName { get; set; }

        [Required]

        public string Level { get; set; }

        [Required]

        public string Message { get; set; }

        public string Logger { get; set; }

        public string Callsite { get; set; }
        public string Exception { get; set; }
    }
}