using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Giqci.PublicWeb.Models.Ajax
{
    public class AjaxResult : ActionResult
    {
        private readonly AjaxResultPackage _ajaxResultPackage = new AjaxResultPackage();
        private readonly JsonSerializerSettings _settings;

        public AjaxResult(object data)
        {
            _ajaxResultPackage.status = RequestStatus.Success;
            _ajaxResultPackage.data = data;
            _ajaxResultPackage.msg = "success";
            _settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new StringEnumConverter() },
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public AjaxResult(object data, JsonSerializerSettings settings) : this(data)
        {
            _settings = settings;
        }

        public AjaxResult(AjaxResultPackage package)
        {
            _ajaxResultPackage = package;
        }
        public AjaxResult(AjaxResultPackage package, JsonSerializerSettings settings) : this(package)
        {
            _settings = settings;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;
            response.TrySkipIisCustomErrors = true;
            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentType = "application/json";
            if (_ajaxResultPackage == null)
                return;
            response.Write(JsonConvert.SerializeObject(_ajaxResultPackage, _settings));
        }
    }
}