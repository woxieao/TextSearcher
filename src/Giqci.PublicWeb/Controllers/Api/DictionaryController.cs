using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Giqci.Enums;
using Giqci.PublicWeb.Converters;
using Giqci.Repositories;
using Ktech.Mvc.ActionResults;
using System.Net;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("api")]
    public class DictionaryController : Controller
    {
        private readonly IDictionaryRepository _repoDictionary;

        public DictionaryController(IDictionaryRepository repoDictionary)
        {
            _repoDictionary = repoDictionary;
        }

        [Route("dict/countries")]
        [HttpGet]
        public ActionResult GetCountries()
        {
            return new KtechJsonResult(HttpStatusCode.OK, new { items = _repoDictionary.GetCountryDictionary() });
        }

        [Route("dict/commonhscodes")]
        [HttpGet]
        public ActionResult GetCommonHSCodes()
        {
            return new KtechJsonResult(HttpStatusCode.OK, new { items = _repoDictionary.GetCommonHSCodes() });
        }

        [Route("dict/loadingports")]
        [HttpGet]
        public ActionResult GetLoadingPorts()
        {
            return new KtechJsonResult(HttpStatusCode.OK, new { items = _repoDictionary.GetLoadingPorts() });
        }

        [Route("dict/destports")]
        [HttpGet]
        public ActionResult GetDestPorts()
        {
            return new KtechJsonResult(HttpStatusCode.OK, new { items = _repoDictionary.GetDestPorts() });
        }
    }
}