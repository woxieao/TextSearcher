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

        public AjaxResult(object data)
        {
            _ajaxResultPackage.Data = data;
        }

        public AjaxResult(object data, string msg) : this(data)
        {
            _ajaxResultPackage.Msg = msg;
        }

        public AjaxResult(object data, string msg, RequestStatus flag) : this(data, msg)
        {
            _ajaxResultPackage.Flag = flag;
            _ajaxResultPackage.Msg = msg;
        }
        public AjaxResult(object data, string msg, RequestStatus flag, CallBackClass callBackClass) : this(data, msg, flag)
        {
            _ajaxResultPackage.CallBackPackage = callBackClass;
            _ajaxResultPackage.Msg = msg;
            _ajaxResultPackage.Flag = flag;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;
            response.TrySkipIisCustomErrors = true;
            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentType = "application/json";
            if (_ajaxResultPackage == null)
                return;
            response.Write(JsonConvert.SerializeObject(_ajaxResultPackage, new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter> { new StringEnumConverter() },
                NullValueHandling = NullValueHandling.Ignore
            }));
        }
    }
}