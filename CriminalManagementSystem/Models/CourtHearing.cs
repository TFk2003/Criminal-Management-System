using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CriminalManagementSystem.Models
{
    public class CourtHearing
    {
        [Key]
        public int HearingID { get; set; }
        public int CaseID { get; set; }
        public int CourtID { get; set; }
        [Required]
        [Display(Name = "Hearing Date")]
        [DataType(DataType.DateTime)]
        public DateTime HearingDate { get; set; }
        [Required]
        [Display(Name = "Hearing Type")]
        public string HearingType { get; set; }
        [Display(Name = "Room Number")]
        public string RoomNumber { get; set; }
        [Display(Name = "Outcome")]
        public string Outcome { get; set; }
        [Display(Name = "Next Hearing Date")]
        [DataType(DataType.DateTime)]
        public DateTime? NextHearingDate { get; set; }
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
        public virtual Case Case { get; set; }
        public virtual Court Court { get; set; }
    }
}