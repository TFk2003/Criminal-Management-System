using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CriminalManagementSystem.Models
{
    public class Court
    {
        [Key]
        public int CourtID { get; set; }
        [Required]
        [Display(Name = "Court Name")]
        public string CourtName { get; set; }
        [Display(Name = "Court Type")]
        public string CourtType { get; set; }
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Judge Name")]
        public string JudgeName { get; set; }
        public virtual ICollection<CourtHearing> CourtHearings { get; set; }
    }
}