﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Hosting;

namespace ConsignmentWebsite.Common
{
    public class Common
    {


        private static string password = ConfigurationManager.AppSettings["PasswordEmail"];
        private static string Email = ConfigurationManager.AppSettings["Email"];

        private static void LogSmtp(string message)
        {
            try
            {
                string logFilePath = @"D:\smtp_log.txt"; 
                string logDir = Path.GetDirectoryName(logFilePath);

                
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }

                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LogSmtp failed: " + ex.Message);
            }
        }
        public static bool SendMail(string fromEmail, string toEmail, string subject, string body)
        {
            LogSmtp("=== SendMail START ===");

            try
            {
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(fromEmail, "CiaraCycleFashion"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mail.To.Add(toEmail);

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromEmail, password)
                };

                LogSmtp($"Sending email from {fromEmail} to {toEmail}");
                smtp.Send(mail);
                LogSmtp("SendMail SUCCESS");

                return true;
            }
            catch (SmtpException smtpEx)
            {
                LogSmtp("SMTP EXCEPTION: " + smtpEx.ToString());
            }
            catch (Exception ex)
            {
                LogSmtp("GENERAL EXCEPTION: " + ex.ToString());
            }

            return false;
        }




        public static string FormatNumber(object value, int SoSauDauPhay = 2)
        {
            bool isNumber = IsNumeric(value);
            decimal GT = 0;
            if (isNumber)
            {
                GT = Convert.ToDecimal(value);
            }
            string str = "";
            string thapPhan = "";
            for (int i = 0; i < SoSauDauPhay; i++)
            {
                thapPhan += "#";
            }
            if (thapPhan.Length > 0) thapPhan = "." + thapPhan;
            string snumformat = string.Format("0:#,##0{0}", thapPhan);
            str = String.Format("{" + snumformat + "}", GT);

            return str;
        }
        private static bool IsNumeric(object value)
        {
            return value is sbyte
                       || value is byte
                       || value is short
                       || value is ushort
                       || value is int
                       || value is uint
                       || value is long
                       || value is ulong
                       || value is float
                       || value is double
                       || value is decimal;
        }
        public static string HtmlRate(int rate)
        {
            var str = "";
            if(rate == 1)
            {
                str = @"<li><i class='fa fa-star' aria-hidden='true'></i></li>
                       <li><i class='fa fa-star-o' aria-hidden='true'></i></li>
                       <li><i class='fa fa-star-o' aria-hidden='true'></i></li>
                       <li><i class='fa fa-star-o' aria-hidden='true'></i></li>
                       <li><i class='fa fa-star-o' aria-hidden='true'></i></li>";
            }
            if (rate == 2)
            {
                str = @"<li><i class='fa fa-star' aria-hidden='true'></i></li>
                       <li><i class='fa fa-star' aria-hidden='true'></i></li>
                       <li><i class='fa fa-star-o' aria-hidden='true'></i></li>
                       <li><i class='fa fa-star-o' aria-hidden='true'></i></li>
                       <li><i class='fa fa-star-o' aria-hidden='true'></i></li>";
            }
            if (rate == 3)
            {
                str = @"<li><i class='fa fa-star' aria-hidden='true'></i></li>
                       <li><i class='fa fa-star' aria-hidden='true'></i></li>
                       <li><i class='fa fa-star' aria-hidden='true'></i></li>
                       <li><i class='fa fa-star-o' aria-hidden='true'></i></li>
                       <li><i class='fa fa-star-o' aria-hidden='true'></i></li>";
            }
            if (rate == 4)
            {
                str = @"<li><i class='fa fa-star' aria-hidden='true'></i></li>
                       <li><i class='fa fa-star' aria-hidden='true'></i></li>
                       <li><i class='fa fa-star' aria-hidden='true'></i></li>
                       <li><i class='fa fa-star' aria-hidden='true'></i></li>
                       <li><i class='fa fa-star-o' aria-hidden='true'></i></li>";
            }
            if (rate == 5)
            {
                str = @"<li><i class='fa fa-star' aria-hidden='true'></i></li>
                       <li><i class='fa fa-star' aria-hidden='true'></i></li>
                       <li><i class='fa fa-star' aria-hidden='true'></i></li>
                       <li><i class='fa fa-star' aria-hidden='true'></i></li>
                       <li><i class='fa fa-star' aria-hidden='true'></i></li>";
            }
            return str;
        }

        
    }
}