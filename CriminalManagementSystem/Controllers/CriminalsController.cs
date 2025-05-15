using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CriminalManagementSystem.Models;

namespace CriminalManagementSystem.Controllers
{
    [Authorize]
    public class CriminalsController : Controller
    {
        private ApplicationDBContext db = new ApplicationDBContext();

        // GET: Criminals
        public ActionResult Index(string searchString)
        {
            var criminals = db.Criminals.Where(c => c.IsActive);
            if(!string.IsNullOrEmpty(searchString))
            {
                criminals = criminals.Where(c => 
                c.FirstName.Contains(searchString) || 
                c.LastName.Contains(searchString) ||
                c.NationalID.Contains(searchString));
            }
            return View(criminals.OrderBy(c => c.LastName).ToList());
        }

        // GET: Criminals/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Criminal criminal = db.Criminals
                .Include(c => c.Aliases)
                .Include(c => c.Arrests)
                .Include(c => c.Warrants)
                .FirstOrDefault(c => c.CriminalID == id);
            if (criminal == null)
            {
                return HttpNotFound();
            }
            return View(criminal);
        }

        // GET: Criminals/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Criminals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FirstName,MiddleName,LastName,DateOfBirth,Gender,Race,Height,Weight,EyeColor,HairColor,IdentifyingMarks,NationalID,SSN,LastKnownAddress,PhotoPath,FingerprintPath")] Criminal criminal,
    HttpPostedFileBase PhotoFile, HttpPostedFileBase FingerprintFile)
        {
            if (ModelState.IsValid)
            {
                if (PhotoFile != null && PhotoFile.ContentLength > 0)
                {
                    string photoPath = Path.Combine(Server.MapPath("~/Uploads/Photos"), Path.GetFileName(PhotoFile.FileName));
                    PhotoFile.SaveAs(photoPath);
                    criminal.PhotoPath = "/Uploads/Photos/" + PhotoFile.FileName;
                }

                if (FingerprintFile != null && FingerprintFile.ContentLength > 0)
                {
                    string fingerprintPath = Path.Combine(Server.MapPath("~/Uploads/Fingerprints"), Path.GetFileName(FingerprintFile.FileName));
                    FingerprintFile.SaveAs(fingerprintPath);
                    criminal.FingerprintPath = "/Uploads/Fingerprints/" + FingerprintFile.FileName;
                }
                if (criminal.Gender == "Male") { criminal.Gender = "M"; } else { criminal.Gender = "F"; }
                criminal.IsActive = true;
                criminal.CreatedBy = (int)Session["userID"];
                criminal.CreatedDate = DateTime.Now;
                db.Criminals.Add(criminal);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(criminal);
        }

        // GET: Criminals/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Criminal criminal = db.Criminals.Find(id);
            if (criminal == null)
            {
                return HttpNotFound();
            }
            return View(criminal);
        }

        // POST: Criminals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CriminalID,FirstName,MiddleName,LastName,DateOfBirth,Gender,Race,Height,Weight,EyeColor,HairColor,IdentifyingMarks,NationalID,SSN,LastKnownAddress,PhotoPath,FingerprintPath,IsActive")] Criminal criminal,
    HttpPostedFileBase PhotoFile, HttpPostedFileBase FingerprintFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingCriminal = db.Criminals
                    .FirstOrDefault(c => c.CriminalID == criminal.CriminalID);

                    if (existingCriminal == null)
                    {
                        return HttpNotFound();
                    }

                    if (PhotoFile != null && PhotoFile.ContentLength > 0)
                    {
                        string fileName = Guid.NewGuid() + Path.GetExtension(PhotoFile.FileName);
                        string photoPath = Path.Combine(Server.MapPath("~/Uploads/Photos"), fileName);
                        PhotoFile.SaveAs(photoPath);
                        existingCriminal.PhotoPath = "/Uploads/Photos/" + fileName;
                    }

                    if (FingerprintFile != null && FingerprintFile.ContentLength > 0)
                    {
                        string fileName = Guid.NewGuid() + Path.GetExtension(FingerprintFile.FileName);
                        string fingerprintPath = Path.Combine(Server.MapPath("~/Uploads/Fingerprints"), fileName);
                        FingerprintFile.SaveAs(fingerprintPath);
                        existingCriminal.FingerprintPath = "/Uploads/Fingerprints/" + fileName;
                    }
                    existingCriminal.FirstName = criminal.FirstName;
                    existingCriminal.MiddleName = criminal.MiddleName;
                    existingCriminal.LastName = criminal.LastName;
                    existingCriminal.DateOfBirth = criminal.DateOfBirth;
                    existingCriminal.Gender = criminal.Gender == "Male" ? "M" : "F";
                    existingCriminal.Race = criminal.Race;
                    existingCriminal.Height = criminal.Height;
                    existingCriminal.Weight = criminal.Weight;
                    existingCriminal.EyeColor = criminal.EyeColor;
                    existingCriminal.HairColor = criminal.HairColor;
                    existingCriminal.IdentifyingMarks = criminal.IdentifyingMarks;
                    existingCriminal.NationalID = criminal.NationalID;
                    existingCriminal.SSN = criminal.SSN;
                    existingCriminal.LastKnownAddress = criminal.LastKnownAddress;
                    existingCriminal.IsActive = criminal.IsActive;
                    existingCriminal.ModifiedBy = (int)Session["userID"];
                    existingCriminal.ModifiedDate = DateTime.Now;

                    db.Entry(existingCriminal).State = EntityState.Modified;


                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error: {ex}");

                    // Log full error details
                    var innerEx = ex;
                    while (innerEx != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Inner Error: {innerEx.Message}");
                        innerEx = innerEx.InnerException;
                    }

                    ModelState.AddModelError("", "Error saving changes. Please try again.");
                }
            }
            return View(criminal);
        }


        // GET: Criminals/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Criminal criminal = db.Criminals.Find(id);
            if (criminal == null)
            {
                return HttpNotFound();
            }
            return View(criminal);
        }

        // POST: Criminals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Criminal criminal = db.Criminals.Find(id);
            criminal.IsActive = false;
            db.Entry(criminal).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

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
