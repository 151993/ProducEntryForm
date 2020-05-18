using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OmniBridge_Task;
using PagedList;

namespace OmniBridge_Task.Controllers
{
    public class ProductsController : Controller
    {
        private TaskDBEntities db = new TaskDBEntities();

        // GET: Products
        public ActionResult Index(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Name_Description" : "";
            ViewBag.SortingDate = Sorting_Order == "Date_Purchase" ? "Date_Expiry" : "Date_Purchase";
            ViewBag.SortingDateForEx = Sorting_Order == "Date_Purchase" ? "Date_Expiry" : "Date_Expiry";

            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }

            ViewBag.FilterValue = Search_Data;

            var products = from stu in db.Products select stu;
            {
                if (!String.IsNullOrEmpty(Search_Data))
                {
                    products = products.Where(stu => stu.Name.ToUpper().Contains(Search_Data.ToUpper()));
                }
            }
            switch (Sorting_Order)
            {
                case "Name_Description":
                    products = products.OrderByDescending(stu => stu.Name);
                    break;
                case "Date_Purchase":
                    products = products.OrderByDescending(stu => stu.Purchase_Date);
                    break;
                case "Date_Expiry":
                    products = products.OrderByDescending(stu => stu.ExpiryDate);
                    break;
                default:
                    products = products.OrderBy(stu => stu.Name);
                    break;
            }
            int Size_Of_Page = 4;
            int No_Of_Page = (Page_No ?? 1);
            return View(products.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
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

        // GET: Products/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            bool ValidateName = db.Products.Where(x => x.Name == product.Name).Any();
            if (ValidateName)
            {
                ViewBag.ValidateName = "Product name is already exist";
            }
            else
            {
                if (ModelState.IsValid)
                {
                    db.Products.Add(product);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(product);
           
        }

        // GET: Products/Edit/5
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
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductId,Name,Price,Quantity,IsGSTApplicable,Purchase_Date,ExpiryDate,Color")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Products/Delete/5
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

        // POST: Products/Delete/5
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
