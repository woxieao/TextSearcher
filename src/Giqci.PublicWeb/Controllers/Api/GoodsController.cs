using Giqci.Repositories;
using Ktech.Mvc.ActionResults;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Giqci.Models;
using Giqci.PublicWeb.Models.Goods;
using Giqci.Services;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("api")]
    public class GoodsController : Controller
    {
        private readonly IGoodsRepository _repo;
        private readonly IDictionaryRepository _repoDictionary;
        private readonly IMerchantRepository _merchant;
        private readonly ICacheService _cacheService;

        public GoodsController(IGoodsRepository repo, IMerchantRepository merchant, IDictionaryRepository repoDictionary,
            ICacheService cacheService)
        {
            _repo = repo;
            _repoDictionary = repoDictionary;
            _merchant = merchant;
            _cacheService = cacheService;
        }

        [Route("goods/list")]
        [HttpPost]
        public ActionResult GoodsList(string keywords, int pageIndex = 1, int pageSize = 10)
        {
            var goodsList = _repo.SearchGoods(User.Identity.Name, keywords, pageIndex, pageSize);
            var count = goodsList.Count;
            var goodsModelList = new List<GoodsPageModel>();
            foreach (var goods in goodsList)
            {
                goodsModelList.Add(new GoodsPageModel
                {
                    Goods = goods,
                    HSCodeDesc = _repoDictionary.GetHSCode(goods.HSCode).Name,
                    ManufacturerCountryDesc =
                        _repoDictionary.GetCountryDictionaryByCode(goods.ManufacturerCountry).CnName
                });
            }
            return new KtechJsonResult(HttpStatusCode.OK, new {items = goodsModelList, count = count});
        }


        [Route("goods/delete")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var merchant = _merchant.GetMerchant(User.Identity.Name);
            var goods = _repo.GetGoods(id);
            var result = false;
            if (merchant.Id != goods.MerchantId)
            {
                result = false;
            }
            else
            {
                result = _repo.DeleteGoods(id);
            }
            return new KtechJsonResult(HttpStatusCode.OK, new {result = result});
        }
    }
}