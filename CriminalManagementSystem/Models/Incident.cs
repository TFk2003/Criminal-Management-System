using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CriminalManagementSystem.Models
{
    public class Incident
    {
        [Key]
        public int IncidentID { get; set; }
        [Required]
        [Display(Name = "Incident Number")]
        public string IncidentNumber { get; set; }
        [Required]
        [Display(Name = "Incident Type")]
        public string IncidentType { get; set; }
        [Required]
        [Display(Name = "Incident Date")]
        [DataType(DataType.DateTime)]
        public DateTime IncidentDate { get; set; }
        [Required]
        [Display(Name = "Location")]
        public string Location { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
        [Display(Name = "Reported By")]
        public string ReportedBy { get; set; }
        [Display(Name = "Report Date")]
        public DateTime ReportedDate { get; set; } = DateTime.Now;
        [ForeignKey("AssignedOfficer")]
        [Display(Name = "Assigned Officer")]
        public int? AssignedOfficerID { get; set; }
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
        public virtual User AssignedOfficer { get; set; }
        public virtual ICollection<IncidentCase> IncidentCases { get; set; } = new HashSet<IncidentCase>();
    }
}