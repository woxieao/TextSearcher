using System.Web.Mvc;
using Giqci.PublicWeb.Models;
using Giqci.PublicWeb.Models.Ajax;

namespace Giqci.PublicWeb.Extensions
{
    public class AjaxController : Controller
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            var ex = filterContext.Exception;
            var errorMsg = Config.Common.HideServiceError ? (ex is AjaxException ? ex.Message : "服务器异常") : ex.Message;
            filterContext.HttpContext.Response.Clear();
            filterContext.Result = new AjaxResult(null);
            filterContext.Result = new AjaxResult(new AjaxResultPackage
            {
                status = 0,
                msg = errorMsg,
            });
            filterContext.ExceptionHandled = true;
        }
    }
}