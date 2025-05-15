using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CriminalManagementSystem.Models;
using Microsoft.AspNet.Identity;

namespace CriminalManagementSystem.Controllers
{
    [Authorize]
    public class EvidenceController : Controller
    {
        private ApplicationDBContext db = new ApplicationDBContext();

        // GET: Evidences
        public ActionResult Index(int? caseId)
        {
            IQueryable<Evidence> evidence = db.Evidence.Include(e => e.Case).Include(e => e.EvidenceType).Include(e => e.CollectedByUser);
            if (caseId.HasValue)
            {
                evidence = evidence.Where(e => e.CaseID == caseId);
                ViewBag.CaseID = caseId;
                ViewBag.CaseNumber = db.Cases.Find(caseId)?.CaseNumber;
            }
            return View(evidence.OrderByDescending(e => e.CollectionDate).ToList());
        }

        // GET: Evidences/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evidence evidence = db.Evidence
                .Include(e => e.EvidenceType)
                .Include(e => e.Case)
                .Include(e => e.CollectedByUser)
                .Include(e => e.ChainOfCustody.Select(c => c.ReceivedByUser))
                .Include(e => e.ChainOfCustody.Select(c => c.ReleasedByUser))
                .FirstOrDefault(e => e.EvidenceID == id);
            if (evidence == null)
            {
                return HttpNotFound();
            }
            return View(evidence);
        }

        // GET: Evidences/Create
        public ActionResult Create(int? caseId)
        {
            ViewBag.EvidenceTypeID = new SelectList(db.EvidenceTypes, "EvidenceTypeID", "TypeName");
            ViewBag.CollectedBy = new SelectList(db.Users.Where(u => u.IsActive), "UserID", "FullName", User.Identity.Name);
            if (caseId.HasValue)
            {
                ViewBag.CaseID = new SelectList(db.Cases.Where(c => c.Status == "Open"), "CaseID", "CaseNumber", caseId);
            }
            else
            {
                ViewBag.CaseID = new SelectList(db.Cases.Where(c => c.Status == "Open"), "CaseID", "CaseNumber");
            }
            var newEvidence = new Evidence
                {
                EvidenceNumber = GenerateEvidenceNumber()
            };
            return View(newEvidence);
        }

