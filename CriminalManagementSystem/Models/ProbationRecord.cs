using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CriminalManagementSystem.Models
{
    public class ProbationRecord
    {
        [Key]
        public int ProbationID { get; set; }
        public int CriminalID { get; set; }
        public int OfficerID { get; set; }
        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }
        [Required]
        [Display(Name = "Terms")]
        public string Terms { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active";
        [Display(Name = "Voilations")]
        public int Voilations { get; set; } = 0;
        [Display(Name = "Notes")]
        public string Notes { get; set; }
        [Display(Name = "Created By")]

        public int CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "Modified By")]
        public int? ModifiedBy { get; set; }

        [Display(Name = "Modified Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? ModifiedDate { get; set; }
        public virtual Criminal Criminal { get; set; }
        public virtual ProbationOfficer ProbationOfficer { get; set; }
    }
}