using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using CriminalManagementSystem.Models;

namespace CriminalManagementSystem.Controllers
{
    [Authorize]
    public class ArrestsController : Controller
    {
        private ApplicationDBContext db = new ApplicationDBContext();

        // GET: Arrests
        public ActionResult Index()
        {
            var arrests = db.Arrests.Include(a => a.Criminal).Include(a => a.ArrestingOfficer);
            return View(arrests.ToList());
        }

        // GET: Arrests/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Arrest arrest = db.Arrests
                .Include(a => a.Criminal)
                .Include(a => a.ArrestingOfficer)
                .Include(a => a.ArrestCharges.Select(ac => ac.Charge))
                .FirstOrDefault(a => a.ArrestID == id);
            if (arrest == null)
            {
                return HttpNotFound();
            }
            return View(arrest);
        }

        // GET: Arrests/Create
        public ActionResult Create(int? criminalId)
        {
            if(criminalId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.CriminalID = new SelectList(db.Criminals.Where(c => c.IsActive), "CriminalID", "FullName", criminalId);
            ViewBag.ArrestingOfficerID = new SelectList(db.Users.Where(u => u.IsActive), "UserID", "FullName");
            ViewBag.Charges = new SelectList(db.Charges, "ChargeID", "ChargeName");
            return View();
        }

        // POST: Arrests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CriminalID,ArrestingOfficerID,ArrestDate,ArrestLocation,ArrestDescription,Status")] Arrest arrest, int[] selectedCharges)
        {
            if (selectedCharges == null || selectedCharges.Length == 0)
            {
                ModelState.AddModelError("selectedCharges", "At least one charge must be selected");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    arrest.CreatedBy = (int)Session["userID"];
                    arrest.CreatedDate = DateTime.Now;
                    arrest.Status = arrest.Status ?? "Pending";
                    if (arrest.ArrestDate == default(DateTime))
                    {
                        ModelState.AddModelError("ArrestDate", "Arrest date is required");
                        throw new Exception("Missing required fields");
                    }
                    db.Arrests.Add(arrest);
                    db.SaveChanges();
                    foreach (var chargeId in selectedCharges)
                    {
                        db.ArrestCharges.Add(new ArrestCharge
                        {
                            ArrestID = arrest.ArrestID,
                            ChargeID = chargeId
                        });
                    }
                    var criminalExists = db.Criminals.Any(c => c.CriminalID == arrest.CriminalID);
                    if (!criminalExists)
                    {
                        ModelState.AddModelError("", "Associated criminal not found");
                        return View(arrest);
                    }
                    db.SaveChanges();
                    return RedirectToAction("Details", "Criminals", new { id = arrest.CriminalID });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving: " + ex.Message);
                }
            }
            ViewBag.CriminalID = new SelectList(db.Criminals.Where(c => c.IsActive), "CriminalID", "FullName", arrest.CriminalID);
            ViewBag.ArrestingOfficerID = new SelectList(db.Users.Where(u => u.IsActive), "UserID", "FullName", arrest.ArrestingOfficerID);
            ViewBag.Charges = new SelectList(db.Charges, "ChargeID", "ChargeName");
            return View(arrest);
        }

        // GET: Arrests/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Arrest arrest = db.Arrests
                .Include(a => a.ArrestCharges.Select(ac => ac.Charge))
                .FirstOrDefault(a => a.ArrestID == id);
            if (arrest == null)
            {
                return HttpNotFound();
            }
            ViewBag.CriminalID = new SelectList(db.Criminals, "CriminalID", "FirstName", arrest.CriminalID);
            ViewBag.AllCharges = new MultiSelectList(
                db.Charges.OrderBy(c => c.ChargeName).ToList(),
                "ChargeID",
                "ChargeName",
                arrest.ArrestCharges.Select(ac => ac.ChargeID).ToList()
            );
            return View(arrest);
        }

        // POST: Arrests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ArrestID,CriminalID,ArrestingOfficerID,ArrestDate,ArrestLocation,ArrestDescription,Status")] Arrest arrest, int[] SelectedCharges)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingArrest = db.Arrests
                        .Include(a => a.ArrestCharges)
                        .FirstOrDefault(a => a.ArrestID == arrest.ArrestID);
                    if (existingArrest == null)
                    {
                        return HttpNotFound();
                    }
                    existingArrest.ArrestDate = arrest.ArrestDate;
                    existingArrest.ArrestLocation = arrest.ArrestLocation;
                    existingArrest.ArrestDescription = arrest.ArrestDescription;
                    existingArrest.Status = arrest.Status;
                    arrest.ModifiedBy = (int)Session["userID"];
                    arrest.ModifiedDate = DateTime.Now;
                    UpdateArrestCharges(SelectedCharges, existingArrest);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Database error: " + ex.InnerException?.Message);
                }
                
            }
            ViewBag.CriminalID = new SelectList(db.Criminals, "CriminalID", "FirstName", arrest.CriminalID);
            ViewBag.AllCharges = new MultiSelectList(
                db.Charges.OrderBy(c => c.ChargeName).ToList(),
                "ChargeID",
                "ChargeName",
                SelectedCharges
            );
            return View(arrest);
        }
        private void UpdateArrestCharges(int[] selectedCharges, Arrest arrestToUpdate)
        {
            if (selectedCharges == null)
            {
                arrestToUpdate.ArrestCharges = new List<ArrestCharge>();
                return;
            }

            var selectedChargesHS = new HashSet<int>(selectedCharges);
            var arrestCharges = new HashSet<int>(arrestToUpdate.ArrestCharges.Select(ac => ac.ChargeID));

            foreach (var charge in db.Charges)
            {
                if (selectedChargesHS.Contains(charge.ChargeID))
                {
                    if (!arrestCharges.Contains(charge.ChargeID))
                    {
                        arrestToUpdate.ArrestCharges.Add(new ArrestCharge
                        {
                            ArrestID = arrestToUpdate.ArrestID,
                            ChargeID = charge.ChargeID
                        });
                    }
                }
                else
                {
                    if (arrestCharges.Contains(charge.ChargeID))
                    {
                        var chargeToRemove = arrestToUpdate.ArrestCharges
                            .FirstOrDefault(ac => ac.ChargeID == charge.ChargeID);
                        db.ArrestCharges.Remove(chargeToRemove);
                    }
                }
            }
        }

        //// GET: Arrests/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Arrest arrest = db.Arrests.Find(id);
        //    if (arrest == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(arrest);
        //}

        //// POST: Arrests/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Arrest arrest = db.Arrests.Find(id);
        //    db.Arrests.Remove(arrest);
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
