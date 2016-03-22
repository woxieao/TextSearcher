using Giqci.Repositories;
using Ktech.Mvc.ActionResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Giqci.Interfaces;
using Giqci.Models.Tools;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("api")]
    public class GoodsController : Controller
    {
        private readonly IMerchantRepository _merchantRepository;
        private readonly IProductApiProxy _productApiProxy;

        public GoodsController(IMerchantRepository merchantRepository, IProductApiProxy productApiProxy)
        {
            _merchantRepository = merchantRepository;
            _productApiProxy = productApiProxy;
        }

        [Route("goods/getproductlist")]
        [HttpPost]
        public ActionResult MerchantGetProductList(int pageIndex = 1, int pageSize = 10)
        {
            var productList= _merchantRepository.GetFavoriteProducts(User.Identity.Name, new Page(pageIndex, pageSize));
            var result = _productApiProxy.SearchProduct(productList);
            return new KtechJsonResult(HttpStatusCode.OK, new {result = result});
        }

        [Route("goods/addproduct")]
        [HttpPost]
        public ActionResult MerchantAddProduct(string ciqCode)
        {
            string msg;
            var result = _merchantRepository.AddFavoriteProduct(User.Identity.Name, ciqCode, out msg);
            return new KtechJsonResult(HttpStatusCode.OK, new {result = result, msg = msg ?? "添加成功" });
        }

        [Route("goods/delete")]
        [HttpPost]
        public ActionResult MerchantDeleteProduct(string ciqCode)
        {
            _merchantRepository.RemoveFavoriteProduct(User.Identity.Name, ciqCode);

            return new KtechJsonResult(HttpStatusCode.OK, new {result = true});
        }

        [Route("goods/searchproduct")]
        [HttpPost]
        public ActionResult SearchProductList(string ciqCode )
        {
            var result = _productApiProxy.GetProduct(ciqCode);
            return new KtechJsonResult(HttpStatusCode.OK, new {result = result});
        }
    }
}