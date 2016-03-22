﻿using System.Linq;
using System.Web.Mvc;
using Ktech.Mvc.ActionResults;
using System.Net;
using Giqci.ApiProxy.Services;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("api")]
    public class DictionaryController : Controller
    {
        private readonly IDictService _dict;

        public DictionaryController(IDictService dict)
        {
            _dict = dict;
        }

        [Route("dict/countries")]
        [HttpGet]
        public ActionResult GetCountries()
        {
            return new KtechJsonResult(HttpStatusCode.OK, new { items = _dict.GetCountries() });
        }

        [Route("dict/commonhscodes")]
        [HttpGet]
        public ActionResult GetCommonHSCodes(string q)
        {
            return new KtechJsonResult(HttpStatusCode.OK, new { items = _dict.SearchHSCodes(q, 20) });
        }

        [Route("dict/ports")]
        [HttpGet]
        public ActionResult GetPorts(string q)
        {
            return new KtechJsonResult(HttpStatusCode.OK, new { items = _dict.SearchPorts(q, 20) });
        }
    }
}