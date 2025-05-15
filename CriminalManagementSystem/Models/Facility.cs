using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CriminalManagementSystem.Models
{
    public class Facility
    {
        [Key]
        public int FacilityID { get; set; }
        [Required]
        [Display(Name = "Facility Name")]
        public string FacilityName { get; set; }
        [Display(Name = "Facility Type")]
        public string FacilityType { get; set; }
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Capacity")]
        public int? Capacity { get; set; }
        [Display(Name = "Warden")]
        public string Warden { get; set; }
        public virtual ICollection<InmateBooking> InmateBookings { get; set; } = new HashSet<InmateBooking>();
    }
}