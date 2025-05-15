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

namespace CriminalManagementSystem.Controllers
{
    [Authorize]
    public class InmateBookingsController : Controller
    {
        private ApplicationDBContext db = new ApplicationDBContext();

        // GET: InmateBookings
        public ActionResult Index()
        {
            var inmateBookings = db.InmateBookings.Include(i => i.Criminal).Include(i => i.Facility);
            return View(inmateBookings.ToList());
        }

        // GET: InmateBookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InmateBooking inmateBooking = db.InmateBookings
                .Include(i => i.Criminal)
                .Include(i => i.Facility)
                .Include(i => i.InmateMedicalRecords)
                .FirstOrDefault(i => i.BookingID == id);
            if (inmateBooking == null)
            {
                return HttpNotFound();
            }
            return View(inmateBooking);
        }

        // GET: InmateBookings/Create
        public ActionResult Create(int? criminalId)
        {
            ViewBag.CriminalID = new SelectList(db.Criminals, "CriminalID", "FullName", criminalId);
            ViewBag.FacilityID = new SelectList(db.Facilities, "FacilityID", "FacilityName");
            InmateBooking inmateBooking = new InmateBooking
            {
                BookingNumber = GenerateBookingNumber()
            };
            return View(inmateBooking);
        }

        // POST: InmateBookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookingNumber,CriminalID,FacilityID,BookingDate,ReleasedDate,Status,CellNumber")] InmateBooking inmateBooking, List<InmateMedicalRecord> MedicalRecords)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    inmateBooking.CreatedBy = (int)Session["userID"];
                inmateBooking.CreatedDate = DateTime.Now;
                db.InmateBookings.Add(inmateBooking);
                db.SaveChanges();
                if (MedicalRecords != null)
                {
                    foreach (var record in MedicalRecords.Where(r => !string.IsNullOrEmpty(r.MedicalCondition)))
                    {
                        record.BookingID = inmateBooking.BookingID;
                        db.InmateMedicalRecords.Add(record);
                    }
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error saving booking: " + ex.Message);
                }
            }

            ViewBag.CriminalID = new SelectList(db.Criminals, "CriminalID", "FullName", inmateBooking.CriminalID);
            ViewBag.FacilityID = new SelectList(db.Facilities, "FacilityID", "FacilityName", inmateBooking.FacilityID);
            return View(inmateBooking);
        }

        // GET: InmateBookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InmateBooking inmateBooking = db.InmateBookings
                .Include(ib => ib.InmateMedicalRecords)
                .SingleOrDefault(ib => ib.BookingID == id);
            if (inmateBooking == null)
            {
                return HttpNotFound();
            }
            ViewBag.CriminalID = new SelectList(db.Criminals.Where(c => c.IsActive), "CriminalID", "FullName", inmateBooking.CriminalID);
            ViewBag.FacilityID = new SelectList(db.Facilities, "FacilityID", "FacilityName", inmateBooking.FacilityID);
            return View(inmateBooking);
        }

        // POST: InmateBookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(InmateBooking inmateBooking)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingBooking = db.InmateBookings
                        .Include(ib => ib.InmateMedicalRecords)
                        .SingleOrDefault(ib => ib.BookingID == inmateBooking.BookingID);
                    if (existingBooking == null)
                    {
                        return HttpNotFound();
                    }
                    existingBooking.CriminalID = inmateBooking.CriminalID;
                    existingBooking.FacilityID = inmateBooking.FacilityID;
                    existingBooking.BookingDate = inmateBooking.BookingDate;
                    existingBooking.ReleasedDate = inmateBooking.ReleasedDate;
                    existingBooking.Status = inmateBooking.Status;
                    existingBooking.CellNumber = inmateBooking.CellNumber;
                    existingBooking.ModifiedBy = (int)Session["userID"];
                    existingBooking.ModifiedDate = DateTime.Now;
                    db.Entry(existingBooking).State = EntityState.Modified;
                    db.SaveChanges();
                    if (inmateBooking.InmateMedicalRecords != null)
                    {
                        foreach (var medicalRecord in inmateBooking.InmateMedicalRecords)
                        {
                            if (medicalRecord.MedicalRecordID > 0) // Existing record
                            {
                                var existingRecord = existingBooking.InmateMedicalRecords
                                    .FirstOrDefault(mr => mr.MedicalRecordID == medicalRecord.MedicalRecordID);

                                if (existingRecord != null)
                                {
                                    existingRecord.RecordDate = medicalRecord.RecordDate;
                                    existingRecord.MedicalCondition = medicalRecord.MedicalCondition;
                                    existingRecord.Treatment = medicalRecord.Treatment;
                                    existingRecord.Physician = medicalRecord.Physician;
                                    existingRecord.Notes = medicalRecord.Notes;
                                }
                            }
                            else // New record
                            {
                                medicalRecord.BookingID = existingBooking.BookingID;
                                existingBooking.InmateMedicalRecords.Add(medicalRecord);
                            }
                        }

                        // Find records to delete
                        var postedRecordIds = inmateBooking.InmateMedicalRecords
                            .Where(mr => mr.MedicalRecordID > 0)
                            .Select(mr => mr.MedicalRecordID)
                            .ToList();

                        var recordsToDelete = existingBooking.InmateMedicalRecords
                            .Where(mr => !postedRecordIds.Contains(mr.MedicalRecordID))
                            .ToList();

                        foreach (var record in recordsToDelete)
                        {
                            db.InmateMedicalRecords.Remove(record);
                        }
                    }

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {
                    // Log the full exception details
                    System.Diagnostics.Debug.WriteLine(ex.ToString());

                    // Handle specific database errors
                    if (ex.InnerException != null && ex.InnerException.InnerException != null)
                    {
                        var sqlException = ex.InnerException.InnerException as System.Data.SqlClient.SqlException;
                        if (sqlException != null)
                        {
                            if (sqlException.Number == 547) // Foreign key violation
                            {
                                ModelState.AddModelError("", "Couldn't save changes. Related data doesn't exist.");
                            }
                            else if (sqlException.Number == 2601 || sqlException.Number == 2627) // Unique constraint
                            {
                                ModelState.AddModelError("", "Duplicate data detected. Please check your inputs.");
                            }
                            else
                            {
                                ModelState.AddModelError("", "Database error occurred: " + sqlException.Message);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Error: " + ex.InnerException.Message);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "An error occurred while saving. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error: " + ex.Message);
                }
            }
            ViewBag.CriminalID = new SelectList(db.Criminals, "CriminalID", "FullName", inmateBooking.CriminalID);
            ViewBag.FacilityID = new SelectList(db.Facilities, "FacilityID", "FacilityName", inmateBooking.FacilityID);
            inmateBooking.InmateMedicalRecords = db.InmateMedicalRecords
               .Where(mr => mr.BookingID == inmateBooking.BookingID)
               .ToList();
            return View(inmateBooking);
        }
        private string GenerateBookingNumber()
        {
            var lastBooking = db.InmateBookings.OrderByDescending(w => w.BookingID).FirstOrDefault();
            int lastNumber = lastBooking != null ? int.Parse(lastBooking.BookingNumber.Substring(2)) : 0;
            return "BK" + (lastNumber + 1).ToString("D6");
        }

        // GET: InmateBookings/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    InmateBooking inmateBooking = db.InmateBookings.Find(id);
        //    if (inmateBooking == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(inmateBooking);
        //}

        //// POST: InmateBookings/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    InmateBooking inmateBooking = db.InmateBookings.Find(id);
        //    db.InmateBookings.Remove(inmateBooking);
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
