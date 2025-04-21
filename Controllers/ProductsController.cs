using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConsignmentWebsite.Models;
using ConsignmentWebsite.Models.EF;

namespace ConsignmentWebsite.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Products
        public ActionResult Index(int? id)
        {
            var items = db.Products.ToList();
            if (id != null)
            {
                items = items.Where(x => x.ProductCategoryId == id).ToList();
            }
            return View(items);
        }
        public ActionResult Detail(string title,int id)
        {
            var items = db.Products.Find(id);
            if (items != null)
            {
                db.Products.Attach(items);
                items.ViewCount = items.ViewCount + 1;
                db.Entry(items).Property(x => x.ViewCount).IsModified = true;
                db.SaveChanges();
            }
                
            return View(items);
        }
        public ActionResult ProductCategory(string title ,int? id)
        {
            var items = db.Products.ToList();
            if (id > 0)
            {
                items = items.Where(x => x.ProductCategoryId == id).ToList();
            }
            var cate = db.ProductCategories.Find(id);
            if (cate != null)
            {
                ViewBag.CateName = cate.Title;
            }

            ViewBag.CateId = id;
            return View(items);
        }
        public ActionResult Partial_ItemByCateIdMenu()
        {
            var items = db.Products.Where(x => x.IsHome && x.IsActive).Take(15).ToList();
            return PartialView(items);
        }
         public ActionResult Partial_ProductSales()
        {
            var items = db.Products.ToList();
            return PartialView(items);
        }
        
    }
}