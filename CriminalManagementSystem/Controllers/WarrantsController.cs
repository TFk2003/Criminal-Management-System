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
    public class WarrantsController : Controller
    {
        private ApplicationDBContext db = new ApplicationDBContext();

        // GET: Warrants
        public ActionResult Index()
        {
            var warrants = db.Warrants.Include(w => w.Criminal);
            return View(warrants.ToList());
        }

        // GET: Warrants/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warrant warrant = db.Warrants.Find(id);
            if (warrant == null)
            {
                return HttpNotFound();
            }
            return View(warrant);
        }

        // GET: Warrants/Create
        public ActionResult Create(int? criminalId)
        {
            Warrant warrant = new Warrant
            {
                WarrantNumber = GenerateWarrantNumber()
            };
            ViewBag.CriminalID = new SelectList(db.Criminals.Where(c => c.IsActive), "CriminalID", "FullName", criminalId);
            return View(warrant);
        }

        // POST: Warrants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WarrantNumber,CriminalID,WarrantType,IssueDate,ExpirationDate,IssuingJudge,Status,Description")] Warrant warrant)
        {
            if (ModelState.IsValid)
            {
                warrant.CreatedBy = (int)Session["userID"];
                warrant.CreatedDate = DateTime.Now;
                db.Warrants.Add(warrant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CriminalID = new SelectList(db.Criminals.Where(c => c.IsActive), "CriminalID", "FullName", warrant.CriminalID);
            return View(warrant);
        }
        private string GenerateWarrantNumber()
        {
            var lastWarrant = db.Warrants.OrderByDescending(w => w.WarrantID).FirstOrDefault();
            int lastNumber = lastWarrant != null ? int.Parse(lastWarrant.WarrantNumber.Substring(2)) : 0;
            return "WR" + (lastNumber + 1).ToString("D6");
        }

        // GET: Warrants/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Warrant warrant = db.Warrants.Find(id);
        //    if (warrant == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.CriminalID = new SelectList(db.Criminals, "CriminalID", "FirstName", warrant.CriminalID);
        //    return View(warrant);
        //}

        //// POST: Warrants/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "WarrantID,CriminalID,WarrantNumber,WarrantType,IssueDate,ExpirationDate,IssuingJudge,Status,Description")] Warrant warrant)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(warrant).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.CriminalID = new SelectList(db.Criminals, "CriminalID", "FirstName", warrant.CriminalID);
        //    return View(warrant);
        //}

        //// GET: Warrants/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Warrant warrant = db.Warrants.Find(id);
        //    if (warrant == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(warrant);
        //}

        //// POST: Warrants/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Warrant warrant = db.Warrants.Find(id);
        //    db.Warrants.Remove(warrant);
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
