using Giqci.Repositories;
using Ktech.Mvc.ActionResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Giqci.PublicWeb.Controllers.Api
{

    [RoutePrefix("api")]
    public class GoodsController : Controller
    {
        private readonly IGoodsRepository _repo;

        public GoodsController(IGoodsRepository repo)
        {
            _repo = repo;
        }

        [Route("goods/list")]
        [HttpPost]
        public ActionResult GoodsList(string keywords, int pageIndex = 1, int pageSize = 10)
        {
            var model = _repo.SearchGoods(User.Identity.Name, keywords, pageIndex, pageSize);

            var count = model.Count;

            return new KtechJsonResult(HttpStatusCode.OK, new { items = model, count = count });
        }
    }
}