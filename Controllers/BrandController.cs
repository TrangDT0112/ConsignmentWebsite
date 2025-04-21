using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConsignmentWebsite.Models.EF;
using ConsignmentWebsite.Models;

namespace ConsignmentWebsite.Controllers
{
    public class BrandController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Brand
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Partial_BrandPreferred()
        {
            var items = db.Brands.Where(x => x.IsPrefer && x.IsPrefer).Take(30).ToList();
            return PartialView(items);
        }
    }
}