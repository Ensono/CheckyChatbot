using System.Web.Mvc;

namespace Checky.Slack.AlertProxy.Controllers {
    public class HomeController : Controller {
        public ActionResult Index() {
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}