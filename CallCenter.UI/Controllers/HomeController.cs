using CallCenter.Model.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CallCenter.UI.Controllers
{
    public class HomeController : Controller
    {
        private ICallCenterService service = null;
        public HomeController(){}
        public HomeController(ICallCenterService service)
        {
            this.service = service;
        }
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
    }
}