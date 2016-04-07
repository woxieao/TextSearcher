using System;
using System.Net;
using System.Web.Mvc;
using Giqci.Chapi.Models.Customer;
using Giqci.Chapi.Models.Product;
using Giqci.Interfaces;
using Giqci.PublicWeb.Services;
using Ktech.Mvc.ActionResults;

namespace Giqci.PublicWeb.Controllers
{
    public class GoodsController : Controller
    {
        private readonly IMerchantApiProxy _merchantRepo;
        private readonly IMerchantProductApiProxy _merchantRepository;
        private readonly IProductApiProxy _prodApi;
        private readonly IAuthService _auth;

        public GoodsController(IMerchantProductApiProxy merchantRepository, IMerchantApiProxy merchantRepo,
            IProductApiProxy prodApi, IAuthService auth)
        {
            _merchantRepository = merchantRepository;
            _merchantRepo = merchantRepo;
            _prodApi = prodApi;
            _auth = auth;
        }

        [Route("goods/list")]
        [HttpGet]
        [Authorize]
        public ActionResult MerchantProductList()
        {
            return View();
        }

        [Route("goods/customproductlist")]
        [HttpGet]
        [Authorize]
        public ActionResult MerchantCustomProductList()
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

        [Route("goods/showcustomproduct")]
        [HttpGet]
        [Authorize]
        public ActionResult ShowCustomProduct(int id = 0)
        {
            var product = id == 0
                ? new CustomerProduct()
                : _merchantRepository.GetCustomerProduct(_auth.GetAuth().MerchantId, id);
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
            //return new KtechJsonResult(HttpStatusCode.OK, new { result = result, errors = errors });
        }
    }
}