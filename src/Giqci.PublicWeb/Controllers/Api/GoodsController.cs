using Giqci.Repositories;
using Ktech.Mvc.ActionResults;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using Giqci.ApiProxy.Services;
using Giqci.Interfaces;
using Giqci.PublicWeb.Models.Goods;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("api")]
    public class GoodsController : Controller
    {
        private readonly IProductApiProxy _prodApi;
        private readonly IDictService _dict;
        private readonly IMerchantRepository _merchant;

        public GoodsController(IProductApiProxy prodApi, IMerchantRepository merchant, IDictService dict)
        {
            _prodApi = prodApi;
            _dict = dict;
            _merchant = merchant;
        }

        //[Route("goods/list")]
        //[HttpPost]
        //public ActionResult GoodsList(string keywords, int pageIndex = 1, int pageSize = 10)
        //{
        //    var goodsList = _repo.SearchProducts(User.Identity.Name, keywords, pageIndex, pageSize);
        //    var count = goodsList.Count;
        //    var goodsModelList = new List<ProductPageModel>();
        //    foreach (var goods in goodsList)
        //    {
        //        goodsModelList.Add(new ProductPageModel
        //        {
        //            Goods = goods,
        //            HSCodeDesc = _dict.GetHSCode(goods.HSCode).Name,
        //            ManufacturerCountryDesc = _dict.GetCountry(goods.ManufacturerCountry).CnName
        //        });
        //    }
        //    return new KtechJsonResult(HttpStatusCode.OK, new { items = goodsModelList, count = count });
        //}


        [Route("goods/delete")]
        [HttpPost]
        public ActionResult Delete(string code)
        {
            // TODO: remove by code rather than id
            _merchant.RemoveProduct(User.Identity.Name, code);
            return new KtechJsonResult(HttpStatusCode.OK, new { result = true });
        }
    }
}