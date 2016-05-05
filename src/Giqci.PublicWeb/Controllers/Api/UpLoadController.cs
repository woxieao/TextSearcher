using System.Web.Mvc;
using System.Web;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("api")]
    public class UpLoadController : Controller
    {
        public UpLoadController()
        {
        }

        [Route("Upload")]
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            return null;
        }
    }
}