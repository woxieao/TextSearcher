using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Giqci.PublicWeb.Extensions;

namespace Giqci.PublicWeb.Controllers
{
    public class CertificateController : Controller
    {

        [Route("{languageType}/certificate/search")]
        [HttpGet]
        [BaseAuthorize]
        public ActionResult CertificateSearch()
        {
            return View();
        }
    }
}