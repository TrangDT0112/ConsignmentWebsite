using ConsignmentWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ConsignmentWebsite.Areas.Admin.Controllers
{
    public class ChatController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // Hiển thị giao diện chat
        public ActionResult Index()
        {
            return PartialView();
        }

        // Lấy tất cả cuộc trò chuyện (mỗi session đại diện cho 1 khách hàng)
        public JsonResult GetAllChats()
        {
            var userLatestSessions = db.ChatMessages
                .Where(m => m.Sender != "Admin" && m.Sender != "Employee")
                .GroupBy(m => m.Sender)
                .Select(g => g.OrderByDescending(m => m.SentAt).FirstOrDefault())
                .OrderByDescending(m => m.SentAt)
                .ToList();

            var result = userLatestSessions.Select(m => new
            {
                SessionId = m.SessionId,
                DisplayName = m.Sender
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        // Lấy tin nhắn theo SessionId
        public JsonResult GetMessages(string senderName)
        {
            var messages = db.ChatMessages
                .Where(m => (m.Sender == "Admin" && m.SessionId != null &&
                                db.ChatMessages.Any(x => x.SessionId == m.SessionId && x.Sender == senderName))
                            || m.Sender == senderName)
                .Where(m => m.Sender != "System")
                .OrderBy(m => m.SentAt)
                .ToList();

            return Json(messages, JsonRequestBehavior.AllowGet);
        }


        // Gửi tin nhắn từ Admin
        [HttpPost]
        public JsonResult SendMessage(string message, string sessionId)
        {
            var msg = new ChatMessage
            {
                Sender = "Admin",
                Message = message,
                SessionId = sessionId,
                SentAt = DateTime.Now
            };

            db.ChatMessages.Add(msg);
            db.SaveChanges();

            return Json(new { success = true });
        }

        // Lấy 4 cuộc trò chuyện gần nhất từ các khách hàng thực sự (không lấy bot/hệ thống)
        public JsonResult GetRecentMessages()
        {
            // Lấy tất cả tin nhắn không phải hệ thống
            var allValidMessages = db.ChatMessages
                .Where(m =>
                    m.Sender != "System" &&
                    m.Message != "Thanks for your message. We will respond soon!")
                .ToList();

            // Group theo người dùng thực sự (không phải Admin)
            var latestByUser = allValidMessages
                .Where(m => m.Sender != "Admin")
                .GroupBy(m => m.Sender)
                .Select(g =>
                {
            // Lấy các tin nhắn có liên quan đến user này (cùng SessionId)
            var sessionIds = g.Select(x => x.SessionId).Distinct().ToList();

                    var messagesInSameSession = allValidMessages
                        .Where(x => sessionIds.Contains(x.SessionId))
                        .OrderByDescending(x => x.SentAt)
                        .FirstOrDefault();

                    return messagesInSameSession;
                })
                .Where(m => m != null)
                .OrderByDescending(m => m.SentAt)
                .Take(4)
                .ToList();

            var data = latestByUser.Select(m => new
            {
                Sender = m.Sender == "Admin" ? GetCustomerNameBySessionId(m.SessionId) : m.Sender,
                Message = m.Message,
                Time = m.SentAt.ToString("HH:mm dd/MM"),
                Avatar = "/Content/Clients/dist/img/anh-avatar-mac-dinh.jpg"
            });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // Hàm phụ lấy tên khách hàng từ SessionId
        private string GetCustomerNameBySessionId(string sessionId)
        {
            var userMsg = db.ChatMessages
                .Where(m => m.SessionId == sessionId && m.Sender != "Admin" && m.Sender != "System")
                .OrderBy(m => m.SentAt)
                .FirstOrDefault();

            return userMsg?.Sender ?? "Unknown";
        }

        public ActionResult ChatView()
        {
            return View(); 
        }
    }
}
