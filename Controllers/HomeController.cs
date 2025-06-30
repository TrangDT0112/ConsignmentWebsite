using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConsignmentWebsite.Services;
using ConsignmentWebsite.Models;
using ConsignmentWebsite.Models.EF;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Threading;

namespace ConsignmentWebsite.Controllers
{
    public class HomeController : Controller
    {
        private TinyLlamaService _tinyLlamaService = new TinyLlamaService();
        private ApplicationDbContext db = new ApplicationDbContext();

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
        public HomeController()
        {
            _tinyLlamaService = new TinyLlamaService(); // Cần inject service này nếu sử dụng DI
        }
        [HttpPost]
        public async Task<ActionResult> AskTinyLlama(string userPrompt)
        {
            try
            {
                var response = await _tinyLlamaService.GetResponseAsync(userPrompt);
                if (string.IsNullOrEmpty(response))
                {
                    return Json(new { response = "❌ No valid response from Chip." });
                }

                return Json(new { response });
            }
            catch (HttpRequestException httpEx)
            {
                return Json(new { response = $"❌ HTTP error: {httpEx.Message}" });
            }
            catch (Exception ex)
            {
                return Json(new { response = $"❌ Internal server error: {ex.Message}" });
            }
        }

        public ActionResult AskTinyLlama()
        {
            return PartialView();
        }

    }
}