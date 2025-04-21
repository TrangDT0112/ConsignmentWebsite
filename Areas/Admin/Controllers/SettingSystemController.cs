using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConsignmentWebsite.Models.EF;
using ConsignmentWebsite.Models;

namespace ConsignmentWebsite.Areas.Admin.Controllers
{
    public class SettingSystemController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/SettingSystem
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Partial_Setting()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult AddSetting(SettingSystemView set)
        {
            SettingSystem setting = null;
            var checkTitle = db.SettingSystems.FirstOrDefault(x => x.SettingKey.Contains("SettingTitle"));
            if(checkTitle == null)
            {
                setting = new SettingSystem();
                setting.SettingKey = "SettingTitle";
                setting.SettingValue = set.SettingTitle;
                db.SettingSystems.Add(setting);                
            }
            else 
            {
                checkTitle.SettingValue = set.SettingTitle;
                db.Entry(checkTitle).State = System.Data.Entity.EntityState.Modified;
            }
            //logo
            var checkLogo = db.SettingSystems.FirstOrDefault(x => x.SettingKey.Contains("SettingLogo"));
            if (checkLogo == null)
            {
                setting = new SettingSystem();
                setting.SettingKey = "SettingLogo";
                setting.SettingValue = set.SettingLogo;
                db.SettingSystems.Add(setting);
            }
            else
            {
                checkLogo.SettingValue = set.SettingLogo;
                db.Entry(checkLogo).State = System.Data.Entity.EntityState.Modified;
            }
            var checkEmail = db.SettingSystems.FirstOrDefault(x => x.SettingKey.Contains("SettingEmail"));
            if (checkEmail == null)
            {
                setting = new SettingSystem();
                setting.SettingKey = "SettingEmail";
                setting.SettingValue = set.SettingEmail;
                db.SettingSystems.Add(setting);
            }
            else
            {
                checkEmail.SettingValue = set.SettingEmail;
                db.Entry(checkEmail).State = System.Data.Entity.EntityState.Modified;
            }
            var checkHotline = db.SettingSystems.FirstOrDefault(x => x.SettingKey.Contains("SettingHotline"));
            if (checkHotline == null)
            {
                setting = new SettingSystem();
                setting.SettingKey = "SettingHotline";
                setting.SettingValue = set.SettingHotline;
                db.SettingSystems.Add(setting);
            }
            else
            {
                checkHotline.SettingValue = set.SettingHotline;
                db.Entry(checkHotline).State = System.Data.Entity.EntityState.Modified;
            }
            var TitleSeo = db.SettingSystems.FirstOrDefault(x => x.SettingKey.Contains("SettingTitleSeo"));
            if (TitleSeo == null)
            {
                setting = new SettingSystem();
                setting.SettingKey = "SettingTitleSeo";
                setting.SettingValue = set.SettingTitleSeo;
                db.SettingSystems.Add(setting);
            }
            else
            {
                TitleSeo.SettingValue = set.SettingTitleSeo;
                db.Entry(TitleSeo).State = System.Data.Entity.EntityState.Modified;
            }
            var DescripSEO = db.SettingSystems.FirstOrDefault(x => x.SettingKey.Contains("SettingDesSeo"));
            if (DescripSEO == null)
            {
                setting = new SettingSystem();
                setting.SettingKey = "SettingDesSeo";
                setting.SettingValue = set.SettingDesSeo;
                db.SettingSystems.Add(setting);
            }
            else
            {
                DescripSEO.SettingValue = set.SettingDesSeo;
                db.Entry(DescripSEO).State = System.Data.Entity.EntityState.Modified;
            }
            var keySEO = db.SettingSystems.FirstOrDefault(x => x.SettingKey.Contains("SettingKeySeo"));
            if (keySEO == null)
            {
                setting = new SettingSystem();
                setting.SettingKey = "SettingKeySeo";
                setting.SettingValue = set.SettingKeySeo;
                db.SettingSystems.Add(setting);
            }
            else
            {
                keySEO.SettingValue = set.SettingKeySeo;
                db.Entry(keySEO).State = System.Data.Entity.EntityState.Modified;
            }
            db.SaveChanges();
            return View("Partial_Setting");
        }
    }
}