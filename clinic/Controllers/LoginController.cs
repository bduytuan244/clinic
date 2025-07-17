using clinic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace clinic.Controllers
{
    public class LoginController : Controller
    {
        dbDClinicDataContext db = new dbDClinicDataContext();
        // GET: Login
        public ActionResult Login()
        {
            if (Session["User"] != null || Session["Admin"] != null)
            {
                return RedirectToAction("Home", "AccountStore");
            }
            return View();
        }

        // POST: Login
        [HttpPost]
        public ActionResult Login(LoginView model)
        {
            if (ModelState.IsValid)
            {

                var user = db.customers.SingleOrDefault(n => n.full_name == model.username && n.password == model.password);


                var admin = db.admins.SingleOrDefault(a => a.username == model.username && a.password == model.password);

                if (user != null)
                {
                    Session["User"] = user;
                    Session["CustomerId"] = user.patient_id;
                    Session["CustomerName"] = user.full_name;
                    Session["Role"] = "Customer";

                    return RedirectToAction("Home", "AccountStore");
                }
                else if (admin != null)
                {
                    Session["Admin"] = admin;
                    Session["Role"] = "Admin";

                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    ViewBag.Message = "❌ Sai tài khoản hoặc mật khẩu!";
                }
            }
            return View(model);
        }

        // Đăng xuất
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}