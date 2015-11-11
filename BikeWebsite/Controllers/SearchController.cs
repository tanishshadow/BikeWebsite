using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BikeWebsite;

namespace BikeWebsite.Controllers
{
    public class SearchController : Controller
    {
        private DBEntities db = new DBEntities();

        // GET: Search
        public ActionResult Index()
        {
            var searchstring = "g";
            searchstring = searchstring.ToLower();
            var searchResult = (from v in db.vProductAndDescriptions
                                join c in db.ProductCategories on v.ProductCategoryID equals
                                    c.ProductCategoryID
                                where v.Culture == "en" && v.SellEndDate == null && (c.ParentProductCategoryID == 1
                                    || c.ParentProductCategoryID == 2) && 
                                    (c.ProductCategoryID == 2 || c.ProductCategoryID == 3 || c.ProductCategoryID == 4)
                                select v).ToList();
            searchResult = searchResult.Where(x => (x.Name.ToLower()).Contains(searchstring)).ToList();
            return View(searchResult);
        }

        public ActionResult Result(string searchstring)
        {
            searchstring.ToLower();
            var searchResult = (from v in db.vProductAndDescriptions
                                join c in db.ProductCategories on v.ProductCategoryID equals
                                    c.ProductCategoryID
                                where v.Culture == "en" && v.SellEndDate == null && (c.ParentProductCategoryID == 1
                                    || c.ParentProductCategoryID == 2)
                                select v).ToList();
            searchResult = searchResult.Where(x => (x.Name.ToLower()).Contains(searchstring)).ToList();
            return View(searchResult);
        }


       
    }
}
