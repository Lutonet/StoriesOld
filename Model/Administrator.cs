using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Stories.Model
{
    public class Administrator
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int? ActivatedBy { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0: dd-MM-yyyy HH:mm}")]
        [DisplayName("Activated Time:")]
        public DateTime ActivatedAt { get; set; }

        [StringLength(128)]
        [DisplayName("Reason for deactivation:")]
        [MaxLength(256, ErrorMessage = "Maximum 256 characters")]
        public string DeactivationReason { get; set; } = "";

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0: dd-MM-yyyy HH:mm}")]
        [DisplayName("Deactivation Time:")]
        public DateTime DeactivatedAt { get; set; }

        public int? DeactivatedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public User User { get; set; }
        public IList<SpamReports> SpamReports { get; set; }
        public IList<User> Users { get; set; }
    }
}