using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CriminalManagementSystem.Models
{
    public class CaseVictim
    {
        [Key]
        public int CaseVictimID { get; set; }
        public int CaseID { get; set; }
        public int VictimID { get; set; }
        [Display(Name = "Statement")]
        public string Statement { get; set; }
        [Display(Name = "Impact Statement")]
        public string ImpactStatement { get; set; }
        [Display(Name = "Contacted")]
        public bool IsContacted { get; set; } = false;
        public virtual Case Case { get; set; }
        public virtual Victim Victim { get; set; }
    }
}