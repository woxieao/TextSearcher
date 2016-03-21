using System.Linq;
using System.Web.Mvc;
using Ktech.Mvc.ActionResults;
using System.Net;
using Giqci.ApiProxy.Services;
using Giqci.Services;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("api")]
    public class DictionaryController : Controller
    {
        private readonly IDictService _dict;
        private readonly ICacheService _cacheService;

        public DictionaryController(IDictService dict, ICacheService cacheService)
        {
            _dict = dict;
            _cacheService = cacheService;
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
        public ActionResult GetLoadingPorts(string q)
        {
            return new KtechJsonResult(HttpStatusCode.OK, new { items = _dict.SearchPorts(q, 20) });
        }
    }
}