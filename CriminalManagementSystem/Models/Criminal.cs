using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CriminalManagementSystem.Models
{
    public class Criminal
    {
        [Key]
        public int CriminalID { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        [Display(Name = "Gender")]
        public string Gender { get; set; }
        [Display(Name = "Race")]
        public string Race { get; set; }
        [Display(Name = "Height (cm)")]
        public decimal? Height { get; set; }
        [Display(Name = "Weight (kg)")]
        public decimal? Weight { get; set; }
        [Display(Name = "Eye Color")]
        public string EyeColor { get; set; }
        [Display(Name = "Hair Color")]
        public string HairColor { get; set; }
        [Display(Name = "Identifying Marks")]
        public string IdentifyingMarks { get; set; }
        [Display(Name = "National ID")]
        public string NationalID { get; set; }
        [Display(Name = "SSN")]
        public string SSN { get; set; }
        [Display(Name = "Last Known Address")]
        public string LastKnownAddress { get; set; }
        [Display(Name = "Photo Path")]
        public string PhotoPath { get; set; }
        [Display(Name = "Fingerprint Path")]
        public string FingerprintPath { get; set; }
        public bool IsActive { get; set; }
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
        [Display(Name = "Aliases")]
        public virtual ICollection<CriminalAlias> Aliases{ get; set; } = new HashSet<CriminalAlias>();
        public virtual ICollection<Arrest> Arrests { get; set; } = new HashSet<Arrest>();
        public virtual ICollection<Warrant> Warrants { get; set; } = new HashSet<Warrant>();
        public string FullName => $"{FirstName} {MiddleName?.Trim()} {LastName}".Replace("  ", " ").Trim();
    }
    [Table("CriminalAliases")]
    public class CriminalAlias
    {
        [Key]
        [Column("AliasesID")]
        public int AliasID { get; set; }
        [Required]
        [Display(Name = "Alias Name")]
        public string AliasName { get; set; }
        public int CriminalID { get; set; }
        public virtual Criminal Criminal { get; set; }
    }
}