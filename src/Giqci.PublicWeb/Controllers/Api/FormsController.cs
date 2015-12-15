using System.Net;
using System.Web.Mvc;
using Giqci.Repositories;
using Ktech.Mvc.ActionResults;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Giqci.Enums;
using Giqci.PublicWeb.Converters;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("api")]
    public class FormsController : Controller
    {
        private readonly ICertificateRepository _certRepo;

        public FormsController(ICertificateRepository certRepo)
        {
            _certRepo = certRepo;
        }


        [Route("forms/search")]
        [HttpPost]
        public ActionResult FormsSearch(string certNo)
        {
            var model = _certRepo.SearchCertificate(certNo);

            //return new KtechJsonResult(HttpStatusCode.OK, new { items = model });
            return new KtechJsonResult(HttpStatusCode.OK, new { items = model }, new JsonSerializerSettings { Converters = new List<JsonConverter> { new DescriptionEnumConverter() } });
        }
    }
}