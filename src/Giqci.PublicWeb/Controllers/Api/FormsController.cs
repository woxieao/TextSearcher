using Giqci.Enums;
using Giqci.Repositories;
using Ktech.Mvc.ActionResults;
using System;
using System.Net;
using System.Web.Mvc;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("api")]
    public class FormsController : Controller
    {
        private IApplicationRepository _repo;
        public FormsController(IApplicationRepository repo)
        {
            _repo = repo;
        }


        [Route("forms/search")]
        [HttpPost]
        public ActionResult GetAllItem(string applyNo, ApplicationStatus? status, DateTime? start, DateTime? end, int pageIndex = 1, int pageSize = 10)
        {
            var model = _repo.SearchApplications(applyNo, status, start, end, pageIndex, pageSize);

            var count = model.Count;

            return new KtechJsonResult(HttpStatusCode.OK, new { items = model, count = count });
        }
    }
}