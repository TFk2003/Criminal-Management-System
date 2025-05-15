using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CriminalManagementSystem.Models
{
    public class ProbationOfficer
    {
        [Key]
        public int OfficerID { get; set; }
        public int UserID { get; set; }
        [Required]
        [Display(Name = "Badge Number")]
        public string BadgeNumber { get; set; }
        [Display(Name = "Specialization")]
        public string Specialization { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<ProbationRecord> ProbationRecords { get; set; } = new HashSet<ProbationRecord>();
    }
}