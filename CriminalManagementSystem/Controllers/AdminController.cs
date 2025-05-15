using CriminalManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;

namespace CriminalManagementSystem.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private ApplicationDBContext db = new ApplicationDBContext();
        public ActionResult Users()
        {
            if (Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }
            var users = db.Users.Include(u => u.UserRoles.Select(ur => ur.Role));
            return View(users);
        }
        public ActionResult Roles()
        {
            if (Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }
            return View(db.Roles.ToList());
        }
        public ActionResult Permissions()
        {
            if (Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }
            return View(db.Permissions.ToList());
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        // GET: Admin
        public ActionResult Index()
        {
            if (Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }
                return View();
        }
    }
}