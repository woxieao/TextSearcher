using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Security;
using Giqci.Chapi.Enums.App;
using Giqci.Chapi.Models.App;
using Giqci.Chapi.Models.Customer;
using Giqci.Interfaces;
using Giqci.PublicWeb.Extensions;
using Giqci.PublicWeb.Models.Ajax;
using Giqci.PublicWeb.Models.Goods;
using Giqci.PublicWeb.Services;
using Ktech.Mvc.ActionResults;

namespace Giqci.PublicWeb.Controllers.AuthorizeAjax
{
    [RoutePrefix("{languageType}/api")]
    [AjaxAuthorize]
    public class GoodsController : AjaxController
    {
        private readonly IMerchantProductApiProxy _merchantRepository;
        private readonly IProductApiProxy _productApiProxy;
        private readonly IAuthService _auth;
        private readonly IDictService _dict;
        private readonly IMerchantApplicationApiProxy _repo;

        public GoodsController(IMerchantProductApiProxy merchantRepository, IProductApiProxy productApiProxy,
            IAuthService auth, IDictService dict, IMerchantApplicationApiProxy repo)
        {
            _merchantRepository = merchantRepository;
            _productApiProxy = productApiProxy;
            _auth = auth;
            _dict = dict;
            _repo = repo;
        }

        [Route("goods/getproductlist")]
        [HttpPost]
        public ActionResult MerchantGetProductList(string keywords, int pageIndex = 1, int pageSize = 20)
        {
            //预留分页条件
            pageSize = 10000;
            var productList = _merchantRepository.GetProducts(_auth.GetAuth().MerchantId, pageIndex, pageSize);
            var result = _productApiProxy.SearchProduct(productList);
            return new AjaxResult(new { result = result.Where(i => string.IsNullOrEmpty(keywords) || i.CiqCode.Contains(keywords) || i.HsCode.Contains(keywords) || (!string.IsNullOrEmpty(i.Description) && i.Description.Contains(keywords)) || (!string.IsNullOrEmpty(i.DescriptionEn) && i.DescriptionEn.Contains(keywords))) });
        }

        [Route("goods/GetCustomerProductDetail")]
        [HttpPost]
        public ActionResult GetCustomerProductDetail(int productId)
        {
            var product = _merchantRepository.GetCustomerProduct(_auth.GetAuth().MerchantId, productId);
            return new AjaxResult(new { product = product });
        }

        [Route("goods/addproduct")]
        [HttpPost]
        public ActionResult MerchantAddProduct(string ciqCode)
        {
            string msg;
            bool result;
            try
            {
                //todo  服务器返回信息乱码
                result = _merchantRepository.AddProduct(_auth.GetAuth().MerchantId, ciqCode, out msg);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                result = false;
            }

            return new AjaxResult(
                new { result = result, msg = msg == null ? "添加成功" : "添加失败,可能的原因该商品已添加" });
        }

        [Route("goods/delete")]
        [HttpPost]
        public ActionResult MerchantDeleteProduct(string ciqCode)
        {
            _merchantRepository.RemoveProduct(_auth.GetAuth().MerchantId, ciqCode);
            return new AjaxResult(new { result = true });
        }

        [AllowAnonymous]
        [Route("goods/searchproduct")]
        [HttpPost]
        public ActionResult SearchProductList(string ciqCode)
        {
            var result = _productApiProxy.GetProduct(ciqCode);
            return new AjaxResult(new { result = result });
        }


        [Route("goods/addcustomproduct")]
        [HttpPost]
        public ActionResult AddOrUpdateCustomProduct(CustomerProduct product)
        {
            string msg;
            bool flag;
            try
            {
                product.Code = string.IsNullOrEmpty(product.Code)
                       ? Guid.NewGuid().ToString("N").Substring(0, 6)
                       : product.Code;
                var isExit = _merchantRepository.IsExistsCustomProductCode(_auth.GetAuth().MerchantId, product.Id,
                    product.Code);
                //todo 两个商品表合一之后用FluentValidation
                var reg = new Regex("^[a-z0-9A-Z]+$");
                if (!reg.IsMatch(product.Code))
                {
                    return new AjaxResult(new { flag = false, msg = "商品标识只能为数字或者字母" });
                }
                if (isExit)
                {
                    return new AjaxResult(new { flag = false, msg = "该商品标识已重复" });
                }
                //todo 两个商品表合一之后用FluentValidation
                if (string.IsNullOrEmpty(product.Brand)
                    || string.IsNullOrEmpty(product.Code)
                    || string.IsNullOrEmpty(product.Description)
                    || string.IsNullOrEmpty(product.DescriptionEn)
                    || string.IsNullOrEmpty(product.HsCode)
                    || string.IsNullOrEmpty(product.ManufacturerCountry)
                    || string.IsNullOrEmpty(product.Package)
                    || string.IsNullOrEmpty(product.Spec)
                    || string.IsNullOrEmpty(product.Manufacturer))
                {
                    return new AjaxResult(new { flag = false, msg = "商品信息不完整" });
                }
                var regManu = new Regex("^[\x20-\x7e]*$");
                if (!regManu.IsMatch(product.Manufacturer))
                {
                    return new AjaxResult(new { flag = false, msg = "制造商中含有不合法字符" });
                }
                if (!regManu.IsMatch(product.DescriptionEn))
                {
                    return new AjaxResult(new { flag = false, msg = "名称(英文)含有不合法字符" });
                }
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
                flag = false;
                msg = "提交信息不完整";
            }
            return new AjaxResult(new { flag = flag, msg = msg });
        }


