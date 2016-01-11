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
        private readonly IApplicationRepository _repo;

        public GoodsController(IApplicationRepository repo)
        {
            _repo = repo;
        }

        [Route("goods/list")]
        [HttpPost]
        public ActionResult GoodsList(string applyNo, int pageIndex = 1, int pageSize = 10)
        {
            var model = "";

            var count = 0;

            return new KtechJsonResult(HttpStatusCode.OK, new { items = model, count = count });
        }
    }
}