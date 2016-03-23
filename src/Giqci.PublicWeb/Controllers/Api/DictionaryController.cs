using System.Web.Mvc;
using Ktech.Mvc.ActionResults;
using System.Net;
using Giqci.Interfaces;

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
            return new KtechJsonResult(HttpStatusCode.OK, new {items = _dict.GetCountries()});
        }

        [Route("dict/commonhscodes")]
        [HttpGet]
        public ActionResult GetCommonHSCodes(string code)
        {
            return new KtechJsonResult(HttpStatusCode.OK, new {items = _dict.SearchHSCodes(code, 20)});
        }

        [Route("dict/ports")]
        [HttpGet]
        public ActionResult GetPorts(string code)
        {
            return new KtechJsonResult(HttpStatusCode.OK, new {items = _dict.SearchPorts(code, 20)});
        }
    }
}