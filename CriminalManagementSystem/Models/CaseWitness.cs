using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CriminalManagementSystem.Models
{
    public class CaseWitness
    {
        [Key]
        public int CaseWitnessID { get; set; }
        public int CaseID { get; set; }
        public int WitnessID { get; set; }
        [Display(Name = "Statement")]
        public string Statement { get; set; }
        [Display(Name = "Statement Date")]
        [DataType(DataType.DateTime)]
        public DateTime StatementDate { get; set; }
        [Display(Name = "Testifying")]
        public bool IsTestifying { get; set; } = false;
        public virtual Case Case { get; set; }
        public virtual Witness Witness { get; set; }
    }
}