using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BikeWebsite.Controllers
{
    public class AccountController : Controller
    {
        private DBEntities db = new DBEntities();

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Models.User user)
        {
            if (ModelState.IsValid)
            {
                if (user.isValid(user.UserName, user.Password))
                {
                    var type = (from t in db.System_Users where t.Email == user.UserName select t.Type).ToList().First();
                    string Type = type.ToString();
                    user.Type = Type;
                    FormsAuthentication.SetAuthCookie(user.UserName, user.RememberMe);
                    Session["type"] = user.Type;
                    Session["name"] = user.UserName;
                    return RedirectToAction("Index", "Bikes");
                }
                else
                {
                    ModelState.AddModelError("", "Login is Incorrect");
                }
            }
            return View(user);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Bikes");
        }
    }
}