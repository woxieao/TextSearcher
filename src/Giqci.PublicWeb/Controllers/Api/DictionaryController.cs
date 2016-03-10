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
            string query = string.IsNullOrEmpty(Request.QueryString.Get("q")) ? "" : Request.QueryString.Get("q");
            return new KtechJsonResult(HttpStatusCode.OK, new { items = _repoDictionary.FindCountryDictionary(query) });
        }

        [Route("dict/commonhscodes")]
        [HttpGet]
        public ActionResult GetCommonHSCodes()
        {
            string query = string.IsNullOrEmpty(Request.QueryString.Get("q")) ? "" : Request.QueryString.Get("q");
            return new KtechJsonResult(HttpStatusCode.OK, new { items = _repoDictionary.FindHSCodeByName(query) });
        }

        [Route("dict/loadingports")]
        [HttpGet]
        public ActionResult GetLoadingPorts()
        {
            string query = string.IsNullOrEmpty(Request.QueryString.Get("q")) ? "" : Request.QueryString.Get("q");
            return new KtechJsonResult(HttpStatusCode.OK, new { items = _repoDictionary.FindLoadingPortsByName(query) });
        }

        [Route("dict/destports")]
        [HttpGet]
        public ActionResult GetDestPorts()
        {
            string query = string.IsNullOrEmpty(Request.QueryString.Get("q")) ? "" : Request.QueryString.Get("q");
            return new KtechJsonResult(HttpStatusCode.OK, new { items = _repoDictionary.FindDestPortsByName(query) });
        }
    }
}