using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Giqci.ApiProxy.App;
using Giqci.ApiProxy.Cust;
using Giqci.ApiProxy.Dict;
using Giqci.Chapi.Enums.App;
using Giqci.Chapi.Enums.Dict;
using Giqci.Chapi.Models.App;
using Giqci.Chapi.Models.Customer;
using Giqci.Interfaces;
using Giqci.PublicWeb.Converters;
using Giqci.PublicWeb.Extensions;
using Giqci.PublicWeb.Models;
using Giqci.PublicWeb.Models.Ajax;
using Giqci.PublicWeb.Services;
using Giqci.Validations;
using Ktech.Api;
using Ktech.Extensions;
using Ktech.Mvc.ActionResults;
using Newtonsoft.Json;

namespace Giqci.PublicWeb.Controllers.AuthorizeAjax
{
    [RoutePrefix("{languageType}/api")]
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
        private readonly IMerchantApiProxy _merchantRepo;
        private readonly IZcodeApplyLogApiProxy _zCodeApiProxy;


        public FormsController(IAuthService auth, IApplicationViewModelApiProxy appView,
            IApplicationCacheApiProxy applicationCacheApiProxy,
            IFileApiProxy fileApiProxy, IDictService cache,
            IMerchantApplicationApiProxy appRepo, IProductApiProxy prodApi,
             IMerchantApiProxy merchantRepo, IZcodeApplyLogApiProxy zCodeApiProxy)
        {
            _auth = auth;
            _appView = appView;
            _applicationCacheApiProxy = applicationCacheApiProxy;
            _fileApiProxy = fileApiProxy;
            _cache = cache;
            _appRepo = appRepo;
            _prodApi = prodApi;
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

            var errors = DataChecker.ApplicationHasErrors(model, isRequireCiqCode, _cache, LanCore.GetCurrentLanType());
            var productList = model.ApplicationProducts;
            if (productList != null)
            {
                foreach (var product in productList)
                {
                    if (product.HandlerType == HandlerType.Add && string.IsNullOrEmpty(product.CiqCode) && product.CustomProductId == 0)
                    {
                        errors.Add("email_is_not_exist".KeyToWord());
                    }
                }
            }
            if (isNew)
            {
                if (!string.IsNullOrEmpty(model.Voyage) && DateTime.Parse(model.Voyage) < DateTime.Now.Date)
                {
                    errors.Add("departure_date_should_ be_greater_than_now".KeyToWord());
                }
                if (model.InspectionDate < DateTime.Now.Date)
                {
                    errors.Add("check_date_should_be_greater_than_now".KeyToWord());
                }
                if (!string.IsNullOrEmpty(model.ShippingDate) && DateTime.Parse(model.ShippingDate) < DateTime.Now.Date)
                {
                    errors.Add("planned_delivery_date_should_be_greater_than_now".KeyToWord());
                }
                if (string.IsNullOrEmpty(model.InspectionAddr))
                {
                    errors.Add("inspection_site_can_not_be_empty".KeyToWord());
                }
                if (string.IsNullOrEmpty(model.Inspector))
                {
                    errors.Add("contact_cannot_be_empty".KeyToWord());
                }
                if (string.IsNullOrEmpty(model.InspectorTel))
                {
                    errors.Add("contact_phone_can_not_be_empty".KeyToWord());
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
                throw new AjaxException("zcode_must_be_a_positive_integer_number".KeyToWord());
            }
            _zCodeApiProxy.SubmitNewApply(new ZcodeApplyLog
            {
                Count = log.Count,
                ZcodeType = log.ZcodeType,
                MerchantId = _auth.GetAuth().MerchantId,
            });
            return new AjaxResult(new { flag = true });
        }

        [Route("forms/getzcodeapplylogs")]
        [HttpPost]
        public ActionResult GetZcodeApplyLogs(int pageIndex, int pageSize, HandleStatus? handleStatus, DateTime? startTime, DateTime? endTime)
        {
            return new AjaxResult(_zCodeApiProxy.GetApplyLogs(pageIndex, pageSize, handleStatus, startTime, endTime, _auth.GetAuth().MerchantId));
        }
    }
}