using clinic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace clinic.Controllers
{
    public class FormController : Controller
    {
        dbDClinicDataContext db = new dbDClinicDataContext();
        // GET: Form
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Create()
        {


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(appointmentform form)
        {
            if (ModelState.IsValid)
            {
                // Bỏ qua việc lấy thông tin người dùng
                form.status = "Pending";
                form.created_at = DateTime.Now;

                // Lưu form vào cơ sở dữ liệu
                db.appointmentforms.InsertOnSubmit(form);
                db.SubmitChanges();

                return RedirectToAction("Index", "Home");
            }

            return View(form);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            appointmentform form = db.appointmentforms.FirstOrDefault(f => f.appointment_id == id);
            if (form == null)
            {
                return HttpNotFound();
            }
            return View(form);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Approve(int id)
        {
            var form = db.appointmentforms.FirstOrDefault(f => f.appointment_id == id);
            if (form != null)
            {
                form.status = "Approved";
                db.SubmitChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Reject(int id)
        {
            var form = db.appointmentforms.FirstOrDefault(f => f.appointment_id == id);
            if (form != null)
            {
                form.status = "Rejected";
                db.SubmitChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Index_Admin()
        {
            var applications = db.appointmentforms.ToList();
            return View(applications);
        }


        public ActionResult Accept(int id)
        {
            var form = db.appointmentforms.FirstOrDefault(f => f.appointment_id == id);
            if (form != null)
            {
                form.status = "Accepted";
                db.SubmitChanges();
            }
            return RedirectToAction("Index_Admin");
        }


        public ActionResult Delete(int id)
        {
            var form = db.appointmentforms.FirstOrDefault(f => f.appointment_id == id);
            if (form != null)
            {
                db.appointmentforms.DeleteOnSubmit(form);
                db.SubmitChanges();
            }
            return RedirectToAction("Index_Admin");
        }
    }
}