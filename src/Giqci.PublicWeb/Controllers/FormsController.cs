using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Giqci.Enums;
using Giqci.PublicWeb.Helpers;
using Giqci.PublicWeb.Models.Application;
using Giqci.Repositories;
using Giqci.Services;
using Ktech.Mvc.ActionResults;
using Application = Giqci.Models.Application;
using GoodsItem = Giqci.Models.GoodsItem;
using Newtonsoft.Json;

namespace Giqci.PublicWeb.Controllers
{

    public class FormsController : Ktech.Mvc.ControllerBase
    {
        private readonly IPublicRepository _publicRepo;
        private readonly IMerchantRepository _merchantRepo;
        private readonly ICachedDictionaryService _cache;
        private readonly IApplicationRepository _appRepo;

        public FormsController(IPublicRepository publicRepo, ICachedDictionaryService cache, IMerchantRepository merchantRepo, IApplicationRepository appRepo)
        {
            _publicRepo = publicRepo;
            _merchantRepo = merchantRepo;
            _cache = cache;
            _appRepo = appRepo;
        }

        [Route("forms/app")]
        [HttpGet]
        [Authorize]
        public ActionResult Application(string applicantCode)
        {
            var merchant = _merchantRepo.GetMerchant(User.Identity.Name);
            string _appString = "";
            //get cookies 
            CookieHelper _cookie = new CookieHelper();
            _appString = _cookie.GetApplication("application");
            Application _application = JsonConvert.DeserializeObject<Application>(_appString);
            var model = new ApplicationPageModel { };
            if (_application == null)
            {
                model = new ApplicationPageModel
                {
                    Application = new Application
                    {
                        ApplicantCode = applicantCode,
                        Applicant = merchant.Name,
                        ApplicantAddr = merchant.Address,
                        ApplicantContact = merchant.Contact,
                        ApplicantPhone = merchant.Phone,
                        Goods = new List<GoodsItem> { new GoodsItem { ManufacturerCountry = "036" } }
                    }
                };
            }
            else
            {
                model = new ApplicationPageModel
               {
                   Application = _application
               };
            }
            ModelBuilder.SetHelperFields(_cache, model);
            return View(model);
        }

        [Route("forms/app")]
        [HttpPost]
        public ActionResult SubmitApplication(Application model)
        {
            var errors = validateApplication(model);
            string appNo = null;
            //set cookies 
            var cookieHelper = new CookieHelper();
            cookieHelper.SetApplication("application", model);
            // check user status
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
            {
                appNo = null;
                errors = new List<string>() { "登录状态已失效，请您重新登录系统" };
            }

            if (!errors.Any())
            {
                // submit
                appNo = _publicRepo.CreateApplication(User.Identity.Name, model);
                errors = null;
            }
            return new KtechJsonResult(HttpStatusCode.OK, new { appNo = appNo, errors = errors });
        }

        private List<string> validateApplication(Application model)
        {
            var errors = new List<string>();
            if (model.Goods.Count == 0)
            {
                errors.Add("请至少填写一个商品");
            }
            else
            {
                for (var i = 0; i < model.Goods.Count; i++)
                {
                    var item = model.Goods[i];
                    if (string.IsNullOrEmpty(item.DescriptionEn)
                    || (string.IsNullOrEmpty(item.HSCode) && string.IsNullOrEmpty(item.OtherHSCode))
                    || string.IsNullOrEmpty(item.CiqCode)
                    || string.IsNullOrEmpty(item.Spec)
                    || string.IsNullOrEmpty(item.ManufacturerCountry)
                    || string.IsNullOrEmpty(item.Brand)
                    || item.Quantity <= 0
                    || string.IsNullOrEmpty(item.Package))
                    {
                        errors.Add("商品" + (i + 1) + "个商品资料不完整");
                    }
                    if (!item.ExpiryDate.HasValue)
                    {
                        errors.Add("请填写商品" + (i + 1) + "的过期日期");
                    }
                    if (!item.ManufacturerDate.HasValue && string.IsNullOrEmpty(item.BatchNo))
                    {
                        errors.Add("请正确填写商品" + (i + 1) + "的生产日期或批次号");
                    }
                }
            }

            if (!model.C101 && !model.C102 && !model.C103)
            {
                errors.Add("请选择证书类型");
            }
            if (!Enum.IsDefined(typeof(TradeType), model.TradeType))
            {
                errors.Add("请选择商业目的");
            }
            if (string.IsNullOrEmpty(model.DestPort) &&
                string.IsNullOrEmpty(model.OtherDestPort))
            {
                errors.Add("请填写目标港口");
            }
            if (string.IsNullOrEmpty(model.LoadingPort) &&
                string.IsNullOrEmpty(model.OtherLoadingPort))
            {
                errors.Add("请填写发货港口");
            }
            if (model.InspectionDate < DateTime.Today)
            {
                errors.Add("请填写正确预约检查日期");
            }
            if (model.TotalUnits <= 0)
                errors.Add("运输总数量必须大于0");
            if (model.TotalWeight <= 0)
                errors.Add("运输总重量必须大于0");
            return errors;
        }
    }
}