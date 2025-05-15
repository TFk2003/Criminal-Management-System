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
        public class VictimsController : Controller
        {
            private ApplicationDBContext db = new ApplicationDBContext();

            // GET: Victims
            public ActionResult Index(string searchString)
            {
                var victims = db.Victims.AsQueryable();
                if (!string.IsNullOrEmpty(searchString)) 
                {
                    victims = victims.Where(v =>
                    v.FirstName.Contains(searchString) ||
                    v.LastName.Contains(searchString) ||
                    v.PhoneNumber.Contains(searchString));
                }
                return View(victims.OrderBy(v => v.LastName).ToList());
            }

            // GET: Victims/Details/5
                public ActionResult Details(int? id)
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Victim victim = db.Victims
                .Include(v => v.CaseVictims.Select(cv => cv.Case))
                .FirstOrDefault(v => v.VictimID == id);
                    if (victim == null)
                    {
                        return HttpNotFound();
                    }
                    return View(victim);
                }

            // GET: Victims/Create
            public ActionResult Create()
            {
                return View();
            }

            // POST: Victims/Create
            // To protect from overposting attacks, enable the specific properties you want to bind to, for 
            // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Create([Bind(Include = "FirstName,MiddleName,LastName,DateOfBirth,Gender,Address,PhoneNumber,Email")] Victim victim)
            {
                if (ModelState.IsValid)
                {
                    victim.CreatedBy = (int)Session["userID"];
                    victim.CreatedDate = DateTime.Now;
                    db.Victims.Add(victim);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(victim);
            }

            // GET: Victims/Edit/5
            public ActionResult Edit(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Victim victim = db.Victims.Find(id);
                if (victim == null)
                {
                    return HttpNotFound();
                }
                return View(victim);
            }

            // POST: Victims/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to, for 
            // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Edit([Bind(Include = "VictimID,FirstName,MiddleName,LastName,DateOfBirth,Gender,Address,PhoneNumber,Email")] Victim victim)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var existingVictim = db.Victims
                           .Find(victim.VictimID);
                        if (existingVictim== null)
                        {
                            return HttpNotFound();
                        }
                        existingVictim.FirstName = victim.FirstName;
                        existingVictim.MiddleName = victim.MiddleName;
                        existingVictim.LastName = victim.LastName;
                        existingVictim.DateOfBirth = victim.DateOfBirth;
                        existingVictim.Gender = victim.Gender;
                        existingVictim.Address = victim.Address;
                        existingVictim.Email = victim.Email;
                        existingVictim.PhoneNumber = victim.PhoneNumber;
                        existingVictim.ModifiedBy = (int)Session["userID"];
                        existingVictim.ModifiedDate = DateTime.Now;
                        db.Entry(existingVictim).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (DbUpdateException ex)
                    {
                        ModelState.AddModelError("", "Database error: " + ex.InnerException?.Message);
                    }
                }
                return View(victim);
            }

            //// GET: Victims/Delete/5
            //public ActionResult Delete(int? id)
            //{
            //    if (id == null)
            //    {
            //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //    }
            //    Victim victim = db.Victims.Find(id);
            //    if (victim == null)
            //    {
            //        return HttpNotFound();
            //    }
            //    return View(victim);
            //}

            //// POST: Victims/Delete/5
            //[HttpPost, ActionName("Delete")]
            //[ValidateAntiForgeryToken]
            //public ActionResult DeleteConfirmed(int id)
            //{
            //    Victim victim = db.Victims.Find(id);
            //    db.Victims.Remove(victim);
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
