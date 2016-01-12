using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Giqci.PublicWeb.Controllers
{
    public class CertificateController : Controller
    {

        [Route("certificate/search")]
        [HttpGet]
        [Authorize]
        public ActionResult CertificateSearch()
        {
            return View();
        }
    }
}