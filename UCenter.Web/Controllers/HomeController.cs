using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UCenter.Common.Handler;

namespace UCenter.Web.Controllers
{
    [Export]
    public class HomeController : Controller
    {
        private AccountHandler handler;

        [ImportingConstructor]
        public HomeController(AccountHandler handler)
        {
            this.handler = handler;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}