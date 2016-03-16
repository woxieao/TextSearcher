using Giqci.Models;
using Giqci.PublicWeb.Helpers;
using Giqci.PublicWeb.Models.Application;
using Giqci.PublicWeb.Models.Goods;
using Giqci.Repositories;
using Giqci.Services;
using Ktech.Mvc.ActionResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GoodsViewModel = Giqci.Models.GoodsViewModel;

namespace Giqci.PublicWeb.Controllers
{
    public class GoodsController : Controller
    {
        private readonly IMerchantRepository _merchantRepo;
        private readonly IGoodsRepository _goodsRepo;
        private readonly ICachedDictionaryService _cache;

        public GoodsController(IMerchantRepository merchantRepo, IGoodsRepository goodsRepo,
            ICachedDictionaryService cache)
        {
            _merchantRepo = merchantRepo;
            _goodsRepo = goodsRepo;
            _cache = cache;
        }

        [Route("goods/list")]
        [HttpGet]
        [Authorize]
        public ActionResult List()
        {
            return View();
        }

        [Route("goods/add")]
        [HttpGet]
        [Authorize]
        public ActionResult AddGoods(string applicantCode)
        {
            var model = new GoodsPageModel
            {
                Goods = new GoodsModel
                {
                    ManufacturerCountry = "036"
                }
            };
            ModelBuilder.SetHelperGoodsModel(_cache, model);
            return View("GoodsAdd", model);
        }


        [Route("goods/add/{id:int}")]
        [HttpGet]
        [Authorize]
        public ActionResult AddGoods(int id = 0)
        {
            var model = new GoodsPageModel();
            var goodmodel = _goodsRepo.GetGoods(id);
            model.Goods = goodmodel;

            var merchant = _merchantRepo.GetMerchant(User.Identity.Name);
            if (merchant.Id != goodmodel.MerchantId)
            {
                Response.Redirect("/goods/list");
            }
            else
            {
                model.Goods = goodmodel;
            }
            ModelBuilder.SetHelperGoodsModel(_cache, model);
            return View("GoodsAdd", model);
        }


        [Route("goods/add")]
        [HttpPost]
        public ActionResult SubmitApplication(GoodsViewModel model)
        {
            var errors = ValidateGoods(model);
            bool result = false;
            // check user status
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
            {
                result = false;
                errors = new List<string>() {"登录状态已失效，请您重新登录系统"};
            }
            if (!errors.Any())
            {
                // submit
                if (model.Id == 0)
                {
                    string message = null;
                    model.CreateDate = DateTime.Now;
                    model.IsDelete = false;
                    result = _goodsRepo.InsertGoods(User.Identity.Name, model, out message);
                    errors.Add(message);
                }
                else
                {
                    string message = null;
                    result = _goodsRepo.UpdateGoods(model.Id, model, out message);
                    errors.Add(message);
                }
            }
            return new KtechJsonResult(HttpStatusCode.OK, new {result = result, errors = errors});
        }


        private List<string> ValidateGoods(GoodsViewModel model)
        {
            var errors = new List<string>();
            if (string.IsNullOrEmpty(model.DescriptionEn))
            {
                errors.Add("请填写商品名称(英文)");
            }
            if (string.IsNullOrEmpty(model.Brand))
            {
                errors.Add("请填写商品品牌 ");
            }
            if (string.IsNullOrEmpty(model.HSCode) && string.IsNullOrEmpty(model.OtherHSCode))
            {
                errors.Add("请填写商品HS编码");
            }
            if (string.IsNullOrEmpty(model.Spec))
            {
                errors.Add("请填写商品规格型号");
            }
            if (string.IsNullOrEmpty(model.ManufacturerCountry))
            {
                errors.Add("请选择制造国代码");
            }
            if (string.IsNullOrEmpty(model.CiqCode))
            {
                errors.Add("请填写商品备案号");
            }
            if (string.IsNullOrEmpty(model.Package))
            {
                errors.Add("请填写商品包装");
            }
            if (model.C102 && string.IsNullOrEmpty(model.C102Comment))
            {
                errors.Add("请填写检验内容");
            }
            return errors;
        }
    }
}