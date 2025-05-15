using CriminalManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CriminalManagementSystem.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private ApplicationDBContext db = new ApplicationDBContext();
        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }
        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users
                    .FirstOrDefault(u => u.Username == model.Username && u.PasswordHash == model.Password && u.IsActive);
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(user.Username, false);

                    UserRole userRole = db.UserRoles.FirstOrDefault(ur => ur.UserID == user.UserID);
                    Session["userID"] = user.UserID;
                    Session["userName"] = user.FullName;
                    Session["Role"] = userRole.Role.RoleName ?? "Officer";

                    user.LastLogin = DateTime.Now;
                    db.SaveChanges();
                    if(Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid username or password.");
            }
            return View(model);
        }
        // GET: Account/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.Any(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Username already exists");
                    return View(model);
                }

                if (db.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email already registered");
                    return View(model);
                }

                var user = new User
                {
                    Username = model.Username,
                    PasswordHash = model.Password,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    IsActive = true,
                    PhoneNumber = model.PhoneNumber,
                    CreatedDate = DateTime.Now,
                    LastLogin = DateTime.Now
                };

                db.Users.Add(user);
                db.SaveChanges();
                var defaultRole = db.Roles.FirstOrDefault(r => r.RoleName == "Officer");
                if (defaultRole != null)
                {
                    db.UserRoles.Add(new UserRole
                    {
                        UserID = user.UserID,
                        RoleID = defaultRole.RoleID
                    });
                    db.SaveChanges();
                }

                FormsAuthentication.SetAuthCookie(user.Username, false);
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult ManageRoles()
        {
            var viewModel = new List<UserRoleViewModel>();
            var allRoles = db.Roles.ToList();
            var allUsers = db.Users.ToList();

            foreach (var user in allUsers)
            {
                var userRoles = db.UserRoles
                                 .Where(ur => ur.UserID == user.UserID)
                                 .Select(ur => ur.RoleID)
                                 .ToList();

                var userRoleViewModel = new UserRoleViewModel
                {
                    UserID = user.UserID,
                    Username = user.Username,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Roles = allRoles.Select(role => new RoleSelection
                    {
                        RoleID = role.RoleID,
                        RoleName = role.RoleName,
                        IsSelected = userRoles.Contains(role.RoleID)
                    }).ToList()
                };

                viewModel.Add(userRoleViewModel);
            }

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult ManageRoles(List<UserRoleViewModel> model)
        {
            if (ModelState.IsValid)
            {
                foreach (var userModel in model)
                {
                    // Get current roles for the user
                    var currentUserRoles = db.UserRoles
                                           .Where(ur => ur.UserID == userModel.UserID)
                                           .ToList();

                    // Determine roles to add and remove
                    var selectedRoleIds = userModel.Roles
                                                 .Where(r => r.IsSelected)
                                                 .Select(r => r.RoleID)
                                                 .ToList();

                    // Remove unselected roles
                    var rolesToRemove = currentUserRoles
                                       .Where(cur => !selectedRoleIds.Contains(cur.RoleID))
                                       .ToList();

                    foreach (var role in rolesToRemove)
                    {
                        db.UserRoles.Remove(role);
                    }

                    // Add new roles
                    var existingRoleIds = currentUserRoles.Select(cur => cur.RoleID).ToList();
                    var rolesToAdd = selectedRoleIds
                                   .Where(id => !existingRoleIds.Contains(id))
                                   .ToList();

                    foreach (var roleId in rolesToAdd)
                    {
                        db.UserRoles.Add(new UserRole
                        {
                            UserID = userModel.UserID,
                            RoleID = roleId
                        });
                    }
                }

                db.SaveChanges();
                return RedirectToAction("ManageRoles");
            }

            // If we got here, something failed; repopulate the view model
            var allRoles = db.Roles.ToList();
            foreach (var userModel in model)
            {
                userModel.Roles = allRoles.Select(role => new RoleSelection
                {
                    RoleID = role.RoleID,
                    RoleName = role.RoleName,
                    IsSelected = userModel.Roles?.Any(r => r.RoleID == role.RoleID && r.IsSelected) ?? false
                }).ToList();
            }

            return View(model);
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

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Phone Number")]
        [Phone]
        public string PhoneNumber { get; set; }
    }
    public class UserRoleViewModel
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public List<RoleSelection> Roles { get; set; } = new List<RoleSelection>();
        public int[] SelectedRoleIDs { get; set; }
    }
    public class RoleSelection
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
}