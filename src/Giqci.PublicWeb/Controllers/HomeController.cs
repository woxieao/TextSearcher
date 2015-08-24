using System.Web.Mvc;

namespace Giqci.PublicWeb.Controllers
{
    public class HomeController : Controller
    {
        [Route("")]
        public ActionResult Index()
        {
            return Redirect("/forms/app");
            //return View();
        }

        [Route("terms")]
        public ActionResult Terms()
        {
            return View();
        }
    }
}