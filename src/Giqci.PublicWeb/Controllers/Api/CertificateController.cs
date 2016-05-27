using System.Net;
using System.Web.Mvc;
using Ktech.Mvc.ActionResults;
using System;
using System.Collections.Generic;
using Giqci.Interfaces;
using Newtonsoft.Json;
using Giqci.PublicWeb.Converters;
using Giqci.PublicWeb.Extensions;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("api")]
    public class CertificateController : AjaxController
    {
        private readonly ICertificateApiProxy _certRepo;

        public CertificateController(ICertificateApiProxy certRepo)
        {
            _certRepo = certRepo;
        }


        [Route("certificate/search")]
        [HttpPost]
        public ActionResult FormsSearch(string certNo)
        {
            //     var model = _certRepo.SearchCertificate(certNo);
            //todo appId=???
            var model = _certRepo.Get(string.Empty, certNo);
            return new KtechJsonResult(HttpStatusCode.OK, new {items = model},
                new JsonSerializerSettings {Converters = new List<JsonConverter> {new DescriptionEnumConverter()}});
        }
    }
}