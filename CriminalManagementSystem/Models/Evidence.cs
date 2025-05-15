using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CriminalManagementSystem.Models
{
    [Table("Evidence")]
    public class Evidence
    {
        [Key]
        public int EvidenceID { get; set; }
        [Required]
        [Display(Name = "Evidence Number")]
        public string EvidenceNumber { get; set; }
        [Required]
        [Display(Name = "Evidence Type")]
        public int EvidenceTypeID { get; set; }
        [Required]
        [Display(Name = "Case")]
        public int CaseID { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Collection Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime CollectionDate { get; set; } = DateTime.Now;
        [Required]
        [Display(Name = "Collected By")]
        public int CollectedBy { get; set; }
        [Required]
        [Display(Name = "Storage Location")]
        public string StorageLocation { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; } = "In Storage";
        [Display(Name = "Disposition Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime? DispositionDate { get; set; }
        [Display(Name = "Disposition Method")]
        public string DispositionMethod { get; set; }
        [Display(Name = "Notes")]
        public string Notes { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public virtual EvidenceType EvidenceType { get; set; }
        public virtual Case Case { get; set; }
        public virtual User CollectedByUser { get; set; }
        public virtual ICollection<EvidenceChainOfCustody> ChainOfCustody { get; set; } = new HashSet<EvidenceChainOfCustody>();
    }
    public class EvidenceType
    {
        [Key]
        public int EvidenceTypeID { get; set; }
        [Required]
        [Display(Name = "Type Name")]
        public string TypeName { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        public virtual ICollection<Evidence> Evidences { get; set; } = new HashSet<Evidence>();
    }
    [Table("EvidenceChainOfCustody")]
    public class EvidenceChainOfCustody
    {
        [Key]
        public int ChainID { get; set; }
        public int EvidenceID { get; set; }
        [Required]
        [Display(Name = "Custody Date")]
        [DataType(DataType.DateTime)]
        public DateTime CustodyDate { get; set; } = DateTime.Now;
        [Required]
        [Display(Name = "Released By")]
        [ForeignKey("ReleasedByUser")]
        public int ReleasedBy { get; set; }
        [Required]
        [Display(Name = "Received By")]
        [ForeignKey("ReceivedByUser")]
        public int ReceivedBy { get; set; }
        [Required]
        [Display(Name = "Transfer Reason")]
        public string TransferReason { get; set; }
        [Display(Name = "Notes")]
        public string Notes { get; set; }
        public virtual Evidence Evidence { get; set; }
        public virtual User ReleasedByUser { get; set; }
        public virtual User ReceivedByUser { get; set; }
    }
}