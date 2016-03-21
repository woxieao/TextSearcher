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
using Giqci.Services;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("api")]
    public class DictionaryController : Controller
    {
        private readonly IDictionaryRepository _repoDictionary;
        private readonly ICacheService _cacheService;

        public DictionaryController(IDictionaryRepository repoDictionary, ICacheService cacheService)
        {
            _repoDictionary = repoDictionary;
            _cacheService = cacheService;
        }

        [Route("dict/countries")]
        [HttpGet]
        public ActionResult GetCountries(string q)
        {
            return new KtechJsonResult(HttpStatusCode.OK,
                new {items = _cacheService.GetCache(_repoDictionary.FindCountryDictionary, q)});
        }

        [Route("dict/commonhscodes")]
        [HttpGet]
        public ActionResult GetCommonHSCodes(string q)
        {
            return new KtechJsonResult(HttpStatusCode.OK,
                new {items = _cacheService.GetCache(_repoDictionary.FindHSCodeByName, q)});
        }

        [Route("dict/loadingports")]
        [HttpGet]
        public ActionResult GetLoadingPorts(string q)
        {
            return new KtechJsonResult(HttpStatusCode.OK,
                new {items = _cacheService.GetCache(_repoDictionary.FindLoadingPortsByName, q)});
        }

        [Route("dict/destports")]
        [HttpGet]
        public ActionResult GetDestPorts(string q)
        {
            return new KtechJsonResult(HttpStatusCode.OK,
                new {items = _cacheService.GetCache(_repoDictionary.FindDestPortsByName, q)});
        }

        [Route("dict/countries/code")]
        [HttpGet]
        public ActionResult GetCountriesByCode(string code)
        {
            return new KtechJsonResult(HttpStatusCode.OK,
                new {items = _cacheService.GetCache(_repoDictionary.GetCountryDictionaryByCode, code)});
        }

        [Route("dict/commonhscodes/code")]
        [HttpGet]
        public ActionResult GetCommonHSCodesByCode(string code)
        {
            return new KtechJsonResult(HttpStatusCode.OK,
                new {items = _cacheService.GetCache(_repoDictionary.GetHSCode, code)});
        }

        [Route("dict/loadingports/code")]
        [HttpGet]
        public ActionResult GetLoadingPortsByCode(string code)
        {
            return new KtechJsonResult(HttpStatusCode.OK,
                new {items = _cacheService.GetCache(_repoDictionary.GetPort, code)});
        }

        [Route("dict/destports/code")]
        [HttpGet]
        public ActionResult GetDestPortsByCode(string code)
        {
            return new KtechJsonResult(HttpStatusCode.OK,
                new {items = _cacheService.GetCache(_repoDictionary.GetPort, code)});
        }
    }
}