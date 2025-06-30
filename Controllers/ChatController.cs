using ConsignmentWebsite.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ConsignmentWebsite.Controllers
{
    public class ChatController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        // Gửi tin nhắn
        [HttpPost]
        public JsonResult SendMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return Json(new { success = false, message = "Message cannot be empty!" });
            }

            string senderName = User.Identity.IsAuthenticated ? User.Identity.Name : "Guest";
            string sessionId = Session.SessionID;

            var msg = new ChatMessage
            {
                Sender = senderName,
                Message = message,
                SessionId = sessionId,
                SentAt = DateTime.Now
            };

            db.ChatMessages.Add(msg);
            db.SaveChanges();

            // Trả lời tự động từ nhân viên
            var reply = new ChatMessage
            {
                Sender = "Employee",
                Message = "Thanks for your message. We will respond soon!",
                SessionId = sessionId,
                SentAt = DateTime.Now
            };

            db.ChatMessages.Add(reply);
            db.SaveChanges();

            return Json(new { success = true, reply = reply.Message });
        }

        // Lấy tin nhắn (theo tên người dùng nếu có, nếu không thì dùng session)
        public JsonResult GetMessages()
        {
            try
            {
                string currentSessionId = Session.SessionID;
                string restoredSessionId = Session["ChatSessionId"] as string;
                string senderName = User.Identity.IsAuthenticated ? User.Identity.Name : null;

                // Nếu người dùng đã đăng nhập và chưa có session gốc lưu lại thì lưu lại
                if (User.Identity.IsAuthenticated && Session["ChatSessionId"] == null)
                {
                    Session["ChatSessionId"] = currentSessionId;
                    restoredSessionId = currentSessionId;
                }

                // Dùng session gốc nếu có
                string effectiveSessionId = restoredSessionId ?? currentSessionId;

                var messages = db.ChatMessages
                    .Where(m =>
                        m.SessionId == effectiveSessionId ||
                        (senderName != null && (m.Sender == senderName || m.Sender == "Admin"))
                    )
                    .OrderBy(m => m.SentAt)
                    .ToList();

                if (messages.Count == 0)
                {
                    return Json(new { error = "No messages found for this session." }, JsonRequestBehavior.AllowGet);
                }

                var result = messages.Select(m => new
                {
                    m.Sender,
                    m.Message,
                    Time = m.SentAt.ToString("HH:mm dd/MM/yyyy")
                }).ToList();

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Error loading messages! " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        // Kiểm tra phản hồi từ Admin
        public JsonResult CheckAdminReply()
        {
            string sessionId = Session.SessionID;
            string senderName = User.Identity.IsAuthenticated ? User.Identity.Name : null;

            var hasAdminReply = db.ChatMessages
                .Where(m =>
                    m.Sender == "Admin" &&
                    (
                        (senderName != null && m.SessionId == sessionId && m.Sender != senderName) ||
                        (senderName == null && m.SessionId == sessionId)
                    )
                )
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefault();

            return Json(new
            {
                hasReply = hasAdminReply != null,
                latestReply = hasAdminReply?.Message,
                time = hasAdminReply?.SentAt.ToString("HH:mm dd/MM/yyyy")
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
