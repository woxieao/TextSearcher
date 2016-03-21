using Giqci.PublicWeb.Helpers;
using Giqci.Repositories;
using System;
using System.Web.Mvc;

namespace Giqci.PublicWeb.Controllers
{
    public class GoodsController : Controller
    {
        private readonly IMerchantRepository _merchantRepo;
        private readonly IProductRepository _goodsRepo;

        public GoodsController(IMerchantRepository merchantRepo, IProductRepository goodsRepo)
        {
            _merchantRepo = merchantRepo;
            _goodsRepo = goodsRepo;
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
            // TODO
            throw new NotImplementedException();
            //var model = new GoodsPageModel
            //{
            //    Goods = new GoodsModel
            //    {
            //        ManufacturerCountry = "036"
            //    }
            //};
            //ModelBuilder.SetHelperGoodsModel(_cache, model);
            //return View("GoodsAdd", model);
        }


        [Route("goods/add/{id:int}")]
        [HttpGet]
        [Authorize]
        public ActionResult AddGoods(int id = 0)
        {
            // TODO
            throw new NotImplementedException();
            //var model = new GoodsPageModel();
            //var goodmodel = _goodsRepo.GetGoods(id);
            //model.Goods = goodmodel;

            //var merchant = _merchantRepo.GetMerchant(User.Identity.Name);
            //if (merchant.Id != goodmodel.MerchantId)
            //{
            //    Response.Redirect("/goods/list");
            //}
            //else
            //{
            //    model.Goods = goodmodel;
            //}
            //ModelBuilder.SetHelperGoodsModel(_cache, model);
            //return View("GoodsAdd", model);
        }


        [Route("goods/add")]
        [HttpPost]
        public ActionResult SubmitApplication(string ciqCode)
        {
            throw new NotImplementedException();
            //var errors = ValidateGoods(model);
            //bool result = false;
            //// check user status
            //var userName = User.Identity.Name;
            //if (string.IsNullOrEmpty(userName))
            //{
            //    result = false;
            //    errors = new List<string>() { "登录状态已失效，请您重新登录系统" };
            //}
            //if (!errors.Any())
            //{
            //    // submit
            //    if (model.Id == 0)
            //    {
            //        string message = null;
            //        model.CreateDate = DateTime.Now;
            //        model.IsDelete = false;
            //        result = _goodsRepo.InsertGoods(User.Identity.Name, model, out message);
            //        errors.Add(message);
            //    }
            //    else
            //    {
            //        string message = null;
            //        result = _goodsRepo.UpdateGoods(model.Id, model, out message);
            //        errors.Add(message);
            //    }
            //}
            //return new KtechJsonResult(HttpStatusCode.OK, new { result = result, errors = errors });
        }
    }
}