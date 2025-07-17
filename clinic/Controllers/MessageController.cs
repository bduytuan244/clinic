using clinic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace clinic.Controllers
{
    public class MessageController : Controller
    {
        dbDClinicDataContext db = new dbDClinicDataContext();
        // GET: Message
        public JsonResult GetAdmins()
        {
            var admins = db.admins
                .Select(a => new { a.doctor_id, a.username })
                .ToList();

            return Json(admins, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            var messages = db.chats.OrderByDescending(m => m.sent_at).ToList();
            return View(messages);
        }

        public JsonResult GetMessages(int receiver_id)
        {
            int sender_id = (Session["User"] as customer)?.patient_id ?? 0;
            var messages = db.chats
                .Where(m => (m.sender_id == sender_id && m.receiver_id == receiver_id) ||
                            (m.sender_id == receiver_id && m.receiver_id == sender_id))
                .OrderBy(m => m.sent_at)
                .Select(m => new
                {
                    m.sender_id,
                    sender_type = m.sender_role,
                    sender_name = (m.sender_role == "customer") ?
                        db.customers.Where(c => c.patient_id == m.sender_id).Select(c => c.full_name).FirstOrDefault()
                        : db.admins.Where(a => a.doctor_id == m.sender_id).Select(a => a.username).FirstOrDefault(),
                    m.message_text,
                    sent_at = m.sent_at.ToString()
                }).ToList();

            return Json(messages, JsonRequestBehavior.AllowGet);
        }

        // 🔹 Gửi tin nhắn
        [HttpPost]
        public JsonResult SendMessage(int receiver_id, string message_text)
        {
            try
            {
                int sender_id;
                string sender_type;
                string receiver_type;

                if (Session["Admin"] != null)
                {
                    sender_id = (Session["Admin"] as admin)?.doctor_id ?? 0;
                    sender_type = "admin";
                    receiver_type = "customer";
                }
                else if (Session["User"] != null)
                {
                    sender_id = (Session["User"] as customer)?.patient_id ?? 0;
                    sender_type = "customer";
                    receiver_type = "admin";
                }
                else
                {
                    return Json(new { success = false, error = "❌ Người dùng chưa đăng nhập!" });
                }

                if (receiver_id == 0 || string.IsNullOrWhiteSpace(message_text))
                {
                    return Json(new { success = false, error = "❌ Thiếu thông tin gửi tin nhắn!" });
                }

                Talk newMessage = new Talk
                {
                    sender_id = sender_id,
                    receiver_id = receiver_id,
                    sender_role = sender_type,
                    receiver_role = receiver_type,
                    message_text = message_text,
                    sent_at = DateTime.Now,
                    is_read = false
                };

                db.chats.InsertOnSubmit(newMessage);
                db.SubmitChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "Lỗi server: " + ex.Message });
            }
        }


        public JsonResult GetCustomersWhoMessaged()
        {
            var customers = db.chats
                .Where(m => m.sender_role == "customer")
                .Select(m => new
                {
                    customer_id = m.sender_id,
                    customer_name = db.customers.Where(c => c.patient_id == m.sender_id).Select(c => c.full_name).FirstOrDefault()
                })
                .Distinct()
                .ToList();

            return Json(customers, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCustomers()
        {
            var customers = db.chats
                .Where(m => m.receiver_role == "admin")
                .Select(m => new
                {
                    m.sender_id,
                    sender_name = db.customers.Where(c => c.patient_id == m.sender_id)
                                              .Select(c => c.full_name)
                                              .FirstOrDefault()
                })
                .Distinct()
                .ToList();

            return Json(customers, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUserRole()
        {
            if (Session["Admin"] != null)
            {
                return Json(new { role = "admin" }, JsonRequestBehavior.AllowGet);
            }
            else if (Session["User"] != null)
            {
                return Json(new { role = "customer" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { role = "guest" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AdminChat()
        {
            return View();
        }
        public JsonResult GetRecentMessages()
        {
            var recentMessages = db.chats
                .Where(m => m.sender_role == "customer")
                .GroupBy(m => m.sender_id)
                .Select(group => new
                {
                    customer_id = group.Key,
                    customer_name = db.customers.Where(c => c.patient_id == group.Key)
                                                .Select(c => c.full_name).FirstOrDefault(),
                    last_message = group.OrderByDescending(m => m.sent_at)
                                        .Select(m => m.message_text).FirstOrDefault(),
                    last_sent_at = group.OrderByDescending(m => m.sent_at)
                                        .Select(m => m.sent_at).FirstOrDefault()
                })
                .OrderByDescending(m => m.last_sent_at)
                .ToList();

            return Json(recentMessages, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetChatsForCustomer()
        {
            int customerId = (Session["User"] as customer)?.patient_id ?? 0;
            if (customerId == 0)
            {
                return Json(new { success = false, error = "Người dùng chưa đăng nhập!" }, JsonRequestBehavior.AllowGet);
            }

            var chats = db.chats
                .Where(m => m.sender_id == customerId && m.receiver_role == "admin")
                .GroupBy(m => m.receiver_id)
                .Select(g => new
                {
                    chat_partner_id = g.Key,
                    chat_partner_name = db.admins
                        .Where(a => a.doctor_id == g.Key)
                        .Select(a => a.username)
                        .FirstOrDefault(),
                    last_message = g.OrderByDescending(m => m.sent_at).Select(m => m.message_text).FirstOrDefault(),
                    last_sent_at = g.OrderByDescending(m => m.sent_at).Select(m => m.sent_at).FirstOrDefault().ToString()
                })
                .ToList();

            return Json(chats, JsonRequestBehavior.AllowGet);
        }
    }
}