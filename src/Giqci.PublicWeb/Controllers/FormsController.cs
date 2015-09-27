using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Giqci.Enums;
using Giqci.Models;
using Giqci.PublicWeb.Helpers;
using Giqci.PublicWeb.Models.Application;
using Giqci.PublicWeb.Services;
using Giqci.Repositories;

namespace Giqci.PublicWeb.Controllers
{
    public class FormsController : Controller
    {
        private readonly IGiqciRepository _coreRepo;
        private readonly IDictionaryRepository _dictRepo;
        private readonly ICacheService _cache;

        public FormsController(IGiqciRepository coreRepo, IDictionaryRepository dictRepo, ICacheService cache)
        {
            _coreRepo = coreRepo;
            _dictRepo = dictRepo;
            _cache = cache;
        }

        [Route("forms/app")]
        [HttpGet]
        public ActionResult Application(string applicantCode)
        {
            var model = new ApplicationViewModel
            {
                Application = new Application
                {
                    ApplicantCode = applicantCode,
                    Goods = new List<GoodsItem>()
                }
            };
            ModelBuilder.SetHelperFields(_cache, model);
            return View(model);
        }

        [Route("forms/app")]
        [HttpPost]
        public ActionResult SubmitApplication(ApplicationViewModel model)
        {
            //var deleteitem = Request.Form["delete"];
            //if (!string.IsNullOrEmpty(deleteitem))
            //{
            //    model.Application.Goods.RemoveAt(int.Parse(deleteitem));
            //}
            resetGoods(model);
            if (Request.Form["submit"] == "additem")
            {
                addItem(model);
            }
            else if (Request.Form["submit"] == "submit")
            {
                validateApplication(model);
                if (model.ErrorMessage.Count == 0)
                {
                    // submit
                    var appNo = _coreRepo.CreateApplication(model.Application);
                    return View("Created", model: appNo);
                }
            }
            ModelBuilder.SetHelperFields(_cache, model);
            return View("Application", model);
        }

        private void resetGoods(ApplicationViewModel model)
        {
            for (int i = 0; i < model.Application.Goods.Count; i++)
            {
                if (string.IsNullOrEmpty(model.Application.Goods[i].DescriptionEn))
                {
                    model.Application.Goods.RemoveAt(i);
                }
            }
        }

        private void validateApplication(ApplicationViewModel model)
        {
            if (model.Application.Goods.Count == 0)
            {
                model.ErrorMessage.Add("请至少填写一个商品");
            }
            if (!Enum.IsDefined(typeof(CertType), model.Application.CertType))
            {
                model.ErrorMessage.Add("请选择证书类型");
            }
            if (!Enum.IsDefined(typeof(TradeType), model.Application.TradeType))
            {
                model.ErrorMessage.Add("请选择商业目的");
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
            if (!model.AcceptTerms)
            {
                model.ErrorMessage.Add("你必须接受我们的条款协议");
            }
            if (string.IsNullOrEmpty(model.Application.DestPort) &&
                string.IsNullOrEmpty(model.Application.OtherDestPort))
            {
                model.ErrorMessage.Add("请填写目标港口");
            }
            if (string.IsNullOrEmpty(model.Application.LoadingPort) &&
                string.IsNullOrEmpty(model.Application.OtherLoadingPort))
            {
                model.ErrorMessage.Add("请填写发货港口");
            }
            //if (!string.IsNullOrEmpty(model.Application.DestPort) && !_dictRepo.IsValidPort(model.Application.DestPort))
            //{
            //    model.ErrorMessage.Add("无效目标港口代码");
            //}
            //if (!string.IsNullOrEmpty(model.Application.LoadingPort) && !_dictRepo.IsValidPort(model.Application.LoadingPort))
            //{
            //    model.ErrorMessage.Add("无效发货港口代码");
            //}
        }

        private void addItem(ApplicationViewModel model)
        {
            var item = model.NewItem;
            if (!string.IsNullOrEmpty(item.DescriptionEn))
            {
                if (string.IsNullOrEmpty(item.DescriptionEn)
                    || (string.IsNullOrEmpty(item.HSCode) && string.IsNullOrEmpty(item.OtherHSCode))
                    || string.IsNullOrEmpty(item.CiqCode)
                    || string.IsNullOrEmpty(item.Spec)
                    || string.IsNullOrEmpty(item.ManufacturerCountry)
                    || string.IsNullOrEmpty(item.Brand))
                {
                    model.ErrorMessage.Add("请填写完整商品资料");
                }
                else
                {
                    //if (!_repo.IsValidCountry(item.ManufacturerCountry))
                    //{
                    //    model.ErrorMessage.Add("制造国家代码错误");
                    //}
                    //else
                    //{
                    if (!string.IsNullOrEmpty(item.HSCode))
                    {
                        var hscode = _dictRepo.GetHSCode(item.HSCode);
                        //item.Package = hscode.Unit;
                    }
                    item.ManufacturerCountryName = _cache.GetCountries().First(i => i.Key == item.ManufacturerCountry).Value;
                    model.Application.Goods.Add(item);
                    model.NewItem = new GoodsItem();
                }
            }
        }
    }
}