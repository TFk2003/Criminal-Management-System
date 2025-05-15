using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CriminalManagementSystem.Models
{
    public class Arrest
    {
        [Key]
        public int ArrestID { get; set; }
        public int CriminalID { get; set; }
        public int ArrestingOfficerID { get; set; }
        [Required]
        [Display(Name = "Arrest Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        [DataType(DataType.Date)]
        public DateTime ArrestDate { get; set; }
        [Required]
        [Display(Name = "Arrest Location")]
        public string ArrestLocation { get; set; }
        [Display(Name = "Description")]
        public string ArrestDescription { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending";
        [Display(Name = "Created By")]
        public int CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "Modified By")]
        public int? ModifiedBy { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }
        public virtual Criminal Criminal { get; set; }
        public virtual User ArrestingOfficer { get; set; }
        public virtual ICollection<ArrestCharge> ArrestCharges { get; set; } = new HashSet<ArrestCharge>();
    }
    public class Charge
    {
        [Key]
        public int ChargeID { get; set; }
        [Required]
        [Display(Name = "Charge Code")]
        public string ChargeCode { get; set; }
        [Required]
        [Display(Name = "Charge Name")]
        public string ChargeName { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Severity Level")]
        public string SeverityLevel { get; set; }
        [Display(Name = "Statute")]
        public string Statute { get; set; }
        public virtual ICollection<ArrestCharge> ArrestCharges { get; set; } = new HashSet<ArrestCharge>();
    }
    public class ArrestCharge
    {
        [Key]
        public int ArrestChargeID { get; set; }
        [ForeignKey("Arrest")]
        [Required(ErrorMessage = "Arrest reference is required")]
        public int ArrestID { get; set; }
        [ForeignKey("Charge")]
        [Required(ErrorMessage = "Charge reference is required")]

        public int ChargeID { get; set; }
        [Range(0, 1000000, ErrorMessage = "Bail amount must be between 0 and 1,000,000")]

        [Display(Name = "Bail Amount")]
        public decimal? BailAmount { get; set; }
        [Display(Name = "Bail Status")]
        [StringLength(50)]
        public string BailStatus { get; set; }
        public virtual Arrest Arrest { get; set; }
        public virtual Charge Charge { get; set; }
    }
}