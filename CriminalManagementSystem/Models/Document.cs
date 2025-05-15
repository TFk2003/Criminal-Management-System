using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace CriminalManagementSystem.Models
{
    public class Document
    {
        [Key]
        public int DocumentID { get; set; }
        public int DocumentTypeID { get; set; }
        [Display(Name = "Case")]
        public int? CaseID { get; set; }
        [Display(Name = "Criminal")]
        public int? CriminalID { get; set; }
        [Display(Name = "Incident")]
        public int? IncidentID { get; set; }
        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }
        [Display(Name ="Description")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "File Path")]
        public string FilePath { get; set; }
        [Display(Name = "Upload Date")]
        public DateTime UploadDate { get; set; } = DateTime.Now;
        [ForeignKey("UploadedByUser")]
        [Display(Name = "Uploaded By")]
        [Column("UploadBy")]
        public int UploadedBy { get; set; }
        public virtual DocumentType DocumentType { get; set; }
        public virtual Case Case { get; set; }
        public virtual Criminal Criminal { get; set; }
        public virtual Incident Incident { get; set; }
        public virtual User UploadedByUser { get; set; }

    }
}