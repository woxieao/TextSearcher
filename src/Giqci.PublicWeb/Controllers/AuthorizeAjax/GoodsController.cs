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
using Giqci.Chapi.Models.Product;
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
            var productList = _merchantRepository.GetProducts(_auth.GetAuth().MerchantId,keywords, pageIndex, pageSize,true);
            return new AjaxResult(new { result = productList });
        }

        [Route("goods/GetCustomerProductDetail")]
        [HttpPost]
        public ActionResult GetCustomerProductDetail(string key)
        {
            var product = _merchantRepository.GetCustomerProduct(_auth.GetAuth().MerchantId, key);
            return new AjaxResult(new { product = product });
        }

        [Route("goods/addproduct")]
        [HttpPost]
        public ActionResult MerchantAddProduct(string key)
        {
            string msg;
            bool result;
            try
            {
                //todo  服务器返回信息乱码
                result = _merchantRepository.AddProduct(_auth.GetAuth().MerchantId, key, out msg);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                result = false;
            }

            return new AjaxResult(
                new { result = result, msg = msg == null ? "added_successfully".KeyToWord() : "product_add_failed".KeyToWord() });
        }

        [Route("goods/delete")]
        [HttpPost]
        public ActionResult MerchantDeleteProduct(string key)
        {
            _merchantRepository.RemoveProduct(_auth.GetAuth().MerchantId, key);
            return new AjaxResult(new { result = true });
        }

        [AllowAnonymous]
        [Route("goods/searchproduct")]
        [HttpPost]
        public ActionResult SearchProductList(string ciqCode)
        {
            var result = _merchantRepository.GetProduct(ciqCode);
            return new AjaxResult(new { result = result });
        }


        [Route("goods/addcustomproduct")]
        [HttpPost]
        public ActionResult AddOrUpdateCustomProduct(QProduct product)
        {
            string msg;
            bool flag;
            try
            {
                product.Code = string.IsNullOrEmpty(product.Code)
                       ? Guid.NewGuid().ToString("N").Substring(0, 6)
                       : product.Code;
                //var isExit = _merchantRepository.IsExistsCustomProductCode(_auth.GetAuth().MerchantId, product.Id,
                //    product.Code);
                //todo 两个商品表合一之后用FluentValidation
                var reg = new Regex("^[a-z0-9A-Z]+$");
                if (!reg.IsMatch(product.Code))
                {
                    return new AjaxResult(new { flag = false, msg = "product_identification_can_only_be_numbers_or_lett".KeyToWord() });
                }
                //if (isExit)
                //{
                //    return new AjaxResult(new { flag = false, msg = "the_product_identification_has_been_repeated".KeyToWord() });
                //}
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
                    return new AjaxResult(new { flag = false, msg = "commodity_information_is_not_complete".KeyToWord() });
                }
                var regManu = new Regex("^[\x20-\x7e]*$");
                if (!regManu.IsMatch(product.Manufacturer))
                {
                    return new AjaxResult(new { flag = false, msg = "the_manufacturer_contains_illegal_characters".KeyToWord() });
                }
                if (!regManu.IsMatch(product.DescriptionEn))
                {
                    return new AjaxResult(new { flag = false, msg = "english_name_contains_illegal_characters".KeyToWord() });
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
                msg = "submit_successfully".KeyToWord();
                flag = true;
            }
            catch
            {
                flag = false;
                msg = "submit_information_is_not_complete".KeyToWord();
            }
            return new AjaxResult(new { flag = flag, msg = msg });
        }


        [Route("goods/getcustomproductlist")]
        [HttpPost]
        public ActionResult MerchantGetCustomProductList(string keywords = "")
        {
            var result = _merchantRepository.GetProducts(_auth.GetAuth().MerchantId, keywords,1,9999,false);
            return new AjaxResult(new { result = result });
        }

        [Route("goods/deletecustomproduct")]
        [HttpPost]
        public ActionResult MerchantDeleteCustomProduct(string key)
        {
            _merchantRepository.RemoveProduct(_auth.GetAuth().MerchantId, key);
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
            
            var ciqProductList = _merchantRepository.GetProducts(_auth.GetAuth().MerchantId, keyWords, 1,
                string.IsNullOrEmpty(keyWords) ? 10 : 10000, true);
            var customProductList = _merchantRepository.GetProducts(_auth.GetAuth().MerchantId, string.Empty,1,9999,false);
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
                    throw new Exception("the_record_number_does_not_exist".KeyToWord());
                }
                if (!ciqProduct.IsApproved)
                {
                    if (string.IsNullOrEmpty(enName))
                    {
                        throw new Exception("please_improve_the_english_name_of_goods".KeyToWord());
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
                _merchantRepository.RemoveProduct(_auth.GetAuth().MerchantId, "");
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