using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConsignmentWebsite.Services;
using ConsignmentWebsite.Models;
using ConsignmentWebsite.Models.EF;
using System.Threading.Tasks;

namespace ConsignmentWebsite.Controllers
{
    [Authorize(Roles = "Customer")]
    public class HomeController : Controller
    {
        private readonly AiChatService gpt = new AiChatService();
        private ApplicationDbContext db = new ApplicationDbContext();
        public HomeController()
        {
            gpt = new AiChatService();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Refresh()
        {
            var item = new StatisticModel();

            ViewBag.Visitors_online = HttpContext.Application["visitors_online"];
            var td = HttpContext.Application["Today"];
            item.Today = HttpContext.Application["Today"].ToString();
            item.Yesterday = HttpContext.Application["Yesterday"].ToString();
            item.ThisWeek = HttpContext.Application["ThisWeek"].ToString();
            item.LastWeek = HttpContext.Application["LastWeek"].ToString();
            item.ThisMonth = HttpContext.Application["ThisMonth"].ToString();
            item.LastMonth = HttpContext.Application["LastMonth"].ToString();
            item.Total = HttpContext.Application["Total"].ToString();
            return PartialView(item);
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult AskChatGpt()
        {
            return View(gpt);
        }
        [HttpPost]
        public async Task<JsonResult> AskChatGpt(string userInput)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userInput))
                {
                    return Json(new { response = "❌ Missing user input!" });
                }

                var response = await gpt.AskChatGptAsync(userInput);
                return Json(new { response });
            }
            catch (System.Exception ex)
            {
                return Json(new { response = $"❌ Server error: {ex.Message}" });
            }
        }

    }
}