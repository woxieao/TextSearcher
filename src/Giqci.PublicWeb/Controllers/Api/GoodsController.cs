using System;
using System.Collections.Generic;
using Ktech.Mvc.ActionResults;
using System.Net;
using System.Web.Mvc;
using Giqci.Chapi.Models.Customer;
using Giqci.Interfaces;
using Giqci.PublicWeb.Models.Goods;
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
        public ActionResult MerchantGetProductList(int pageIndex = 1, int pageSize = 100)
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
                //todo  服务器返回信息乱码
                msg = ex.Message;
                msg = "添加失败,可能的原因:该商品已添加";
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


        [Route("goods/addcustomproduct")]
        [HttpPost]
        public ActionResult AddOrUpdateCustomProduct(CustomerProduct product)
        {
            string msg;
            bool flag;
            try
            {
                if (product.Id > 0)
                {
                    if (!product.IsApproved)
                    {
                        _merchantRepository.UpdateCustomerProducts(_auth.GetAuth().MerchantId, product);
                    }
                }
                else
                {
                    _merchantRepository.AddCustomProduct(_auth.GetAuth().MerchantId, product);
                }
                msg = "提交成功";
                flag = true;
            }
            catch
            {
                //todo  服务器返回信息乱码
                flag = false;
                msg = "提交信息不完整";
            }
            return new KtechJsonResult(HttpStatusCode.OK, new {flag = flag, msg = msg});
        }


        [Route("goods/getcustomproductlist")]
        [HttpPost]
        public ActionResult MerchantGetCustomProductList(string keywords = "")
        {
            var result = _merchantRepository.SelectCustomerProducts(_auth.GetAuth().MerchantId, keywords);
            return new KtechJsonResult(HttpStatusCode.OK, new {result = result});
        }

        [Route("goods/deletecustomproduct")]
        [HttpPost]
        public ActionResult MerchantDeleteCustomProduct(int id)
        {
            _merchantRepository.DeleteCustomerProduct(_auth.GetAuth().MerchantId, id);
            return new KtechJsonResult(HttpStatusCode.OK, new {flag = true});
        }

        [Route("goods/GetAllProduct")]
        [HttpPost]
        public ActionResult GetAllProduct()
        {
            var allProduct = new List<CommonProduct>();
            var productStrList = _merchantRepository.GetProducts(_auth.GetAuth().MerchantId, 1, 100);
            var ciqProductList = _productApiProxy.SearchProduct(productStrList);

            var customProductList = _merchantRepository.SelectCustomerProducts(_auth.GetAuth().MerchantId, string.Empty);
            foreach (var product in ciqProductList)
            {
                allProduct.Add(new CommonProduct
                {
                    IsApproved = product.IsApproved,
                    Brand = product.Brand,
                    Description = product.Description,
                    DescriptionEn = product.DescriptionEn,
                    HsCode = product.HsCode,
                    CiqCode = product.CiqCode,
                    ManufacturerCountry = product.ManufacturerCountry,
                    Package = product.Package,
                    Spec = product.Spec,
                    Manufacturer = product.Manufacturer
                });
            }
            foreach (var customProduct in customProductList)
            {
                allProduct.Add(new CommonProduct
                {
                    IsApproved = customProduct.IsApproved,
                    Brand = customProduct.Brand,
                    Description = customProduct.Description,
                    DescriptionEn = customProduct.DescriptionEn,
                    HsCode = customProduct.HsCode,
                    CiqCode = string.Empty,
                    ManufacturerCountry = customProduct.ManufacturerCountry,
                    Package = customProduct.Package,
                    Spec = customProduct.Spec,
                    Id = customProduct.Id,
                    Manufacturer = customProduct.Manufacturer,
                });
            }
            return new KtechJsonResult(HttpStatusCode.OK, new {result = allProduct});
        }
    }
}