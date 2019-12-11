using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using cis237_assignment6.Models;

namespace cis237_assignment6.Controllers
{
    [Authorize] // Ensures that the user must be authenticated in the system.
    public class BeveragesController : Controller
    {
        private BeveragesContext db = new BeveragesContext();

        // GET: Beverages
        public ActionResult Index()
        {
            // Setup a variable to hold the cars data
            DbSet<Beverage> BeveragesToFilter = db.Beverages;

            // Setup some strings to hold the data that might be
            // in the session. 
            string filterName = "";
            string filterPack = "";
            string filterMin = "";
            string filterMax = "";
            // Define a min and max for the price
            int min = 0;
            int max = 100;


            // Check to see if there is a value in the session,
            // and if there is, assign it to the variable that
            // we setup to hold the value.
            if (!String.IsNullOrWhiteSpace(
                (string)Session["s_name"]
            ))
            {
                filterName = (string)Session["s_name"];
            }
            if (!String.IsNullOrWhiteSpace(
                (string)Session["s_pack"]
            ))
            {
                filterPack = (string)Session["s_pack"];
            }
            if (!String.IsNullOrWhiteSpace(
                (string)Session["s_min"]
            ))
            {
                filterMin = (string)Session["s_min"];
                min = Int32.Parse(filterMin);
            }
            if (!String.IsNullOrWhiteSpace(
                (string)Session["s_max"]
            ))
            {
                filterMax = (string)Session["s_max"];
                max = Int32.Parse(filterMax);
            }

            // Do the filter on the BeveragesToFilter Dataset.
            IEnumerable<Beverage> filtered = BeveragesToFilter.Where(
                car => car.price >= min &&
                car.price <= max &&
                car.name.Contains(filterName) &&
                car.pack.Contains(filterPack)
            );

            // Convert the dataset to a list now that the query
            // work is done on it. 
            IList<Beverage> finalFiltered = filtered.ToList();

            // Place the string representation of the values
            // that are in the session into the viewbag so
            // that they can be retrieved and displayed in the View.
            ViewBag.filterName = filterName;
            ViewBag.filterPack = filterPack;
            ViewBag.filterMin = filterMin;
            ViewBag.filterMax = filterMax;

            // Return the view with the filtered selection of beverages
            return View(finalFiltered);
        }

        // GET: Beverages/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // GET: Beverages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Beverages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,pack,price,active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                db.Beverages.Add(beverage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(beverage);
        }

        // GET: Beverages/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // POST: Beverages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,pack,price,active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(beverage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(beverage);
        }

        // GET: Beverages/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // POST: Beverages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Beverage beverage = db.Beverages.Find(id);
            db.Beverages.Remove(beverage);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Method to take in the filter data and store it in
        // the session. Will get used in the index method.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Filter()
        {
            // Get the form data that we sent out for the request object.
            string name = Request.Form.Get("name");
            string pack = Request.Form.Get("pack");
            string min = Request.Form.Get("min");
            string max = Request.Form.Get("max");

            // put it into the session so that other methods can have access to it.
            Session["s_name"] = name;
            Session["s_pack"] = pack;
            Session["s_min"] = min;
            Session["s_max"] = max;

            // Redirect to the index page
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