        // POST: Evidences/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EvidenceNumber,EvidenceTypeID,CaseID,Description,CollectionDate,CollectedBy,StorageLocation,DispositionDate,DispositionMethod,Status,Notes")] Evidence evidence)
        {


            System.Diagnostics.Debug.WriteLine($"{ModelState.IsValid}");

            if (ModelState.IsValid)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine($"Start");
                    evidence.EvidenceNumber = GenerateEvidenceNumber();
                    System.Diagnostics.Debug.WriteLine($"{evidence.EvidenceTypeID}");
                    evidence.CreatedBy = (int)Session["userID"]; // Add this line
                    evidence.CreatedDate = DateTime.Now;
                    db.Evidence.Add(evidence);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Database error: " + ex.InnerException?.Message);
                }
            }
            else
            {
                var errors = ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .Select(x => new {
                x.Key,
                Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
            });

                foreach (var error in errors)
                {
                    System.Diagnostics.Debug.WriteLine($"Key: {error.Key}, Errors: {string.Join(", ", error.Errors)}");
                }
            }
            ViewBag.CollectedBy = new SelectList(db.Users.Where(u => u.IsActive), "UserID", "FullName", evidence.CollectedBy);
            ViewBag.EvidenceTypeID = new SelectList(db.EvidenceTypes, "EvidenceTypeID", "TypeName", evidence.EvidenceTypeID);
            ViewBag.CaseID = new SelectList(db.Cases.Where(c => c.Status == "Open"), "CaseID", "CaseNumber", evidence.CaseID);
            return View(evidence);
        }

        // GET: Evidences/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evidence evidence = db.Evidence.Find(id);
            if (evidence == null)
            {
                return HttpNotFound();
            }
            ViewBag.CaseID = new SelectList(db.Cases.Where(c => c.Status == "Open"), "CaseID", "CaseNumber", evidence.CaseID);
            ViewBag.EvidenceTypeID = new SelectList(db.EvidenceTypes, "EvidenceTypeID", "TypeName", evidence.EvidenceTypeID);
            ViewBag.CollectedBy = new SelectList(db.Users.Where(u => u.IsActive), "UserID", "FullName", evidence.CollectedBy);
            return View(evidence);
        }

        // POST: Evidences/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EvidenceID,EvidenceNumber,EvidenceTypeID,CaseID,Description,CollectionDate,CollectedBy,StorageLocation,Status,DispositionDate,DispositionMethod,Notes")] Evidence evidence)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingEvidence = db.Evidence.Find(evidence.EvidenceID);
                    if (existingEvidence == null)
                    {
                        return HttpNotFound();
                    }
                    existingEvidence.EvidenceTypeID = evidence.EvidenceTypeID;
                    existingEvidence.CaseID = evidence.CaseID;
                    existingEvidence.Description = evidence.Description;
                    existingEvidence.CollectionDate = evidence.CollectionDate;
                    existingEvidence.CollectedBy = evidence.CollectedBy;
                    existingEvidence.Status = evidence.Status;
                    existingEvidence.Notes = evidence.Notes;
                    existingEvidence.DispositionMethod = evidence.DispositionMethod;
                    existingEvidence.DispositionDate = evidence.DispositionDate;
                    existingEvidence.StorageLocation = evidence.StorageLocation;
                    existingEvidence.ModifiedBy = (int)Session["userID"];
                    existingEvidence.ModifiedDate = DateTime.Now;
                    db.Entry(existingEvidence).State = EntityState.Modified;
                    db.SaveChanges();
                return RedirectToAction("Index", new {caseId = evidence.CaseID});
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Database error: " + ex.InnerException?.Message);
                }
            }
            ViewBag.CaseID = new SelectList(db.Cases.Where(c => c.Status == "Open"), "CaseID", "CaseNumber", evidence.CaseID);
            ViewBag.EvidenceTypeID = new SelectList(db.EvidenceTypes, "EvidenceTypeID", "TypeName", evidence.EvidenceTypeID);
            ViewBag.CollectedBy = new SelectList(db.Users.Where(u => u.IsActive), "UserID", "FullName", evidence.CollectedBy);
            return View(evidence);
        }
        //GET: Evidences/AddChainOfCustody/5
        public ActionResult AddChainOfCustody(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evidence evidence = db.Evidence.Find(id);
            if (evidence == null)
            {
                return HttpNotFound();
            }
            ViewBag.EvidenceID = id;
            ViewBag.EvidenceNumber = evidence.EvidenceNumber;
            ViewBag.ReceivedBy = new SelectList(db.Users.Where(u => u.IsActive), "UserID", "FullName");
            ViewBag.ReleasedBy = new SelectList(db.Users.Where(u => u.IsActive), "UserID", "FullName");
            return View();
        }
        // POST: Evidences/AddChainOfCustody/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddChainOfCustody(int id, [Bind(Include = "ReceivedBy,ReleasedBy,TransferReason,Notes")] EvidenceChainOfCustody chain)
        {
            if (ModelState.IsValid)
            {
                chain.EvidenceID = id;
                chain.CustodyDate = DateTime.Now;
                db.EvidenceChainOfCustody.Add(chain);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = id });
            }
            ViewBag.EvidenceID = id;
            ViewBag.EvidenceNumber = db.Evidence.Find(id)?.EvidenceNumber;
            ViewBag.ReceivedBy = new SelectList(db.Users.Where(u => u.IsActive), "UserID", "FullName", chain.ReceivedBy);
            ViewBag.ReleasedBy = new SelectList(db.Users.Where(u => u.IsActive), "UserID", "FullName", chain.ReleasedBy);
            return View(chain);
        }
        private string GenerateEvidenceNumber()
        {
            var lastEvidence = db.Evidence.OrderByDescending(e => e.EvidenceID).FirstOrDefault();
            int lastNumber = lastEvidence != null ? int.Parse(lastEvidence.EvidenceNumber.Substring(2)) : 0;
            return "EV" + (lastNumber + 1).ToString("D6");
        }

        // GET: Evidences/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Evidence evidence = db.Evidence.Find(id);
        //    if (evidence == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(evidence);
        //}

        //// POST: Evidences/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Evidence evidence = db.Evidence.Find(id);
        //    db.Evidence.Remove(evidence);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
