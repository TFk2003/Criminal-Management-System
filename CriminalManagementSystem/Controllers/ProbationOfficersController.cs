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
    public class ProbationOfficersController : Controller
    {
        private ApplicationDBContext db = new ApplicationDBContext();

        // GET: ProbationOfficers
        public ActionResult Index()
        {
            if (Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }
            var probationOfficers = db.ProbationOfficers.Include(p => p.User);
            return View(probationOfficers.ToList());
        }

        // GET: ProbationOfficers/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProbationOfficer probationOfficer = db.ProbationOfficers.Find(id);
            if (probationOfficer == null)
            {
                return HttpNotFound();
            }
            return View(probationOfficer);
        }

        // GET: ProbationOfficers/Create
        public ActionResult Create()
        {
            if (Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.UserID = new SelectList(db.Users.Where(u => u.IsActive), "UserID", "FullName");
            return View();
        }

        // POST: ProbationOfficers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,BadgeNumber,Specialization")] ProbationOfficer probationOfficer)
        {
            if (ModelState.IsValid)
            {
                db.ProbationOfficers.Add(probationOfficer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.Users.Where(u => u.IsActive), "UserID", "FullName", probationOfficer.UserID);
            return View(probationOfficer);
        }

        // GET: ProbationOfficers/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ProbationOfficer probationOfficer = db.ProbationOfficers.Find(id);
        //    if (probationOfficer == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.UserID = new SelectList(db.Users, "UserID", "Username", probationOfficer.UserID);
        //    return View(probationOfficer);
        //}

        //// POST: ProbationOfficers/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "OfficerID,UserID,BadgeNumber,Specialization")] ProbationOfficer probationOfficer)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(probationOfficer).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.UserID = new SelectList(db.Users, "UserID", "Username", probationOfficer.UserID);
        //    return View(probationOfficer);
        //}

        //// GET: ProbationOfficers/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ProbationOfficer probationOfficer = db.ProbationOfficers.Find(id);
        //    if (probationOfficer == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(probationOfficer);
        //}

        //// POST: ProbationOfficers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    ProbationOfficer probationOfficer = db.ProbationOfficers.Find(id);
        //    db.ProbationOfficers.Remove(probationOfficer);
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
