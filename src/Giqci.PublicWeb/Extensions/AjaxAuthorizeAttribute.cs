using System.Net;
using System.Web.Mvc;
using Giqci.PublicWeb.Models.Ajax;
using Ktech.Mvc.ActionResults;

namespace Giqci.PublicWeb.Extensions
{
    public class AjaxAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new KtechJsonResult(HttpStatusCode.OK, new AjaxResultPackage
            {
                Flag = RequestStatus.LogOut,
                Msg = "登录状态已失效,请重新登陆",
            });
        }
    }
}