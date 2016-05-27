using System.Linq;
using System.Web.Mvc;
using Ktech.Mvc.ActionResults;
using System.Net;
using Giqci.Interfaces;
using Giqci.PublicWeb.Extensions;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("api")]
    public class DictionaryController : AjaxController
    {
        private readonly IDictService _dict;

        public DictionaryController(IDictService dict)
        {
            _dict = dict;
        }

        [Route("dict/countries")]
        [HttpGet]
        public ActionResult GetCountries(string code)
        {
            return new KtechJsonResult(HttpStatusCode.OK, new { items = _dict.GetCountries().Where(x => string.IsNullOrEmpty(code) || x.Code == code || x.CnName.Contains(code)) });
        }

        [Route("dict/commonhscodes")]
        [HttpGet]
        public ActionResult GetCommonHSCodes(string code = "")
        {
            return new KtechJsonResult(HttpStatusCode.OK, new { items = _dict.SearchHSCodes(code, 20) });
        }

        [Route("dict/ports")]
        [HttpGet]
        public ActionResult GetPorts(string code = "")
        {
            return new KtechJsonResult(HttpStatusCode.OK, new { items = _dict.SearchPorts(code, 20) });
        }
    }
}