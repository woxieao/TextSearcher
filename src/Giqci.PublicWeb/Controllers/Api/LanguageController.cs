using System.Net;
using System.Web.Mvc;
using Giqci.Interfaces;
using Giqci.PublicWeb.Extensions;
using Giqci.PublicWeb.Models;
using Giqci.PublicWeb.Models.Ajax;
using Giqci.PublicWeb.Models.Api;
using Ktech.Core.Mail;
using Ktech.Mvc.ActionResults;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("api/language")]
    public class LanguageController : AjaxController
    {

        private readonly IDictService _dict;

        public LanguageController(IDictService dict)
        {
            _dict = dict;
        }
        [Route("getallwords")]
        [HttpGet]
        public ActionResult GetAllWords()
        {
            return new AjaxResult(_dict.GetAllLanguage());
        }
    }
}