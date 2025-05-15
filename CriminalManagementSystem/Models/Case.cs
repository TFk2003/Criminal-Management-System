using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CriminalManagementSystem.Models
{
    public class Case
    {
        [Key]
        public int CaseID { get; set; }
        [Required]
        [Display(Name = "Case Number")]
        [RegularExpression(@"^CS\d{6}$", ErrorMessage = "Case number must be in format CS000000")]
        public string CaseNumber { get; set; }
        [Required]
        [Display(Name = "Case Title")]
        public string CaseTitle { get; set; }
        [Display(Name = "Description")]
        public string CaseDescription { get; set; }
        [Display(Name = "Case Type")]
        public string CaseType { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; } = "Open";
        [Display(Name = "Priority")]
        public string Priority { get; set; }
        [Display(Name = "Opening Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime OpeningDate { get; set; } = DateTime.Now;
        [Display(Name = "Closing Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ClosingDate { get; set; }
        [Display(Name = "Assigned Officer")]
        public int? AssignedOfficerID { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual User AssignedOfficer { get; set; }
        public virtual ICollection<CaseCriminal> CaseCriminals { get; set; } = new HashSet<CaseCriminal>();
        public virtual ICollection<CaseWitness> CaseWitnesses { get; set; } = new HashSet<CaseWitness>();
        public virtual ICollection<CaseVictim> CaseVictims { get; set; } = new HashSet<CaseVictim>();
        public virtual ICollection<Evidence> Evidence { get; set; } = new HashSet<Evidence>();
        public virtual ICollection<CourtHearing> CourtHearings { get; set; } = new HashSet<CourtHearing>();
        public virtual ICollection<Document> Documents { get; set; } = new HashSet<Document>();
    }
    public class CaseCriminal
    {
        [Key]
        public int CaseCriminalID { get; set; }
        public int CaseID { get; set; }
        public int CriminalID { get; set; }
        [Display(Name = "Role in Case")]
        public string RoleInCase { get; set; }
        public virtual Case Case { get; set; }
        public virtual Criminal Criminal { get; set; }
    }
}