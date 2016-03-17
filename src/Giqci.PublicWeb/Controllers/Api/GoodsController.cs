using Giqci.Repositories;
using Ktech.Mvc.ActionResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Giqci.Models;
using Giqci.PublicWeb.Models.Goods;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("api")]
    public class GoodsController : Controller
    {
        private readonly IGoodsRepository _repo;
        private readonly IDictionaryRepository _repoDictionary;

        public GoodsController(IGoodsRepository repo, IDictionaryRepository repoDictionary)
        {
            _repo = repo;
            _repoDictionary = repoDictionary;
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
            var result = _repo.DeleteGoods(id);
            return new KtechJsonResult(HttpStatusCode.OK, new {result = result});
        }
    }
}