using clinic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace clinic.Controllers
{
    public class ServiceController : Controller
    {
        dbDClinicDataContext db = new dbDClinicDataContext();
        // GET: Service
        public ActionResult Index(int? page, string searchString)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            ViewBag.Keyword = searchString;

            var serviceListQuery = db.services.Select(w => w);

            if (!string.IsNullOrEmpty(searchString))
            {
                serviceListQuery = serviceListQuery.Where(w =>w.service_name.Contains(searchString));
            }

            var serviceList = serviceListQuery.OrderBy(w => w.service_name).ToPagedList(pageNumber, pageSize);

            return View(serviceList);
        }
        public ActionResult Index_customer(int? page, string searchString)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            ViewBag.Keyword = searchString;

            var serviceListQuery = db.services.Select(w => w);

            if (!string.IsNullOrEmpty(searchString))
            {
                serviceListQuery = serviceListQuery.Where(w =>
                    w.service_name.Contains(searchString));
            }

            var weaponList = serviceListQuery.OrderBy(w => w.service_name).ToPagedList(pageNumber, pageSize);

            return View(weaponList);
        }
        public ActionResult Detail(int id)
        {
            var service = db.services.FirstOrDefault(w => w.service_id == id);
            if (service == null)
            {
                return HttpNotFound();
            }
            return View(service);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var service = db.services.FirstOrDefault(w => w.service_id == id);
            if (service == null)
            {
                return HttpNotFound();
            }
            return View(service);
        }

        [HttpPost]
        public ActionResult Edit(service updatedService) { 
            var service = db.services.FirstOrDefault(w => w.service_id == updatedService.service_id);
            if (service == null)
            {
                return HttpNotFound();
            }

            service.service_name = updatedService.service_name;
            service.price = updatedService.price;
            service.description = updatedService.description;
            service.update_date = updatedService.update_date;
            service.image_path = updatedService.image_path;

            db.SubmitChanges();

            return RedirectToAction("Index");
        }
    }
}