using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using TangentWeb.Filters;
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

        [Authorize]
        public ActionResult Compose()
        {
          return View();
        }

        [Authorize]
        public ActionResult Claims()
        {
            ViewBag.Message = "Your claims page.";

            ViewBag.ClaimsIdentity = Thread.CurrentPrincipal.Identity;


            return View();
        }

        //
        // GET: /Home/Details/5
        [Authorize]
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