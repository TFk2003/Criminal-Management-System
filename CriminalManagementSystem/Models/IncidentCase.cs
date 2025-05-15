using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CriminalManagementSystem.Models
{
    public class IncidentCase
    {
        [Key]
        public int IncidentCaseID { get; set; }
        public int IncidentID { get; set; }
        public int CaseID { get; set; }
        public virtual Incident Incident { get; set; }
        public virtual Case Case { get; set; }
    }
}