using System;
using System.Web.Mvc;
using System.Web.Security;
using Giqci.Chapi.Models.Customer;
using Giqci.Interfaces;
using Giqci.PublicWeb.Extensions;
using Giqci.PublicWeb.Services;

namespace Giqci.PublicWeb.Controllers
{

    [BaseAuthorize]
    [RoutePrefix("{languageType}")]
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
    }
}