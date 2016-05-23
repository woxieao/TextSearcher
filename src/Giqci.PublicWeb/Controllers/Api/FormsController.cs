using Giqci.PublicWeb.Converters;
using Ktech.Mvc.ActionResults;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Giqci.ApiProxy.App;
using Giqci.Chapi.Enums.App;
using Giqci.Chapi.Models.App;
using Giqci.Interfaces;
using Giqci.PublicWeb.Models;
using Giqci.PublicWeb.Services;
using Giqci.Tools;
using Ktech.Extensions;
using Newtonsoft.Json.Converters;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("api")]
    [Authorize]
    public class FormsController : Controller
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
            var model = _appView.Search(applyNo, _auth.GetAuth().MerchantId, status, start, end, pageIndex, pageSize);
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
            var result = _applicationCacheApiProxy.Get(_auth.GetAuth().MerchantId, false);
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
            _applicationCacheApiProxy.Add(_auth.GetAuth().MerchantId, JsonConvert.SerializeObject(app));
            return new KtechJsonResult(HttpStatusCode.OK, new { });
        }

        [Route("forms/RemoveAppCache")]
        [HttpPost]
        public ActionResult RemoveAppCache()
        {
            _applicationCacheApiProxy.Remove(_auth.GetAuth().MerchantId);
            return new KtechJsonResult(HttpStatusCode.OK, new { });
        }
    }
}