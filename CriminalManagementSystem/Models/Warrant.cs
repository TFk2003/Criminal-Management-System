using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CriminalManagementSystem.Models
{
    public class Warrant
    {
        [Key]
        public int WarrantID { get; set; }
        [Required]
        [Display(Name = "Criminal")]
        public int CriminalID { get; set; }
        [Required]
        [Display(Name = "Warrant Number")]
        public string WarrantNumber { get; set; }
        [Required]
        [Display(Name = "Warrant Type")]
        public string WarrantType { get; set; }
        [Required]
        [Display(Name = "Issue Date")]
        [DataType(DataType.DateTime)]
        public DateTime IssueDate { get; set; }
        [Display(Name = "Expiration Date")]
        [DataType(DataType.DateTime)]
        public DateTime? ExpirationDate { get; set; }
        [Display(Name = "Issuing Judge")]
        public string IssuingJudge { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active";
        [Display(Name = "Description")]
        public string Description { get; set; }
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
    }
}