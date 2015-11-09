using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CaptchaMvc.HtmlHelpers;
using BikeWebsite.Models;
using System.Net;
using System.Net.Mail;

namespace BikeWebsite.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ContactUs()
        {
            return View();
        }

        public ActionResult Success()
        {
            return View();
        }


        [HttpPost]
        public ActionResult ContactUs(string fName, string lName, string email, string comment)
        {
            if (this.IsCaptchaValid("Captcha is not valid"))
            {
                ViewBag.ErrorMessage = "Valid";
                try
                {
                    MailMessage msg = new MailMessage(email, "milestoneCS516BikeShopTest@gmail.com");
                    msg.From = new MailAddress(email, fName + " " + lName);
                    SmtpClient smtp = new SmtpClient();
                    msg.ReplyToList.Add(email);
                    msg.Subject = "Contact Us on " + DateTime.Now;
                    msg.Body = "First Name: " + fName + "\nLast Name: " + lName +
                        "\nEmail: " + @email + "\nComment: " + comment;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Credentials = new NetworkCredential("milestoneCS516BikeShopTest@gmail.com", "amazingBikeShopTest");

                    smtp.Send(msg);
                    msg.Dispose();
                   
                    return RedirectToAction("Success");
                }
                catch (Exception)
                {
                    ViewBag.ErrorMessage = "Error sending Email, try again later";
                    ViewData["fName"] = fName;
                    ViewData["lName"] = lName;
                    ViewData["email"] = email;
                    ViewData["comment"] = comment;
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Error: captcha is not valid.";
                ViewData["fName"] = fName;
                ViewData["lName"] = lName;
                ViewData["email"] = email;
                ViewData["comment"] = comment;
            }
            return View();
        }
    }
}
