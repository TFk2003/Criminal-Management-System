using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CriminalManagementSystem.Models;

namespace CriminalManagementSystem.Controllers
{
    [Authorize]
    public class IncidentsController : Controller
    {
        private ApplicationDBContext db = new ApplicationDBContext();

        // GET: Incidents
        public ActionResult Index()
        {
            var incidents = db.Incidents.Include(i => i.AssignedOfficer).ToList();
            return View(incidents);
        }

        // GET: Incidents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Incident incident = db.Incidents
                .Include(i => i.AssignedOfficer)
                .Include(i => i.IncidentCases.Select(ic => ic.Case))
                .FirstOrDefault(i => i.IncidentID == id);
            if (incident == null)
            {
                return HttpNotFound();
            }
            return View(incident);
        }

        // GET: Incidents/Create
        public ActionResult Create()
        {
            Incident incident = new Incident
            {
                IncidentNumber = GenerateIncidentNumber()
            };
            ViewBag.AssignedOfficerID = new SelectList(db.Users, "UserID", "FullName");
            return View(incident);
        }

        // POST: Incidents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IncidentNumber,IncidentType,IncidentDate,Location,Description,Status,ReportedBy,ReportedDate,AssignedOfficerID")] Incident incident)
        {
            if (ModelState.IsValid)
            {
                incident.CreatedBy = (int)Session["userID"];
                incident.CreatedDate = DateTime.Now;
                db.Incidents.Add(incident);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssignedOfficerID = new SelectList(db.Users, "UserID", "FullName", incident.AssignedOfficerID);

            return View(incident);
        }
        private string GenerateIncidentNumber()
        {
            var lastIncident = db.Incidents.OrderByDescending(i => i.IncidentID).FirstOrDefault();
            int lastNumber = lastIncident != null ? int.Parse(lastIncident.IncidentNumber.Substring(2)) : 0;
            return "IN" + (lastNumber + 1).ToString("D6");
        }

        // GET: Incidents/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Incident incident = db.Incidents.Find(id);
        //    if (incident == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(incident);
        //}

        //// POST: Incidents/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "IncidentID,IncidentNumber,IncidentType,IncidentDate,Location,Description,Status,ReportedBy,ReportedDate,AssignedOfficerID")] Incident incident)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(incident).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(incident);
        //}

        //// GET: Incidents/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Incident incident = db.Incidents.Find(id);
        //    if (incident == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(incident);
        //}

        //// POST: Incidents/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Incident incident = db.Incidents.Find(id);
        //    db.Incidents.Remove(incident);
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
