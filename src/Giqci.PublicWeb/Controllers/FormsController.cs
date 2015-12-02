using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Giqci.Entities.Core;
using Giqci.Enums;
using Giqci.Models;
using Giqci.PublicWeb.Helpers;
using Giqci.PublicWeb.Models.Application;
using Giqci.PublicWeb.Services;
using Giqci.Repositories;
using Ktech.Mvc.ActionResults;
using Application = Giqci.Models.Application;
using GoodsItem = Giqci.Models.GoodsItem;

namespace Giqci.PublicWeb.Controllers
{
    //[Authorize]
    public class FormsController : Ktech.Mvc.ControllerBase
    {
        private readonly IGiqciRepository _coreRepo;
        private readonly IMerchantRepository _merchantRepo;
        private readonly ICacheService _cache;

        public FormsController(IGiqciRepository coreRepo, ICacheService cache, IMerchantRepository merchantRepo)
        {
            _coreRepo = coreRepo;
            _merchantRepo = merchantRepo;
            _cache = cache;
        }

        [Route("forms/app")]
        [HttpGet]
        public ActionResult Application(string applicantCode)
        {
            //var merchant = _merchantRepo.GetMerchant(User.Identity.Name);
            var merchant = new MerchantViewModel();
            var model = new ApplicationViewModel
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
            ModelBuilder.SetHelperFields(_cache, model);
            return View(model);
        }

        [Route("forms/app")]
        [HttpPost]
        public ActionResult SubmitApplication(Application model)
        {
            var errors = validateApplication(model);
            string appNo = null;
            if (!errors.Any())
            {
                // submit
                appNo = _coreRepo.CreateApplication(model);
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
            if (!Enum.IsDefined(typeof(CertType), model.CertType))
            {
                errors.Add("请选择证书类型");
            }
            if (!Enum.IsDefined(typeof(TradeType), model.TradeType))
            {
                errors.Add("请选择商业目的");
            }
            // 可以后补
            //if (string.IsNullOrEmpty(model.Application.Billno))
            //{
            //    model.ErrorMessage.Add("请填写提货单");
            //}
            //if (string.IsNullOrEmpty(model.Application.Vesselcn))
            //{
            //    model.ErrorMessage.Add("请填写船名");
            //}
            //if (string.IsNullOrEmpty(model.Application.Voyage))
            //{
            //    model.ErrorMessage.Add("请填写船次");
            //}
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
                errors.Add("请填写正确预约发货日期");
            }
            if (model.TotalPallets <= 0)
                errors.Add("运输总数量必须大于0");
            if (model.TotalWeight <= 0)
                errors.Add("运输总重量必须大于0");
            //if (!string.IsNullOrEmpty(model.Application.DestPort) && !_dictRepo.IsValidPort(model.Application.DestPort))
            //{
            //    model.ErrorMessage.Add("无效目标港口代码");
            //}
            //if (!string.IsNullOrEmpty(model.Application.LoadingPort) && !_dictRepo.IsValidPort(model.Application.LoadingPort))
            //{
            //    model.ErrorMessage.Add("无效发货港口代码");
            //}

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
                    errors.Add("第" + (i + 1) + "个商品资料不完整");
                }
            }
            return errors;
        }
    }
}