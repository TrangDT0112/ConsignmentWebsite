﻿using System;
using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConsignmentWebsite.Models.EF;
using ConsignmentWebsite.Models;

namespace ConsignmentWebsite.Areas.Admin.Controllers
{
    public class PurchaseOrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Order
        public ActionResult Index(int? page)
        {
            var items = db.Orders.OrderByDescending(x => x.CreatedDate).ToList();
            if (page == null) 
            {
                page = 1;
            }
            var pageNumber = page ?? 1;
            var pageSize = 10;
            ViewBag.PageSize = pageSize;
            ViewBag.Page = pageNumber;
            return View(items.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult View(int id)
        {
            var item = db.Orders.Find(id);
           
            return View(item);
        }
        public ActionResult Partial_Product(int id)
        {
            var items = db.OrderDetails.Where(x=> x.OrderId == id);

            return PartialView(items);
        }
        public ActionResult UpdateStatus(int id, int status)
        {
            var item = db.Orders.Find(id);
            if(item != null)
            {
                db.Orders.Attach(item);
                item.TypePayment = status;
                db.Entry(item).Property(x => x.TypePayment).IsModified = true;
                db.SaveChanges();
                return Json(new { message = "Success", success = true });
            }
            return Json(new { message = "Unsuccess", success = false });
        }
    }
}