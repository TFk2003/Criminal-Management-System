using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CriminalManagementSystem.Models
{
    public class InmateMedicalRecord
    {
        [Key]
        public int MedicalRecordID { get; set; }
        public int BookingID { get; set; }
        [Required]
        [Display(Name = "Record Date")]
        [DataType(DataType.DateTime)]
        public DateTime RecordDate { get; set; }
        [Required]
        [Display(Name = "Medical Condition")]
        public string MedicalCondition { get; set; }
        [Display(Name = "Treatment")]
        public string Treatment { get; set; }
        [Display(Name = "Physician")]
        public string Physician { get; set; }
        [Display(Name = "Notes")]
        public string Notes { get; set; }
        public virtual InmateBooking InmateBooking { get; set; }
    }
}