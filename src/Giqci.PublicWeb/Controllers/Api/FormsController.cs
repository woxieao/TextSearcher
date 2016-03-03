using Giqci.Enums;
using Giqci.PublicWeb.Converters;
using Giqci.Repositories;
using Ktech.Mvc.ActionResults;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ktech.Extensions;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("api")]
    public class FormsController : Controller
    {
        private readonly IApplicationRepository _repo;

        public FormsController(IApplicationRepository repo)
        {
            _repo = repo;
        }

        [Route("forms/list")]
        [HttpPost]
        public ActionResult GetAllItem(string applyNo, ApplicationStatus? status, DateTime? start, DateTime? end, int pageIndex = 1, int pageSize = 10)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return new KtechJsonResult(HttpStatusCode.OK, new { count = 0 });
            }
            var model = _repo.SearchApplications(applyNo, User.Identity.Name, status, start, end, pageIndex, pageSize);

            var count = model.Count;

            return new KtechJsonResult(HttpStatusCode.OK, new { items = model, count = count }, new JsonSerializerSettings { Converters = new List<JsonConverter> { new DescriptionEnumConverter() } });
        }

        [Route("forms/getstatus")]
        [HttpGet]
        public ActionResult GetStatus()
        {
            var statusValues = Enum.GetValues(typeof(ApplicationStatus)).Cast<ApplicationStatus>().Select(i => new { value = i.ToString(), name = i.ToDescription() }).ToArray();
            return new KtechJsonResult(HttpStatusCode.OK, new { items = statusValues });
        }
    }
}