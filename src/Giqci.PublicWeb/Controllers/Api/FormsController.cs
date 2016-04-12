using Giqci.PublicWeb.Converters;
using Ktech.Mvc.ActionResults;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Giqci.Chapi.Enums.App;
using Giqci.Interfaces;
using Giqci.PublicWeb.Helpers;
using Giqci.PublicWeb.Services;
using Ktech.Extensions;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("api")]
    [Authorize]
    public class FormsController : Controller
    {
        private readonly IMerchantApplicationApiProxy _repo;
        private readonly IAuthService _auth;
        private readonly IApplicationViewModelApiProxy _appView;

        public FormsController(IMerchantApplicationApiProxy repo, IAuthService auth,
            IApplicationViewModelApiProxy appView)
        {
            _repo = repo;
            _auth = auth;
            _appView = appView;
        }

        [Route("forms/list")]
        [HttpPost]
        public ActionResult GetAllItem(string applyNo, ApplicationStatus? status, DateTime? start, DateTime? end,
            int pageIndex = 1, int pageSize = 10)
        {
            var model = _appView.Search(applyNo, _auth.GetAuth().MerchantId, status, start, end, pageIndex, pageSize);
            var count = model.Count();
            return new KtechJsonResult(HttpStatusCode.OK, new {items = model, count = count},
                new JsonSerializerSettings {Converters = new List<JsonConverter> {new DescriptionEnumConverter()}});
        }

        [Route("forms/getstatus")]
        [HttpGet]
        public ActionResult GetStatus()
        {
            var statusValues =
                Enum.GetValues(typeof (ApplicationStatus))
                    .Cast<ApplicationStatus>()
                    .Select(i => new {value = i.ToString(), name = i.ToDescription()})
                    .ToArray();
            return new KtechJsonResult(HttpStatusCode.OK, new {items = statusValues});
        }

        [Route("forms/deleteexample")]
        [HttpPost]
        public ActionResult DeleteExample(string certFilePath)
        {
            var cookieHelper = new CookieHelper();
            var currentExampleStr = cookieHelper.GetExampleListStr();
            var newExampleStr = currentExampleStr.Replace(certFilePath, string.Empty);
            cookieHelper.OverrideCookies(cookieHelper.ExampleFileListKeyName, newExampleStr);
            return new KtechJsonResult(HttpStatusCode.OK, new {});
        }
    }
}