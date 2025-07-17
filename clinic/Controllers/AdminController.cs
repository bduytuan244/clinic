using clinic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace clinic.Controllers
{
    public class AdminController : Controller
    {
        dbDClinicDataContext db = new dbDClinicDataContext();
        // GET: Admin
        private bool IsAdminLoggedIn()
        {
            return Session["Admin"] != null;
        }
        [HttpGet]
        public ActionResult Dashboard() /*login phân trang */
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            TempData["WelcomeMessage"] = "Chào Mừng Admins đã quay trở lại!";
            return RedirectToAction("Index", "AccountStore");
        }
        public ActionResult ManageUsers()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Login");
            }

            var users = db.customers.ToList();
            return View(users);
        }
        public ActionResult DeleteUser(int id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Login");
            }

            var user = db.customers.SingleOrDefault(c => c.patient_id == id);
            if (user != null)
            {
                db.customers.DeleteOnSubmit(user);
                db.SubmitChanges();
                ViewBag.Message = "User deleted successfully.";
            }
            else
            {
                ViewBag.Message = "User not found.";
            }
            return RedirectToAction("ManageUsers");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult Index()
        {
            return View();
        }
    }
}