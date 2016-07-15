using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Giqci.PublicWeb.Models.Ajax;
using Ktech.Mvc.ActionResults;
using Newtonsoft.Json;

namespace Giqci.PublicWeb.Extensions
{
    public class BaseAuthorizeAttribute : AuthorizeAttribute
    {
        protected override HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext)
        {
            return base.OnCacheAuthorization(httpContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.RequestContext.HttpContext.Response.Redirect("/" + LanCore.GetCurrentLanTypeStr() + "/account/login");
            return;
        }
    }
}