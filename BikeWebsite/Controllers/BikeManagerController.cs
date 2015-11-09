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
    public class BikeManagerController : Controller
    {
        private DBEntities db = new DBEntities();

        // GET: BikeManager
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.ProductCategory).Include(p => p.ProductModel);
            return View(products.ToList());
        }

        public JsonResult doesNameExist(string Name)
        {
            //check if any Product Names match the Name specified in the Parameter using the ANY extension method.  
            return Json(!db.Products.Any(x => x.Name == Name), JsonRequestBehavior.AllowGet);
        }

        public JsonResult doesProdNumExist(string ProductNumber)
        {
            //check if any Product Numbers match the ProductNumber specified in the Parameter using the ANY extension method.  
            return Json(!db.Products.Any(x => x.ProductNumber == ProductNumber), JsonRequestBehavior.AllowGet);
        }

        public string byteToImage(int? id)
        {
            Product product = db.Products.Find(id);
            if (product.ThumbNailPhoto != null)
            {
                byte[] imageData = db.Products.Find(id).ThumbNailPhoto;
                string imageBaseData = Convert.ToBase64String(imageData);
                string imageDataURL = string.Format("data:image/gif;base64,{0}", imageBaseData);
                return imageDataURL;
            }
            return "";
        }

        // GET: BikeManager/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            ViewBag.ThumbNailPhoto = byteToImage(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        public ActionResult getBikes(int? id)
        {
            var bikes = (from r in db.vProductAndDescriptions
                         where r.Culture == "en" && r.ProductCategoryID == id
                         select new { r.ProductModel, r.ProductModelID }).Distinct();
            SelectList list = new SelectList(bikes, "ProductModelID", "ProductModel");
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public SelectList getProductCategories()
        {
            var productCategories = from p in db.ProductCategories where p.ParentProductCategoryID == 1 select p;
            return new SelectList(productCategories, "ProductCategoryID", "Name");
        }

        public SelectList getProductModels()
        {
            var productModels = (from r in db.vProductAndDescriptions
                                where r.Culture == "en" && (r.ProductCategoryID == 5 || r.ProductCategoryID == 6 || r.ProductCategoryID == 7)
                                select new { r.ProductModel, r.ProductModelID }).Distinct();
             return new SelectList(productModels, "ProductModelID", "ProductModel");
        }

        // GET: BikeManager/Create
        public ActionResult Create()
        {
            ViewBag.ProductCategoryID = getProductCategories();
            ViewBag.ProductModelID = getProductModels();
            return View(new Product { SellStartDate = DateTime.Now });
        }

        // POST: BikeManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,Name,ProductNumber,Color,StandardCost,ListPrice,Size,Weight,ProductCategoryID,ProductModelID,SellStartDate,SellEndDate,DiscontinuedDate,ThumbNailPhoto,ThumbnailPhotoFileName,rowguid,ModifiedDate")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.ThumbNailPhoto = Convert.FromBase64String("R0lGODlhUAAxAPcAAAAAAIAAAACAAICAAAAAgIAAgACAgICAgMDAwP8AAAD/AP//AAAA//8A/wD//////wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMwAAZgAAmQAAzAAA/wAzAAAzMwAzZgAzmQAzzAAz/wBmAABmMwBmZgBmmQBmzABm/wCZAACZMwCZZgCZmQCZzACZ/wDMAADMMwDMZgDMmQDMzADM/wD/AAD/MwD/ZgD/mQD/zAD//zMAADMAMzMAZjMAmTMAzDMA/zMzADMzMzMzZjMzmTMzzDMz/zNmADNmMzNmZjNmmTNmzDNm/zOZADOZMzOZZjOZmTOZzDOZ/zPMADPMMzPMZjPMmTPMzDPM/zP/ADP/MzP/ZjP/mTP/zDP//2YAAGYAM2YAZmYAmWYAzGYA/2YzAGYzM2YzZmYzmWYzzGYz/2ZmAGZmM2ZmZmZmmWZmzGZm/2aZAGaZM2aZZmaZmWaZzGaZ/2bMAGbMM2bMZmbMmWbMzGbM/2b/AGb/M2b/Zmb/mWb/zGb//5kAAJkAM5kAZpkAmZkAzJkA/5kzAJkzM5kzZpkzmZkzzJkz/5lmAJlmM5lmZplmmZlmzJlm/5mZAJmZM5mZZpmZmZmZzJmZ/5nMAJnMM5nMZpnMmZnMzJnM/5n/AJn/M5n/Zpn/mZn/zJn//8wAAMwAM8wAZswAmcwAzMwA/8wzAMwzM8wzZswzmcwzzMwz/8xmAMxmM8xmZsxmmcxmzMxm/8yZAMyZM8yZZsyZmcyZzMyZ/8zMAMzMM8zMZszMmczMzMzM/8z/AMz/M8z/Zsz/mcz/zMz///8AAP8AM/8AZv8Amf8AzP8A//8zAP8zM/8zZv8zmf8zzP8z//9mAP9mM/9mZv9mmf9mzP9m//+ZAP+ZM/+ZZv+Zmf+ZzP+Z///MAP/MM//MZv/Mmf/MzP/M////AP//M///Zv//mf//zP///yH5BAEAABAALAAAAABQADEAAAj/AP8JHEiwoMGDCBMqXMiwocOHECNKnEixosWLGDNq3Mixo8ePIEOKHEmypMmTKFOqXJkRBYqBLhfGZPnQ5ct/MxPmpMnQpsCZNm/CfBnTZ86gQ3HeRMoRadGlQpUqJfoUZ9KnVH9GxVhUKtCoVaWKnZrVK9SmVMPuVHvWrFisPjd+LbuW7tmvb8t6nJuXIFutfbH2lSt07ta/eeOy3clTYuGtjS8yjUy5suXLmDHHdRjWIGPGIjdDBA3YL2SQVY+mvQsVL16yqLOqfuyWtlHZbTv+nY176G67H38DTs068GrSkoMSN+62+fKQqrW2Xe6aem7CSaf6fq7ceevTmcOLEh9Pvrz58+jTq1/Pvr379+8DAgA7");
                product.ThumbnailPhotoFileName = "no_image_available_small.gif";
                product.rowguid = Guid.NewGuid();
                product.ModifiedDate = DateTime.Now;
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductCategoryID = getProductCategories();
            ViewBag.ProductModelID = getProductModels();
            return View(product);
        }

        // GET: BikeManager/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductCategoryID = getProductCategories();
            ViewBag.ProductModelID = getProductModels();
            return View(product);
        }

        // POST: BikeManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,Name,ProductNumber,Color,StandardCost,ListPrice,Size,Weight,ProductCategoryID,ProductModelID,SellStartDate,SellEndDate,DiscontinuedDate,ThumbNailPhoto,ThumbnailPhotoFileName,rowguid,ModifiedDate")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductCategoryID = getProductCategories();
            ViewBag.ProductModelID = getProductModels();
            return View(product);
        }

        // GET: BikeManager/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: BikeManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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
