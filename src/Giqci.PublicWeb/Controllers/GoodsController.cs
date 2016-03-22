using Giqci.PublicWeb.Helpers;
using Giqci.Repositories;
using System;
using System.Web.Mvc;
using Giqci.Interfaces;

namespace Giqci.PublicWeb.Controllers
{
    public class GoodsController : Controller
    {
        private readonly IMerchantRepository _merchantRepo;
        private readonly IProductApiProxy _prodApi;

        public GoodsController(IMerchantRepository merchantRepo, IProductApiProxy prodApi)
        {
            _merchantRepo = merchantRepo;
            _prodApi = prodApi;
        }

        [Route("goods/list")]
        [HttpGet]
        [Authorize]
        public ActionResult MerchantProductList()
        {
            return View();
        }

        [Route("goods/add")]
        [HttpGet]
        [Authorize]
        public ActionResult GoodsAdd()
        {
            return View();
        }



        //[Route("goods/add")]
        //[HttpPost]
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