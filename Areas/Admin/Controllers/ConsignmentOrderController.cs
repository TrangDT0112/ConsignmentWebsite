using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using System.Web.Mvc;
using ConsignmentWebsite.Models.EF;
using ConsignmentWebsite.Models;

namespace ConsignmentWebsite.Areas.Admin.Controllers
{
    public class ConsignmentOrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Default
        public ActionResult Index(int? page)
        {
            var items = db.ConsignmentOrders.OrderByDescending(x => x.CreatedDate).ToList();

            foreach (var order in items)
            {
                var products = db.Products.Where(p => p.ConsignmentOrderId == order.Id).ToList();
                decimal totalPayout = 0;

                foreach (var p in products)
                {
                    if (p.Quantity == 0)
                    {
                        decimal commissionRate = p.BrandId == 22 ? 0.3m : 0.25m;
                        totalPayout += 1 * p.Price * (1 - commissionRate);
                    }
                }

                // Gán lại và lưu
                order.TotalAmount = totalPayout;
                db.Entry(order).Property(x => x.TotalAmount).IsModified = true;
            }

            db.SaveChanges(); 

            var pageNumber = page ?? 1;
            var pageSize = 10;
            ViewBag.PageSize = pageSize;
            ViewBag.Page = pageNumber;
            return View(items.ToPagedList(pageNumber, pageSize));
        }


        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ConsignmentOrder model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                db.ConsignmentOrders.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        public ActionResult View(int id)
        {
            var item = db.ConsignmentOrders.Find(id);
            if (item != null)
            {
                var products = db.Products.Where(x => x.ConsignmentOrderId == id).ToList();

                decimal totalAmount = 0;
                foreach (var p in products)
                {
                    if (p.Quantity == 0)
                    {
                        decimal commissionRate = p.BrandId == 22 ? 0.3m : 0.25m;
                        totalAmount += p.Price * (1 - commissionRate);
                    }
                }

                item.TotalAmount = totalAmount;
            }
            return View(item);
        }

        public ActionResult Partial_ConsignmentOrder(int id)
        {
            var items = db.Products.Where(x => x.ConsignmentOrderId == id).ToList();
            return PartialView(items);
        }
        [HttpPost]
        public ActionResult DeleteAll(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var obj = db.ConsignmentOrders.Find(Convert.ToInt32(item));
                        db.ConsignmentOrders.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public ActionResult UpdateStatus(int id, int status, int orderStatus)
        {
            var item = db.ConsignmentOrders.Find(id);
            if (item != null)
            {
                db.ConsignmentOrders.Attach(item);
                item.PaymentStatus = status;
                item.Status = orderStatus;
                db.Entry(item).Property(x => x.PaymentStatus).IsModified = true;
                db.Entry(item).Property(x => x.Status).IsModified = true;
                db.SaveChanges();
                return Json(new { message = "Success", success = true });
            }
            return Json(new { message = "Unsuccess", success = false });
        }


    }
}