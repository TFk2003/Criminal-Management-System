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
    public class CourtHearingsController : Controller
    {
        private ApplicationDBContext db = new ApplicationDBContext();

        // GET: CourtHearings
        public ActionResult Index(int? caseId)
        {
            IQueryable<CourtHearing> hearings = db.CourtHearings.Include(c => c.Case).Include(c => c.Court);
            if (caseId.HasValue)
            {
                hearings = hearings.Where(h => h.CaseID == caseId);
                ViewBag.CaseID = caseId;
                ViewBag.CaseNumber = db.Cases.Find(caseId)?.CaseNumber;
            }
            return View(hearings.OrderBy(h => h.HearingDate).ToList());
        }

        // GET: CourtHearings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourtHearing courtHearing = db.CourtHearings
                .Include(c => c.Case)
                .Include(c => c.Court)
                .FirstOrDefault(c => c.HearingID == id);
            if (courtHearing == null)
            {
                return HttpNotFound();
            }
            return View(courtHearing);
        }

        // GET: CourtHearings/Create
        public ActionResult Create(int? caseId)
        {
            ViewBag.CaseID = new SelectList(db.Cases.Where(c => c.Status == "Open"), "CaseID", "CaseNumber", caseId);
            ViewBag.CourtID = new SelectList(db.Courts, "CourtID", "CourtName");
            return View();
        }

        // POST: CourtHearings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CaseID,CourtID,HearingDate,HearingType,RoomNumber,Outcome,NextHearingDate,Notes")] CourtHearing courtHearing)
        {
            if (ModelState.IsValid)
            {
                courtHearing.CreatedBy = (int)Session["userID"];
                courtHearing.CreatedDate = DateTime.Now;
                db.CourtHearings.Add(courtHearing);
                db.SaveChanges();
                return RedirectToAction("Index", new {caseId = courtHearing.CaseID});
            }

            ViewBag.CaseID = new SelectList(db.Cases.Where(c => c.Status == "Open"), "CaseID", "CaseNumber", courtHearing.CaseID);
            ViewBag.CourtID = new SelectList(db.Courts, "CourtID", "CourtName", courtHearing.CourtID);
            return View(courtHearing);
        }

        // GET: CourtHearings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourtHearing courtHearing = db.CourtHearings.Find(id);
            if (courtHearing == null)
            {
                return HttpNotFound();
            }
            ViewBag.CaseID = new SelectList(db.Cases.Where(c => c.Status == "Open"), "CaseID", "CaseNumber", courtHearing.CaseID);
            ViewBag.CourtID = new SelectList(db.Courts, "CourtID", "CourtName", courtHearing.CourtID);
            return View(courtHearing);
        }

        // POST: CourtHearings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HearingID,CaseID,CourtID,HearingDate,HearingType,RoomNumber,Outcome,NextHearingDate,Notes")] CourtHearing courtHearing)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingCourtHearing = db.CourtHearings
                       .Find(courtHearing.HearingID);
                    if (existingCourtHearing == null)
                    {
                        return HttpNotFound();
                    }
                    existingCourtHearing.CaseID = courtHearing.CaseID;
                    existingCourtHearing.CourtID = courtHearing.CourtID;
                    existingCourtHearing.HearingDate= courtHearing.HearingDate;
                    existingCourtHearing.RoomNumber = courtHearing.RoomNumber;
                    existingCourtHearing.Outcome = courtHearing.Outcome;
                    existingCourtHearing.NextHearingDate = courtHearing.NextHearingDate;
                    existingCourtHearing.Notes = courtHearing.Notes;
                    existingCourtHearing.ModifiedBy = (int)Session["userID"];
                    existingCourtHearing.ModifiedDate = DateTime.Now;
                    db.Entry(existingCourtHearing).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Database error: " + ex.InnerException?.Message);
                }
            }
            ViewBag.CaseID = new SelectList(db.Cases.Where(c => c.Status == "Open"), "CaseID", "CaseNumber", courtHearing.CaseID);
            ViewBag.CourtID = new SelectList(db.Courts, "CourtID", "CourtName", courtHearing.CourtID);
            return View(courtHearing);
        }

        // GET: CourtHearings/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    CourtHearing courtHearing = db.CourtHearings.Find(id);
        //    if (courtHearing == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(courtHearing);
        //}

        //// POST: CourtHearings/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    CourtHearing courtHearing = db.CourtHearings.Find(id);
        //    db.CourtHearings.Remove(courtHearing);
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
