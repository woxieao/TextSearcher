using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Giqci.Enums;
using Giqci.Models;
using Giqci.PublicWeb.Models.Application;
using Giqci.PublicWeb.Services;
using Giqci.Repositories;

namespace Giqci.PublicWeb.Controllers
{
    public class FormsController : Controller
    {
        private IGiqciRepository _repo;
        private ICacheService _cache;

        public FormsController(IGiqciRepository repo, ICacheService cache)
        {
            _repo = repo;
            _cache = cache;
        }

        [Route("forms/app")]
        [HttpGet]
        public ActionResult Application(string applicantCode)
        {
            var countries = _repo.GetCountryDictionary();

            var model = new ApplicationViewModel
            {
                Application = new Application
                {
                    ApplicantCode = applicantCode,
                    Goods = new List<GoodsItem>()
                },
                Countries = _cache.GetCountries().Select(i => new SelectListItem { Text = i.Value, Value = i.Key, Selected = i.Key == "036" }).ToList()
            };
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
            if (Request.Form["submit"] == "additem")
            {
                addItem(model);
            }
            validateApplication(model);
            if (model.ErrorMessage.Count == 0 && Request.Form["submit"] == "submit")
            {
                // submit
                _repo.CreateApplication(model.Application);
                return View("Created");
            }
            model.Countries = _cache.GetCountries().Select(i => new SelectListItem { Text = i.Value, Value = i.Key, Selected = i.Key == "036" }).ToList();
            return View("Application", model);
        }

        private void validateApplication(ApplicationViewModel model)
        {
            for (int i = model.Application.Goods.Count - 1; i > 0; i--)
            {
                if (string.IsNullOrEmpty(model.Application.Goods[i].DescriptionEn))
                {
                    model.Application.Goods.RemoveAt(i);
                }
            }

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
            if (string.IsNullOrEmpty(model.Application.Billno))
            {
                model.ErrorMessage.Add("请填写提货单");
            }
            if (string.IsNullOrEmpty(model.Application.Vesselcn))
            {
                model.ErrorMessage.Add("请填写船名");
            }
            if (string.IsNullOrEmpty(model.Application.Voyage))
            {
                model.ErrorMessage.Add("请填写船次");
            }
            if (!model.AcceptTerms)
            {
                model.ErrorMessage.Add("你必须接受我们的条款协议");
            }
            if (!string.IsNullOrEmpty(model.Application.DestPort) && !_repo.IsValidPort(model.Application.DestPort))
            {
                model.ErrorMessage.Add("无效目标港口代码");
            }
            if (!string.IsNullOrEmpty(model.Application.LoadingPort) && !_repo.IsValidPort(model.Application.LoadingPort))
            {
                model.ErrorMessage.Add("无效发货港口代码");
            }
        }

        private void addItem(ApplicationViewModel model)
        {
            var item = model.Application.Goods[0];
            if (!string.IsNullOrEmpty(item.DescriptionEn))
            {
                if (string.IsNullOrEmpty(item.HSCode)
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
                    model.Application.Goods.Insert(0, new GoodsItem());
                }
            }
        }
    }
}