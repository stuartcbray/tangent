using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TangentWeb.Models;

namespace TangentWeb.Controllers
{
    public class HomeController : Controller
    {
        private TangentWebContext db = new TangentWebContext();

        //
        // GET: /Home/
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(db.TangentItems.ToList());
        }

        public ActionResult About()
        {
          return View();
        }


        public ActionResult Contact()
        {
          return View();
        }

        //
        // GET: /Home/Details/5

        public ActionResult Details(int id = 0)
        {
            TangentItem tangentitem = db.TangentItems.Find(id);
            if (tangentitem == null)
            {
                return HttpNotFound();
            }
            return View(tangentitem);
        }

        //
        // GET: /Home/Create
        //[Authorize(Roles = "canEdit")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Home/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TangentItem tangentitem)
        {
            if (ModelState.IsValid)
            {
                db.TangentItems.Add(tangentitem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tangentitem);
        }

        //
        // GET: /Home/Edit/5

        public ActionResult Edit(int id = 0)
        {
            TangentItem tangentitem = db.TangentItems.Find(id);
            if (tangentitem == null)
            {
                return HttpNotFound();
            }
            return View(tangentitem);
        }

        //
        // POST: /Home/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TangentItem tangentitem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tangentitem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tangentitem);
        }

        //
        // GET: /Home/Delete/5

        public ActionResult Delete(int id = 0)
        {
            TangentItem tangentitem = db.TangentItems.Find(id);
            if (tangentitem == null)
            {
                return HttpNotFound();
            }
            return View(tangentitem);
        }

        //
        // POST: /Home/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TangentItem tangentitem = db.TangentItems.Find(id);
            db.TangentItems.Remove(tangentitem);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}