using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CriminalManagementSystem.Models
{
    public class InmateBooking
    {
        [Key]
        public int BookingID { get; set; }
        public int CriminalID { get; set; }
        public int FacilityID { get; set; }
        [Required]
        [Display(Name = "Booking Number")]
        public string BookingNumber { get; set; }
        [Required]
        [Display(Name = "Booking Date")]
        [DataType(DataType.DateTime)]
        public DateTime BookingDate { get; set; }
        [Display(Name = "Released Date")]
        [DataType(DataType.DateTime)]
        public DateTime? ReleasedDate { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; } = "In Custody";
        [Display(Name = "Cell Number")]
        public string CellNumber { get; set; }
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
        public virtual Facility Facility { get; set; }
        public virtual List<InmateMedicalRecord> InmateMedicalRecords { get; set; }
    }
}