using System.Net;
using System.Web.Mvc;
using Giqci.PublicWeb.Models.Ajax;
using Ktech.Mvc.ActionResults;
using Newtonsoft.Json;

namespace Giqci.PublicWeb.Extensions
{
    public class AjaxAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new KtechJsonResult(HttpStatusCode.OK, new AjaxResultPackage
            {
                status = RequestStatus.LogOut,
                msg = "登录状态已失效,请重新登陆",
            });
        }
    }
}