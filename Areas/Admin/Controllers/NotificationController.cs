using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConsignmentWebsite.Models;
using ConsignmentWebsite.Models.EF;

namespace ConsignmentWebsite.Areas.Admin.Controllers
{
    
    public class NotificationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Notification
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Partial_Notification()
        {
            return PartialView();
        }
        public JsonResult GetAdminNotifications()
        {
            // Lấy các phiên chat mà tin nhắn cuối cùng là từ User
            var unreadMessages = db.ChatMessages
                .GroupBy(m => m.SessionId)
                .Select(g => g.OrderByDescending(m => m.SentAt).FirstOrDefault())
                .Where(m => m.Sender != "Admin")
                .ToList();

            int newMessages = unreadMessages.Count;

            // Lấy thời gian của tin nhắn gần nhất từ User
            DateTime? lastMessageTime = unreadMessages
                .OrderByDescending(m => m.SentAt)
                .Select(m => (DateTime?)m.SentAt)
                .FirstOrDefault();

            // Đơn đặt hàng chờ xử lý
            int newOrders = db.Orders
                .Where(o => o.TypePayment == 0)
                .Count();

            // Đơn hàng ký gửi đã bán
            int newConsignOrders = db.ConsignmentOrders
                .Where(c => c.Status == 2) // Status == 2 nghĩa là Sold
                .Count();

            // Nếu chưa có tin nhắn nào, gán thời gian hiện tại
            var latestTime = lastMessageTime ?? DateTime.Now;

            return Json(new
            {
                newMessages = newMessages,
                lastMessageTime = latestTime.ToString("yyyy-MM-dd HH:mm:ss"),
                newOrders = newOrders,
                newConsignOrders = newConsignOrders
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
