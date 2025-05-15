using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace CriminalManagementSystem.Models
{
    public class ApplicationDBContext: DbContext
    {
        public ApplicationDBContext() : base("name=CriminalManagementSystemDB")
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Criminal> Criminals { get; set; }
        public DbSet<CriminalAlias> CriminalAliases { get; set; }
        public DbSet<Arrest> Arrests { get; set; }
        public DbSet<Charge> Charges { get; set; }
        public DbSet<ArrestCharge> ArrestCharges { get; set; }
        public DbSet<Case> Cases { get; set; }
        public DbSet<CaseCriminal> CaseCriminals { get; set; }
        public DbSet<Witness> Witnesses { get; set; }
        public DbSet<CaseWitness> CaseWitnesses { get; set; }
        public DbSet<Victim> Victims { get; set; }
        public DbSet<CaseVictim> CaseVictims { get; set; }
        public DbSet<EvidenceType> EvidenceTypes { get; set; }
        public DbSet<Evidence> Evidence { get; set; }
        public DbSet<EvidenceChainOfCustody> EvidenceChainOfCustody { get; set; }
        public DbSet<Court> Courts { get; set; }
        public DbSet<CourtHearing> CourtHearings { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<InmateBooking> InmateBookings { get; set; }
        public DbSet<InmateMedicalRecord> InmateMedicalRecords { get; set; }
        public DbSet<Warrant> Warrants { get; set; }
        public DbSet<ProbationOfficer> ProbationOfficers { get; set; }
        public DbSet<ProbationRecord> ProbationRecords { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<IncidentCase> IncidentCases { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configure relationships and constraints
            modelBuilder.Entity<CriminalAlias>()
                .HasRequired(a => a.Criminal)
                .WithMany(c => c.Aliases)
                .HasForeignKey(a => a.CriminalID);

            modelBuilder.Entity<Arrest>()
                .HasRequired(a => a.Criminal)
                .WithMany(c => c.Arrests)
                .HasForeignKey(a => a.CriminalID);

            modelBuilder.Entity<Arrest>()
                .HasRequired(a => a.ArrestingOfficer)
                .WithMany()
                .HasForeignKey(a => a.ArrestingOfficerID);

            modelBuilder.Entity<ArrestCharge>()
                .HasRequired(ac => ac.Arrest)
                .WithMany(a => a.ArrestCharges)
                .HasForeignKey(ac => ac.ArrestID);

            modelBuilder.Entity<ArrestCharge>()
                .HasRequired(ac => ac.Charge)
                .WithMany(c => c.ArrestCharges)
                .HasForeignKey(ac => ac.ChargeID);

            modelBuilder.Entity<Case>()
                .HasOptional(c => c.AssignedOfficer)
                .WithMany()
                .HasForeignKey(c => c.AssignedOfficerID);

            modelBuilder.Entity<CaseCriminal>()
                .HasRequired(cc => cc.Case)
                .WithMany(c => c.CaseCriminals)
                .HasForeignKey(cc => cc.CaseID);

            modelBuilder.Entity<CaseCriminal>()
                .HasRequired(cc => cc.Criminal)
                .WithMany()
                .HasForeignKey(cc => cc.CriminalID);

            modelBuilder.Entity<CaseWitness>()
                .HasRequired(cw => cw.Case)
                .WithMany(c => c.CaseWitnesses)
                .HasForeignKey(cw => cw.CaseID);

            modelBuilder.Entity<CaseWitness>()
                .HasRequired(cw => cw.Witness)
                .WithMany(w => w.CaseWitnesses)
                .HasForeignKey(cw => cw.WitnessID);

            modelBuilder.Entity<CaseVictim>()
                .HasRequired(cv => cv.Case)
                .WithMany(c => c.CaseVictims)
                .HasForeignKey(cv => cv.CaseID);

            modelBuilder.Entity<CaseVictim>()
                .HasRequired(cv => cv.Victim)
                .WithMany(v => v.CaseVictims)
                .HasForeignKey(cv => cv.VictimID);

            modelBuilder.Entity<Evidence>()
                .HasRequired(e => e.Case)
                .WithMany(c => c.Evidence)
                .HasForeignKey(e => e.CaseID);

            modelBuilder.Entity<CourtHearing>()
                .HasRequired(ch => ch.Case)
                .WithMany(c => c.CourtHearings)
                .HasForeignKey(ch => ch.CaseID);

            modelBuilder.Entity<CourtHearing>()
                .HasRequired(ch => ch.Court)
                .WithMany(c => c.CourtHearings)
                .HasForeignKey(ch => ch.CourtID);

            modelBuilder.Entity<Document>()
                .HasRequired(d => d.UploadedByUser)
                .WithMany()
                .HasForeignKey(d => d.UploadedBy)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Evidence>()
                .HasRequired(e => e.CollectedByUser)
                .WithMany()
                .HasForeignKey(e => e.CollectedBy);

            modelBuilder.Entity<EvidenceChainOfCustody>()
                .HasRequired(c => c.ReceivedByUser)
                .WithMany()
                .HasForeignKey(c => c.ReceivedBy);

            modelBuilder.Entity<EvidenceChainOfCustody>()
                .HasRequired(c => c.ReleasedByUser)
                .WithMany()
                .HasForeignKey(c => c.ReleasedBy);

            modelBuilder.Entity<InmateBooking>()
                .HasRequired(ib => ib.Criminal)
                .WithMany()
                .HasForeignKey(ib => ib.CriminalID);

            modelBuilder.Entity<InmateBooking>()
                .HasRequired(ib => ib.Facility)
                .WithMany(f => f.InmateBookings)
                .HasForeignKey(ib => ib.FacilityID);

            modelBuilder.Entity<Incident>()
                .HasOptional(i => i.AssignedOfficer)
                .WithMany()
                .HasForeignKey(i => i.AssignedOfficerID);

            modelBuilder.Entity<InmateMedicalRecord>()
                .HasRequired(imr => imr.InmateBooking)
                .WithMany(ib => ib.InmateMedicalRecords)
                .HasForeignKey(imr => imr.BookingID);

            modelBuilder.Entity<ProbationOfficer>()
                .HasRequired(po => po.User)
                .WithMany()
                .HasForeignKey(po => po.UserID);

            modelBuilder.Entity<ProbationRecord>()
                .HasRequired(pr => pr.Criminal)
                .WithMany()
                .HasForeignKey(pr => pr.CriminalID);

            modelBuilder.Entity<ProbationRecord>()
                .HasRequired(pr => pr.ProbationOfficer)
                .WithMany(po => po.ProbationRecords)
                .HasForeignKey(pr => pr.OfficerID);

            modelBuilder.Entity<Document>()
                .HasOptional(d => d.Case)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.CaseID);

            modelBuilder.Entity<Document>()
                .HasOptional(d => d.Criminal)
                .WithMany()
                .HasForeignKey(d => d.CriminalID);

            modelBuilder.Entity<Document>()
                .HasOptional(d => d.Incident)
                .WithMany()
                .HasForeignKey(d => d.IncidentID);

            modelBuilder.Entity<IncidentCase>()
                .HasRequired(ic => ic.Incident)
                .WithMany(i => i.IncidentCases)
                .HasForeignKey(ic => ic.IncidentID);

            modelBuilder.Entity<IncidentCase>()
                .HasRequired(ic => ic.Case)
                .WithMany()
                .HasForeignKey(ic => ic.CaseID);

            base.OnModelCreating(modelBuilder);
        }
    }
}