        [Route("goods/getcustomproductlist")]
        [HttpPost]
        public ActionResult MerchantGetCustomProductList(string keywords = "")
        {
            var result = _merchantRepository.SelectCustomerProducts(_auth.GetAuth().MerchantId, keywords);
            return new AjaxResult(new { result = result });
        }

        [Route("goods/deletecustomproduct")]
        [HttpPost]
        public ActionResult MerchantDeleteCustomProduct(int id)
        {
            _merchantRepository.DeleteCustomerProduct(_auth.GetAuth().MerchantId, id);
            return new AjaxResult(new { flag = true });
        }

        [Route("goods/GetAllProduct")]
        [HttpPost]
        public ActionResult GetAllProduct(string keyWords = "", string code = "", int tradetype = 0)
        {
            bool requireciqcode = false;
            if (!string.IsNullOrEmpty(code))
            {
                var port = _dict.GetPort(code);
                if (port.RequireCiqCode && tradetype == (int)TradeType.C)
                {
                    requireciqcode = true;
                }
            }
            var allProduct = new List<CommonProduct>();
            var productStrList = _merchantRepository.GetProducts(_auth.GetAuth().MerchantId, 1,
                string.IsNullOrEmpty(keyWords) ? 10 : 10000);
            var ciqProductList = _productApiProxy.SearchProduct(productStrList);
            var customProductList = _merchantRepository.SelectCustomerProducts(_auth.GetAuth().MerchantId, string.Empty);
            var ran = new Random();

            foreach (var product in ciqProductList)
            {
                allProduct.Add(new CommonProduct
                {
                    Id = ran.Next(100000, 999999),
                    IsApproved = product.IsApproved,
                    Brand = product.Brand,
                    Description = product.Description,
                    DescriptionEn = product.DescriptionEn,
                    HsCode = product.HsCode,
                    CiqCode = product.CiqCode,
                    ManufacturerCountry = product.ManufacturerCountry,
                    Package = product.Package,
                    Spec = product.Spec,
                    Manufacturer = product.Manufacturer,
                    Code = String.Empty,
                    CustomProductId = null
                });
            }
            if (!requireciqcode)
            {
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
                        Code = customProduct.Code,
                        CustomProductId = customProduct.Id
                    });
                }
            }

            var filterResult = allProduct.Where(
                i => (i.CiqCode != null && i.CiqCode.IndexOf(keyWords, StringComparison.OrdinalIgnoreCase) > -1)
                     || (i.Description != null && i.Description.IndexOf(keyWords, StringComparison.OrdinalIgnoreCase) > -1)
                     || (i.Brand != null && i.Brand.IndexOf(keyWords, StringComparison.OrdinalIgnoreCase) > -1)
                     || (i.HsCode != null && i.HsCode.IndexOf(keyWords, StringComparison.OrdinalIgnoreCase) > -1)
                     || (i.Manufacturer != null && i.Manufacturer.IndexOf(keyWords, StringComparison.OrdinalIgnoreCase) > -1)
                     || (i.ManufacturerCountry != null && i.ManufacturerCountry.IndexOf(keyWords, StringComparison.OrdinalIgnoreCase) > -1)
                     || (i.Package != null && i.Package.IndexOf(keyWords, StringComparison.OrdinalIgnoreCase) > -1)
                     || (i.Spec != null && i.Spec.IndexOf(keyWords, StringComparison.OrdinalIgnoreCase) > -1)
                     || (i.DescriptionEn != null && i.DescriptionEn.IndexOf(keyWords, StringComparison.OrdinalIgnoreCase) > -1)
                     || (i.Code != null && i.Code.IndexOf(keyWords, StringComparison.OrdinalIgnoreCase) > -1)).Skip(0).Take(10);
            return new AjaxResult(new { result = filterResult });
        }



        [Route("goods/convertproduct/{customProductId}/{ciqCode}")]
        [HttpPost]
        public ActionResult ConvertProduct(int customProductId, string ciqCode, string enName = "")
        {
            var flag = true;
            var msg = "success";
            var convertCount = 0;
            try
            {
                var ciqProduct = _productApiProxy.GetProduct(ciqCode);
                if (ciqProduct == null)
                {
                    throw new Exception("该备案号不存在");
                }
                if (!ciqProduct.IsApproved)
                {
                    if (string.IsNullOrEmpty(enName))
                    {
                        throw new Exception("请完善商品英文名");
                    }
                    var productList = new List<ApplicationProduct>
                        {
                            new ApplicationProduct
                            {
                                CiqCode = ciqProduct.CiqCode,
                                DescriptionEn = enName,
                            }
                        };
                    _productApiProxy.UpdateCiqProductInfo(productList);
                }
                string tempMsg;
                convertCount = _repo.ConvertCustomProduct(_auth.GetAuth().MerchantId, customProductId, ciqCode);
                _merchantRepository.AddProduct(_auth.GetAuth().MerchantId, ciqCode, out tempMsg);
                _merchantRepository.DeleteCustomerProduct(_auth.GetAuth().MerchantId, customProductId);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                flag = false;
            }
            return new AjaxResult(new
            {
                flag = flag,
                convertCount = convertCount,
                msg = msg
            });
        }
    }
}