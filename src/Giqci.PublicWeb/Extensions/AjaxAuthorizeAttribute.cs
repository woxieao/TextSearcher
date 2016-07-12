﻿using System.Net;
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
            filterContext.Result = new AjaxResult( new AjaxResultPackage
            {
                status = -1,
                msg = "login_status_has_failed".KeyToWord(),
            });
        }
    }
}