using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CriminalManagementSystem.Models
{
    public class DocumentType
    {
        [Key]
        public int DocumentTypeID { get; set; }
        [Required]
        [Display(Name = "Type Name")]
        public string TypeName { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        public virtual ICollection<Document> Documents { get; set; } = new HashSet<Document>();
    }
}