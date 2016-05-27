using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Giqci.ApiProxy.App;
using Giqci.Chapi.Enums.App;
using Giqci.Chapi.Models.App;
using Giqci.Interfaces;
using Giqci.PublicWeb.Converters;
using Giqci.PublicWeb.Extensions;
using Giqci.PublicWeb.Models;
using Giqci.PublicWeb.Services;
using Ktech.Extensions;
using Ktech.Mvc.ActionResults;
using Newtonsoft.Json;

namespace Giqci.PublicWeb.Controllers.AuthorizeAjax
{
    [RoutePrefix("api")]
    [AjaxAuthorize]
    public class FormsController : AjaxController
    {
        private readonly IMerchantApplicationApiProxy _repo;
        private readonly IAuthService _auth;
        private readonly IApplicationViewModelApiProxy _appView;
        private readonly IApplicationCacheApiProxy _applicationCacheApiProxy;
        private readonly IFileApiProxy _fileApiProxy;

        public FormsController(IMerchantApplicationApiProxy repo, IAuthService auth,
            IApplicationViewModelApiProxy appView, IApplicationCacheApiProxy applicationCacheApiProxy,
            IFileApiProxy fileApiProxy)
        {
            _repo = repo;
            _auth = auth;
            _appView = appView;
            _applicationCacheApiProxy = applicationCacheApiProxy;
            _fileApiProxy = fileApiProxy;
        }

        [Route("forms/list")]
        [HttpPost]
        public ActionResult GetAllItem(string applyNo, ApplicationStatus? status, DateTime? start, DateTime? end,
            int pageIndex = 1, int pageSize = 10)
        {
            var auth = _auth.GetAuth();
            var model = _appView.Search(applyNo, auth.MerchantId, status, start, end, pageIndex, pageSize);
            var count = model.Count();
            return new KtechJsonResult(HttpStatusCode.OK, new { items = model, count = count },
                new JsonSerializerSettings { Converters = new List<JsonConverter> { new DescriptionEnumConverter() } });
        }

        [Route("forms/getstatus")]
        [HttpGet]
        public ActionResult GetStatus()
        {
            var statusValues =
                 Enum.GetValues(typeof(ApplicationStatus))
                     .Cast<ApplicationStatus>()
                     .Select(i => new { value = i.ToString(), name = i.ToDescription() })
                     .ToArray();
            return new KtechJsonResult(HttpStatusCode.OK, new { items = statusValues });
        }

        [Route("forms/savefile")]
        [HttpPost]
        public ActionResult UploadExampleFile(HttpPostedFileBase file)
        {
            var filePath = string.Format("/{0}/{1}", Config.Common.UserExampleFilePath, Guid.NewGuid().ToString("N"));
            var fileInfo = _fileApiProxy.UploadFile(file, filePath);
            return new KtechJsonResult(HttpStatusCode.OK, fileInfo);
        }

        [Route("forms/GetAppCache")]
        [HttpPost]
        public ActionResult GetAppCache()
        {
            var auth = _auth.GetAuth();
            if (auth == null)
            {
                FormsAuthentication.SignOut();
                return Redirect("~/account/login");
            }
            var result = _applicationCacheApiProxy.Get(auth.MerchantId, false);
            bool flag;
            Application app;
            try
            {
                app = JsonConvert.DeserializeObject<Application>(result.ApplicationJson);
                flag = true;
            }
            catch
            {
                flag = false;
                app = null;
            }
            return new KtechJsonResult(HttpStatusCode.OK, new { flag = flag, app = app }, new JsonSerializerSettings());
        }

        [Route("forms/SaveAppCache")]
        [HttpPost]
        public ActionResult SaveAppCache(Application app)
        {
            var auth = _auth.GetAuth();
            if (auth == null)
            {
                FormsAuthentication.SignOut();
                return Redirect("~/account/login");
            }
            _applicationCacheApiProxy.Add(auth.MerchantId, JsonConvert.SerializeObject(app));
            return new KtechJsonResult(HttpStatusCode.OK, new { });
        }

        [Route("forms/RemoveAppCache")]
        [HttpPost]
        public ActionResult RemoveAppCache()
        {
            var auth = _auth.GetAuth();
            if (auth == null)
            {
                FormsAuthentication.SignOut();
                return Redirect("~/account/login");
            }
            _applicationCacheApiProxy.Remove(auth.MerchantId);
            return new KtechJsonResult(HttpStatusCode.OK, new { });
        }
    }
}