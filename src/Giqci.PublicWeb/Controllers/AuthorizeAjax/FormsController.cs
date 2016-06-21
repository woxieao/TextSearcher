using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Giqci.ApiProxy.App;
using Giqci.ApiProxy.Dict;
using Giqci.Chapi.Enums.App;
using Giqci.Chapi.Models.App;
using Giqci.Interfaces;
using Giqci.PublicWeb.Converters;
using Giqci.PublicWeb.Extensions;
using Giqci.PublicWeb.Models;
using Giqci.PublicWeb.Models.Ajax;
using Giqci.PublicWeb.Services;
using Ktech.Api;
using Ktech.Extensions;
using Ktech.Mvc.ActionResults;
using Newtonsoft.Json;

namespace Giqci.PublicWeb.Controllers.AuthorizeAjax
{
    [RoutePrefix("api")]
    [AjaxAuthorize]
    public class FormsController : AjaxController
    {
        private readonly IAuthService _auth;
        private readonly IApplicationViewModelApiProxy _appView;
        private readonly IApplicationCacheApiProxy _applicationCacheApiProxy;
        private readonly IFileApiProxy _fileApiProxy;
        private readonly IDictService _cache;
        private readonly IMerchantApplicationApiProxy _appRepo;
        private readonly IProductApiProxy _prodApi;
        private readonly IDataChecker _dataChecker;
        private readonly IMerchantApiProxy _merchantRepo;
        private readonly IZcodeApplyLogApiProxy _zCodeApiProxy;

        public FormsController(IAuthService auth, IApplicationViewModelApiProxy appView,
            IApplicationCacheApiProxy applicationCacheApiProxy,
            IFileApiProxy fileApiProxy, IDictService cache,
            IMerchantApplicationApiProxy appRepo, IProductApiProxy prodApi,
            IDataChecker dataChecker, IMerchantApiProxy merchantRepo, IZcodeApplyLogApiProxy zCodeApiProxy)
        {
            _auth = auth;
            _appView = appView;
            _applicationCacheApiProxy = applicationCacheApiProxy;
            _fileApiProxy = fileApiProxy;
            _cache = cache;
            _appRepo = appRepo;
            _prodApi = prodApi;
            _dataChecker = dataChecker;
            _merchantRepo = merchantRepo;
            _zCodeApiProxy = zCodeApiProxy;
        }

        [Route("forms/list")]
        [HttpPost]
        public ActionResult GetAllItem(string applyNo, ApplicationStatus? status, DateTime? start, DateTime? end,
            int pageIndex = 1, int pageSize = 10)
        {

            var model = _appView.Search(applyNo, _auth.GetAuth().MerchantId, status, start, end, pageIndex, pageSize, false);
            var count = model.Count();
            return new AjaxResult(new { items = model, count = count }, new JsonSerializerSettings { Converters = new List<JsonConverter> { new DescriptionEnumConverter() } });
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
            return new AjaxResult(new { items = statusValues });
        }

        [Route("forms/savefile")]
        [HttpPost]
        public ActionResult UploadExampleFile(HttpPostedFileBase file)
        {
            var filePath = string.Format("/{0}/{1}", Config.Common.UserExampleFilePath, Guid.NewGuid().ToString("N"));
            var fileInfo = _fileApiProxy.UploadFile(file, filePath);
            return new AjaxResult(fileInfo);
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
            return new AjaxResult(new { flag = flag, app = app }, new JsonSerializerSettings());
        }

        [Route("forms/SaveAppCache")]
        [HttpPost]
        public ActionResult SaveAppCache(Application app)
        {
            _applicationCacheApiProxy.Add(_auth.GetAuth().MerchantId, JsonConvert.SerializeObject(app));
            return new AjaxResult(new { });
        }

        [Route("forms/RemoveAppCache")]
        [HttpPost]
        public ActionResult RemoveAppCache()
        {
            _applicationCacheApiProxy.Remove(_auth.GetAuth().MerchantId);
            return new AjaxResult(new { });
        }

        [Route("forms/app")]
        [HttpPost]
        public ActionResult SubmitApplication(Application model)
        {
            var appkey = model.Key;
            var isNew = string.IsNullOrEmpty(appkey);
            bool isRequireCiqCode = false;
            if (!string.IsNullOrEmpty(model.DestPort))
            {
                var port = _cache.GetPort(model.DestPort);
                isRequireCiqCode = port.RequireCiqCode;
            }
            var errors = _dataChecker.ApplicationHasErrors(model, false, isRequireCiqCode);
            var productList = model.ApplicationProducts;
            if (productList != null)
            {
                foreach (var product in productList)
                {
                    if (product.HandlerType == HandlerType.Add && string.IsNullOrEmpty(product.CiqCode) && product.CustomProductId == 0)
                    {
                        errors.Add("请选择商品");
                    }
                }
            }
            if (isNew)
            {
                if (!string.IsNullOrEmpty(model.Voyage) && DateTime.Parse(model.Voyage) < DateTime.Now.Date)
                {
                    errors.Add("出发日期应大于等于当前时间");
                }
                if (model.InspectionDate < DateTime.Now.Date)
                {
                    errors.Add("预约检查日期需大于等于今天");
                }
                if (!string.IsNullOrEmpty(model.ShippingDate) && DateTime.Parse(model.ShippingDate) < DateTime.Now.Date)
                {
                    errors.Add("计划发货日期应大于等于当前时间");
                }
                if (string.IsNullOrEmpty(model.InspectionAddr))
                {
                    errors.Add("检验地点不能为空");
                }
                if (string.IsNullOrEmpty(model.Inspector))
                {
                    errors.Add("联系人不能为空");
                }
                if (string.IsNullOrEmpty(model.InspectorTel))
                {
                    errors.Add("联系人电话不能为空");
                }
            }

            errors = errors.Distinct().OrderBy(i => i.Length).ToList();
            var isLogin = true;
            var merchant = _merchantRepo.GetMerchant(User.Identity.Name);
            var merchantId = merchant.Id;
            if (!errors.Any())
            {
                _prodApi.UpdateCiqProductInfo(model.ApplicationProducts);
                GetTotalUnits(ref model);
                if (isNew)
                {

                    appkey = _appRepo.CreateApplication(merchantId, model);
                }
                else
                {
                    _appRepo.Update(merchantId, appkey, model);
                }
                errors = null;
            }
            return new AjaxResult(
                new { isNew = isNew, appkey = appkey, isLogin = isLogin, errors = errors });
        }
        private void GetTotalUnits(ref Application input)
        {
            input.TotalUnits = input.ShippingMethod == ShippingMethod.O
                ? (input.ContainerInfos == null ? 0 : input.ContainerInfos.Count())
                : input.TotalUnits;
        }

        [Route("forms/addzcodeapply")]
        [HttpPost]
        public ActionResult AddZcodeApply(ZcodeApplyLog log)
        {
            if (log.Count <= 0)
            {
                throw new AjaxException("真知码数量必须为正整数");
            }
            _zCodeApiProxy.SubmitNewApply(new ZcodeApplyLog
            {
                Count = log.Count,
                ZcodeType = log.ZcodeType,
                MerchantId = _auth.GetAuth().MerchantId
            });
            return new AjaxResult(new { flag = true });
        }
    }
}