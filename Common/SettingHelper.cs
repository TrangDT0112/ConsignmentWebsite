using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConsignmentWebsite.Models;

namespace ConsignmentWebsite.Common
{
    public class SettingHelper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static string GetValue(string key)
        {
            var item = db.SettingSystems.SingleOrDefault(x => x.SettingKey == key);
            if (item != null)
            {
                return item.SettingValue;
            }
            return "";
        }
    }
}