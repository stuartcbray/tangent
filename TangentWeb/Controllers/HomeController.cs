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

        readonly TangentWebContext db;

        public HomeController()
        {
            db = new TangentWebContext();
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
          return View();
        }  

        public ActionResult Compose()
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
    }
}