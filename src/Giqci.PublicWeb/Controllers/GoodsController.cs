using System;
using System.Web.Mvc;
using System.Web.Security;
using Giqci.Chapi.Models.Customer;
using Giqci.Interfaces;
using Giqci.PublicWeb.Services;

namespace Giqci.PublicWeb.Controllers
{

    [Authorize]
    public class GoodsController : Controller
    {
        private readonly IMerchantProductApiProxy _merchantRepository;
        private readonly IAuthService _auth;

        public GoodsController(IMerchantProductApiProxy merchantRepository, IAuthService auth)
        {
            _merchantRepository = merchantRepository;
            _auth = auth;
        }

        [Route("goods/list")]
        [HttpGet]

        public ActionResult MerchantProductList()
        {
            return View();
        }

        [Route("goods/customproductlist")]
        [HttpGet]
        public ActionResult MerchantCustomProductList()
        {
            return View();
        }

        [Route("goods/add")]
        [HttpGet]
        public ActionResult GoodsAdd()
        {
            return View();
        }

        [Route("goods/showcustomproduct")]
        [HttpGet]
        public ActionResult ShowCustomProduct(int id = 0)
        {
            var merchant = _auth.GetAuth();
            var product = id == 0
                  ? new CustomerProduct()
                  : _merchantRepository.GetCustomerProduct(merchant.MerchantId, id);
            //防止编辑已批准的商品
            product = product.IsApproved ? new CustomerProduct() : product;
            return View(product);
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
            //return new AjaxResult( new { result = result, errors = errors });
        }
    }
}