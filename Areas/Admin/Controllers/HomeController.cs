﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConsignmentWebsite.Models.EF;
using ConsignmentWebsite.Models;

namespace ConsignmentWebsite.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Employee")]
    public class HomeController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }
        
    }
}