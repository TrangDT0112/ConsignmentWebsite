using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConsignmentWebsite.Models;
using ConsignmentWebsite.Models.EF;


namespace ConsignmentWebsite.Areas.Admin.Controllers
{
    public class StatisticController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Statistic
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult GetStatistic(string fromDate, string toDate)
        {
            var query = from o in db.Orders
                        join od in db.OrderDetails on o.Id equals od.OrderId
                        join p in db.Products on od.ProductId equals p.Id
                        select new
                        {
                            CreatedDate = o.CreatedDate,
                            Quantity = od.Quantity,
                            SellPrice = od.Price,
                            OriginalPrice = p.OriginalPrice,
                            BrandId = p.BrandId 
                        };

            //Filter by date
            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime startDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate >= startDate);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate < endDate.AddDays(1));
            }

            var result = query
                .AsEnumerable() // chuyển sang xử lý LINQ in-memory để dùng logic động
                .GroupBy(x => x.CreatedDate.Date)
                .Select(g =>
                {
                    var totalSell = g.Sum(x => x.Quantity * x.SellPrice);
                    var totalPayout = g.Sum(x =>
                    {
                        decimal commissionRate = x.BrandId == 22 ? 0.3m : 0.25m;
                        return x.Quantity * x.SellPrice * (1 - commissionRate);
                    });

                    return new
                    {
                        Date = g.Key,
                        Revenue = totalSell,
                        ConsignorPayout = totalPayout,
                        Profit = totalSell - totalPayout
                    };
                });

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }


    }
}