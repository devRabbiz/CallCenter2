using CallCenter.Model.Abstract;
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