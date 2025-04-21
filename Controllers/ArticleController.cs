using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConsignmentWebsite.Models;

namespace ConsignmentWebsite.Controllers
{
    public class ArticleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Article
        public ActionResult Index(string title)
        {
            var item = db.Posts.FirstOrDefault(x => x.Title == title);
            return View(item);
        }
    }
}