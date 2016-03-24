using System;
using Ktech.Mvc.ActionResults;
using System.Net;
using System.Web.Mvc;
using Giqci.Interfaces;
using Giqci.PublicWeb.Services;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("api")]
    [Authorize]
    public class GoodsController : Controller
    {
        private readonly IMerchantProductApiProxy _merchantRepository;
        private readonly IProductApiProxy _productApiProxy;
        private readonly IAuthService _auth;

        public GoodsController(IMerchantProductApiProxy merchantRepository, IProductApiProxy productApiProxy,
            IAuthService auth)
        {
            _merchantRepository = merchantRepository;
            _productApiProxy = productApiProxy;
            _auth = auth;
        }

        [Route("goods/getproductlist")]
        [HttpPost]
        public ActionResult MerchantGetProductList(int pageIndex = 1, int pageSize = 10)
        {
            var productList = _merchantRepository.GetProducts(_auth.GetAuth().MerchantId, pageIndex, pageSize);
            var result = _productApiProxy.SearchProduct(productList);
            return new KtechJsonResult(HttpStatusCode.OK, new {result = result});
        }

        [Route("goods/addproduct")]
        [HttpPost]
        public ActionResult MerchantAddProduct(string ciqCode)
        {
            string msg;
            bool result;
            try
            {
                result = _merchantRepository.AddProduct(_auth.GetAuth().MerchantId, ciqCode, out msg);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                result = false;
            }
            return new KtechJsonResult(HttpStatusCode.OK, new {result = result, msg = msg ?? "添加成功"});
        }

        [Route("goods/delete")]
        [HttpPost]
        public ActionResult MerchantDeleteProduct(string ciqCode)
        {
            _merchantRepository.RemoveProduct(_auth.GetAuth().MerchantId, ciqCode);

            return new KtechJsonResult(HttpStatusCode.OK, new {result = true});
        }

        [Route("goods/searchproduct")]
        [HttpPost]
        public ActionResult SearchProductList(string ciqCode)
        {
            var result = _productApiProxy.GetProduct(ciqCode);
            return new KtechJsonResult(HttpStatusCode.OK, new {result = result});
        }
    }
}