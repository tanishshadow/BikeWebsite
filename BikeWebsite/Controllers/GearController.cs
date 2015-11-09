using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BikeWebsite;
using BikeWebsite.Models;

namespace BikeWebsite.Controllers
{
    public class GearController : Controller
    {
        private DBEntities db = new DBEntities();

        // GET: Gear
        public ActionResult Index()
        {
            //var productCategories = db.ProductCategories.Include(p => p.ProductCategory2);
            var productCategories = from d in db.ProductCategories
                                    where d.ProductCategoryID == 2 || d.ProductCategoryID == 3
                                        || d.ProductCategoryID == 4
                                    select d;
            return View(productCategories.ToList());
        }

        public ActionResult Components()
        {
            var components = getResults(2);
            return View(components.ToList());
        }

        public ActionResult Clothing()
        {
            var clothing = getResults(3);
            return View(clothing.ToList());
        }

        public ActionResult Accessories()
        {
            var accessories = getResults(4);
            return View(accessories.ToList());
        }

        public IQueryable<ProductCategory> getResults(int parentProdCatID)
        {
            var results = (from p in db.ProductCategories
                           join v in db.vProductAndDescriptions on p.ProductCategoryID equals v.ProductCategoryID
                           where v.Culture == "en" && v.SellEndDate == null && p.ParentProductCategoryID == parentProdCatID
                           select p).Distinct();

            return results;
        }

        // GET: Gear/Details/5
        public ActionResult Details(string id = null)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //ProductCategory productCategory = db.ProductCategories.Find(id);
            //if (productCategory == null)
            //{
            //    return HttpNotFound();
            //}
            var details = from d in db.Products where d.ProductCategoryID.ToString() == id && d.SellEndDate == null select d;
            return View(details.ToList());
        }

        // GET: Gear/Create
        public ActionResult Create()
        {
            ViewBag.ParentProductCategoryID = new SelectList(db.ProductCategories, "ProductCategoryID", "Name");
            return View();
        }

        // POST: Gear/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductCategoryID,ParentProductCategoryID,Name,rowguid,ModifiedDate")] ProductCategory productCategory)
        {
            if (ModelState.IsValid)
            {
                db.ProductCategories.Add(productCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ParentProductCategoryID = new SelectList(db.ProductCategories, "ProductCategoryID", "Name", productCategory.ParentProductCategoryID);
            return View(productCategory);
        }

        // GET: Gear/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCategory productCategory = db.ProductCategories.Find(id);
            if (productCategory == null)
            {
                return HttpNotFound();
            }
            ViewBag.ParentProductCategoryID = new SelectList(db.ProductCategories, "ProductCategoryID", "Name", productCategory.ParentProductCategoryID);
            return View(productCategory);
        }

        // POST: Gear/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductCategoryID,ParentProductCategoryID,Name,rowguid,ModifiedDate")] ProductCategory productCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ParentProductCategoryID = new SelectList(db.ProductCategories, "ProductCategoryID", "Name", productCategory.ParentProductCategoryID);
            return View(productCategory);
        }

        // GET: Gear/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCategory productCategory = db.ProductCategories.Find(id);
            if (productCategory == null)
            {
                return HttpNotFound();
            }
            return View(productCategory);
        }

        // POST: Gear/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductCategory productCategory = db.ProductCategories.Find(id);
            db.ProductCategories.Remove(productCategory);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
