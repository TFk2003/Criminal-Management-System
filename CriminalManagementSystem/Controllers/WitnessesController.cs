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
    public class WitnessesController : Controller
    {
        private ApplicationDBContext db = new ApplicationDBContext();

        // GET: Witnesses
        public ActionResult Index(string searchString)
        {
            var witnesses = db.Witnesses.AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                witnesses = witnesses.Where(w =>
                w.FirstName.Contains(searchString) ||
                w.LastName.Contains(searchString) ||
                w.PhoneNumber.Contains(searchString));
            }
            return View(witnesses.OrderBy(w => w.LastName).ToList());
        }

        // GET: Witnesses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Witness witness = db.Witnesses
                .Include(w => w.CaseWitnesses.Select(cw => cw.Case))
                .FirstOrDefault(w => w.WitnessID == id);
            if (witness == null)
            {
                return HttpNotFound();
            }
            return View(witness);
        }

        // GET: Witnesses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Witnesses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FirstName,MiddleName,LastName,Address,PhoneNumber,Email,DateOfBirth,Gender,RelationshipToCase")] Witness witness)
        {
            if (ModelState.IsValid)
            {
                witness.CreatedBy = (int)Session["userID"];
                witness.CreatedDate = DateTime.Now;
                db.Witnesses.Add(witness);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(witness);
        }

        // GET: Witnesses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Witness witness = db.Witnesses.Find(id);
            if (witness == null)
            {
                return HttpNotFound();
            }
            return View(witness);
        }

        // POST: Witnesses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WitnessID,FirstName,MiddleName,LastName,Address,PhoneNumber,Email,DateOfBirth,Gender,RelationshipToCase")] Witness witness)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingWitness = db.Witnesses
                       .Find(witness.WitnessID);
                    if (existingWitness == null)
                    {
                        return HttpNotFound();
                    }
                    existingWitness.FirstName = witness.FirstName;
                    existingWitness.MiddleName= witness.MiddleName;
                    existingWitness.LastName = witness.LastName;
                    existingWitness.DateOfBirth= witness.DateOfBirth;
                    existingWitness.Gender= witness.Gender;
                    existingWitness.Address = witness.Address;
                    existingWitness.Email = witness.Email;
                    existingWitness.PhoneNumber = witness.PhoneNumber;
                    existingWitness.RelationshipToCase = witness.RelationshipToCase;
                    existingWitness.ModifiedBy = (int)Session["userID"];
                    existingWitness.ModifiedDate = DateTime.Now;
                    db.Entry(existingWitness).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Database error: " + ex.InnerException?.Message);
                }
            }
            return View(witness);
        }

        // GET: Witnesses/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Witness witness = db.Witnesses.Find(id);
        //    if (witness == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(witness);
        //}

        //// POST: Witnesses/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Witness witness = db.Witnesses.Find(id);
        //    db.Witnesses.Remove(witness);
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
