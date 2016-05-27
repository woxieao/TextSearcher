﻿using System;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using Giqci.Chapi.Models.Customer;
using Giqci.Interfaces;
using Giqci.PublicWeb.Extensions;
using Giqci.PublicWeb.Helpers;
using Giqci.PublicWeb.Models;
using Giqci.PublicWeb.Models.Account;
using Giqci.PublicWeb.Services;
using Ktech.Core.Mail;
using Ktech.Mvc.ActionResults;

namespace Giqci.PublicWeb.Controllers.AuthorizeAjax
{
    [RoutePrefix("api")]
    [AjaxAuthorize]
    public class AccountController : AjaxController
    {
        private readonly IMerchantApiProxy _repo;

        private readonly IAuthService _auth;

        public AccountController(IMerchantApiProxy repo, IAuthService auth)
        {
            _repo = repo;
            _auth = auth;
        }


        [Route("account/updateprofile")]
        [HttpPost]
        public ActionResult UpdateProfile(Merchant model)
        {
            bool result = true;
            string message = "";
            try
            {
                var auth = _auth.GetAuth();
                if (auth == null)
                {
                    FormsAuthentication.SignOut();
                    return Redirect("~/account/login");
                }
                _repo.Update(auth.MerchantId, model);
            }
            catch (Exception ex)
            {
                result = false;
                message = ex.Message;
            }
            return new KtechJsonResult(HttpStatusCode.OK, new { result = result, message = message });
        }

        [Route("account/chanagepassword")]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordPageModel model)
        {
            bool result = true;
            string message = "";
            try
            {
                var auth = _auth.GetAuth();
                if (auth == null)
                {
                    FormsAuthentication.SignOut();
                    return Redirect("~/account/login");
                }
                result = _repo.ChangePassword(auth.MerchantId, model.OldPassword, model.NewPassword);
            }
            catch (Exception ex)
            {
                result = false;
                message = ex.Message;
            }
            return new KtechJsonResult(HttpStatusCode.OK, new { result = result, message = message });
        }
    }
}