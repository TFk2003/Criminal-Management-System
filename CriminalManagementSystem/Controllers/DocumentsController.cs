using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CriminalManagementSystem.Models;

namespace CriminalManagementSystem.Controllers
{
    [Authorize]
    public class DocumentsController : Controller
    {
        private ApplicationDBContext db = new ApplicationDBContext();

        // GET: Documents
        public ActionResult Index()
        {
            var documents = db.Documents.Include(d => d.DocumentType);
            return View(documents.ToList());
        }

        // GET: Documents/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Document document = db.Documents.Find(id);
        //    if (document == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(document);
        //}

        // GET: Documents/Create
        public ActionResult Create()
        {
            ViewBag.CaseID = new SelectList(db.Cases, "CaseID", "CaseNumber");
            ViewBag.CriminalID = new SelectList(db.Criminals, "CriminalID", "FullName");
            ViewBag.DocumentTypeID = new SelectList(db.DocumentTypes, "DocumentTypeID", "TypeName");
            ViewBag.IncidentID = new SelectList(db.Incidents, "IncidentID", "IncidentNumber");
            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DocumentTypeID,CaseID,CriminalID,IncidentID,Title,Description")] Document document, HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                ModelState.AddModelError("file", "Please select a file to upload.");
            }
            if (ModelState.ContainsKey("FilePath"))
            {
                ModelState["FilePath"].Errors.Clear();
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { x.Key, x.Value.Errors });

                foreach (var error in errors)
                {
                    System.Diagnostics.Debug.WriteLine($"Key: {error.Key}, Errors: {string.Join(", ", error.Errors.Select(e => e.ErrorMessage))}");
                }
            }

            if (ModelState.IsValid && file != null && file.ContentLength > 0)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine($"Hello");

                    string uploadPath = Server.MapPath("~/Uploads/Documents/");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    string fullPath = Path.Combine(uploadPath, fileName);

                    file.SaveAs(fullPath);

                    document.FilePath = fileName;
                    document.UploadedBy = db.Users.First(u => u.Username == User.Identity.Name).UserID;

                    db.Documents.Add(document);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error uploading file: " + ex.Message);
                }
            }
            else
            {
                if (file == null || file.ContentLength == 0)
                {
                    ModelState.AddModelError("file", "Please select a file to upload.");
                }
            }

            ViewBag.CaseID = new SelectList(db.Cases, "CaseID", "CaseNumber", document.CaseID);
            ViewBag.CriminalID = new SelectList(db.Criminals, "CriminalID", "FullName", document.CriminalID);
            ViewBag.DocumentTypeID = new SelectList(db.DocumentTypes, "DocumentTypeID", "TypeName", document.DocumentTypeID);
            ViewBag.IncidentID = new SelectList(db.Incidents, "IncidentID", "IncidentNumber", document.IncidentID);
            return View(document);
        }
        public FileResult Download(int id)
        {
            var document = db.Documents.Find(id);
            return File("~/Uploads/Documents/" + document.FilePath, "application/octet-stream", document.Title + System.IO.Path.GetExtension(document.FilePath));
        }

        // GET: Documents/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Document document = db.Documents.Find(id);
        //    if (document == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.CaseID = new SelectList(db.Cases, "CaseID", "CaseNumber", document.CaseID);
        //    ViewBag.CriminalID = new SelectList(db.Criminals, "CriminalID", "FirstName", document.CriminalID);
        //    ViewBag.DocumentTypeID = new SelectList(db.DocumentTypes, "DocumentTypeID", "TypeName", document.DocumentTypeID);
        //    ViewBag.IncidentID = new SelectList(db.Incidents, "IncidentID", "IncidentNumber", document.IncidentID);
        //    return View(document);
        //}

        //// POST: Documents/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "DocumentID,DocumentTypeID,CaseID,CriminalID,IncidentID,Title,Description,FilePath,UploadDate,UploadedBy")] Document document)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(document).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.CaseID = new SelectList(db.Cases, "CaseID", "CaseNumber", document.CaseID);
        //    ViewBag.CriminalID = new SelectList(db.Criminals, "CriminalID", "FirstName", document.CriminalID);
        //    ViewBag.DocumentTypeID = new SelectList(db.DocumentTypes, "DocumentTypeID", "TypeName", document.DocumentTypeID);
        //    ViewBag.IncidentID = new SelectList(db.Incidents, "IncidentID", "IncidentNumber", document.IncidentID);
        //    return View(document);
        //}

        //// GET: Documents/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Document document = db.Documents.Find(id);
        //    if (document == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(document);
        //}

        //// POST: Documents/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Document document = db.Documents.Find(id);
        //    db.Documents.Remove(document);
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
