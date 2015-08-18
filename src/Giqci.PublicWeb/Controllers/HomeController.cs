using System.Web.Mvc;

namespace Giqci.PublicWeb.Controllers
{
    public class HomeController : Controller
    {
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }
    }
}