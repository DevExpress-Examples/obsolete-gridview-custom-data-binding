using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Example.Controllers {
    public class HomeController : Controller {
        public ActionResult Index() {
            ViewBag.Message = "Welcome to DevExpress Extensions for ASP.NET MVC!";

            DataObject obj = new DataObject();

            return View(obj);
        }

        public ActionResult GridViewPartial() {
            DataObject obj = new DataObject();

            return PartialView(obj);
        }
    }
}
