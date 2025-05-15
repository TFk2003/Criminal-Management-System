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
    public class CasesController : Controller
    {
        private ApplicationDBContext db = new ApplicationDBContext();

        // GET: Cases
        public ActionResult Index(string statusFilter)
        {
            IQueryable<Case> cases = db.Cases.Include(c => c.AssignedOfficer);
            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
            {
                cases = cases.Where(c => c.Status == statusFilter);
            }
            ViewBag.StatusFilter = new SelectList(new [] { "All", "Open", "Closed", "Pending" }, statusFilter ?? "All");
            return View(cases.OrderByDescending(c => c.OpeningDate).ToList());
        }

        // GET: Cases/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Case @case = db.Cases
                .Include(c => c.AssignedOfficer)
                .Include(c => c.CaseCriminals.Select(cc => cc.Criminal))
                .Include(c => c.CaseWitnesses.Select(cw => cw.Witness))
                .Include(c => c.CaseVictims.Select(cv => cv.Victim))
                .Include(c => c.Evidence)
                .Include(c => c.CourtHearings)
                .FirstOrDefault(c => c.CaseID == id);
            if (@case == null)
            {
                return HttpNotFound();
            }
            return View(@case);
        }

        // GET: Cases/Create
        public ActionResult Create()
        {
            var newCase = new Case
            {
                CaseNumber = GenerateCaseNumber(),
                OpeningDate = DateTime.Now,
                Status = "Open",
                Priority = "Medium"
            };
            System.Diagnostics.Debug.WriteLine("Form submitted");
            ViewBag.AssignedOfficerID = new SelectList(db.Users.Where(u => u.IsActive), "UserID", "FullName");
            return View(newCase);
        }

        // POST: Cases/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CaseNumber,CaseTitle,CaseDescription,CaseType,Status,Priority,AssignedOfficerID")] Case @case)
        {
            System.Diagnostics.Debug.WriteLine("Form submitted");
            System.Diagnostics.Debug.WriteLine($"ModelState isValid: {ModelState.IsValid}");

            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in {state.Key}: {error.ErrorMessage}");
                }
            }
            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(@case.CaseNumber))
                    {
                        @case.CaseNumber = GenerateCaseNumber();
                    }
                    @case.OpeningDate = DateTime.Now;
                    @case.CreatedBy = (int)Session["userID"];
                    @case.CreatedDate = DateTime.Now;
                    db.Cases.Add(@case);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateException dbEx)
            {
                // Log the inner exception which usually has the real error
                var innerException = dbEx.InnerException?.InnerException ?? dbEx.InnerException ?? dbEx;
                System.Diagnostics.Debug.WriteLine("DB Update Error: " + innerException.Message);
                ModelState.AddModelError("", "Database error: " + innerException.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred: " + ex.Message);
            }
            ViewBag.AssignedOfficerID = new SelectList(db.Users.Where(u => u.IsActive), "UserID", "FullName", @case.AssignedOfficerID);

            return View(@case);
        }

        // GET: Cases/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Case @case = db.Cases.Find(id);
            if (@case == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssignedOfficerID = new SelectList(db.Users.Where(u => u.IsActive), "UserID", "FullName", @case.AssignedOfficerID);
            return View(@case);
        }

        // POST: Cases/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CaseID,CaseNumber,CaseTitle,CaseDescription,CaseType,Status,Priority,OpeningDate,ClosingDate,AssignedOfficerID")] Case @case)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingCase = db.Cases.Find(@case.CaseID);
                    if (existingCase == null)
                    {
                        return HttpNotFound();
                    }
                    existingCase.CaseTitle = @case.CaseTitle;
                    existingCase.CaseDescription = @case.CaseDescription;
                    existingCase.Status = @case.Status;
                    existingCase.Priority = @case.Priority;
                    existingCase.ClosingDate = @case.ClosingDate;
                    existingCase.ModifiedBy = (int) Session["userID"];
                    existingCase.ModifiedDate = DateTime.Now;
                    db.Entry(existingCase).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Database error: " + ex.InnerException?.Message);
                }
            }
            ViewBag.AssignedOfficerID = new SelectList(db.Users.Where(u => u.IsActive), "UserID", "FullName", @case.AssignedOfficerID);
            return View(@case);
        }
        // GET: Cases/Close/5
        public ActionResult Close(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Case @case = db.Cases.Find(id);
            if (@case == null)
            {
                return HttpNotFound();
            }
            return View(@case);
        }
        // POST: Cases/Close/5
        [HttpPost, ActionName("Close")]
        [ValidateAntiForgeryToken]
        public ActionResult CloseConfirmed(int id)
        {
            Case @case = db.Cases.Find(id);
            if (@case == null)
            {
                return HttpNotFound();
            }
            @case.Status = "Closed";
            @case.ClosingDate = DateTime.Now;
            db.Entry(@case).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        private string GenerateCaseNumber()
        {
            try
            {
                var lastCase = db.Cases.OrderByDescending(c => c.CaseID).FirstOrDefault();
                if (lastCase != null && lastCase.CaseNumber.StartsWith("CS"))
                {
                    if (int.TryParse(lastCase.CaseNumber.Substring(2), out int lastNumber))
                    {
                        return "CS" + (lastNumber + 1).ToString("D6");
                    }
                }
                return "CS000001";
            }
            catch
            {
                return "CS" + DateTime.Now.ToString("yyMMdd") + "01";
            }
        }
        // GET: Cases/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Case @case = db.Cases.Find(id);
        //    if (@case == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(@case);
        //}

        //// POST: Cases/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Case @case = db.Cases.Find(id);
        //    db.Cases.Remove(@case);
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
