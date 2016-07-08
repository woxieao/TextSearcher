﻿using System;
using System.Net;
using System.Web.Mvc;
using Giqci.Chapi.Models.Customer;
using Giqci.Interfaces;
using Giqci.PublicWeb.Extensions;
using Giqci.PublicWeb.Helpers;
using Giqci.PublicWeb.Models;
using Giqci.PublicWeb.Models.Account;
using Giqci.PublicWeb.Models.Ajax;
using Giqci.PublicWeb.Services;
using Ktech.Core.Mail;
using Ktech.Mvc.ActionResults;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("{languageType}/api")]
    //[RoutePrefix("api")]
    public class AccountController : AjaxController
    {
        private readonly IMerchantApiProxy _repo;

        private readonly IAuthService _auth;

        public AccountController(IMerchantApiProxy repo, IAuthService auth)
        {
            _repo = repo;
            _auth = auth;
        }

        [Route("account/reg")]
        [HttpPost]

        public ActionResult Register(Merchant input, string Password)
        {
            bool result;
            string message;
            try
            {
                Guid authCode;
                result = _repo.Register(input, Password, out authCode, out message);
                if (result)
                {
                    var msg = new SendEmailTemplate
                    {
                        FromEmail = Config.Common.NoReplyEmail,
                        Subject = Config.Common.RegMerchantEmailSubject,
                        TextTemplate = EmailTemplateHelper.GetEmailTemplate("Active.txt"),
                        HtmlTemplate = EmailTemplateHelper.GetEmailTemplate("Active.html")
                    };
                    var m = new SmartMail(msg);
                    m.AddParameter("ActiveUrl",
                        string.Format(@"{0}account/active?code={1}&email={2}", Config.Common.Host, authCode,
                            input.Email));
                    m.To.Add(input.Email);
                    m.SendEmail();
                }
            }
            catch (Exception ex)
            {
                result = false;
                message = ex.Message;
            }
            return new AjaxResult(new { result = result, message = message });
        }



        [Route("account/login")]
        [HttpPost]
        public ActionResult Login(LoginViewModel input)
        {
            bool result = true;
            string message = "";
            var m = _repo.MerchantLogin(input.Email, input.Password);
            if (m != null)
            {
                _auth.SetAuth(m);
            }
            else
            {
                result = false;
                message = "用户名或密码错误!";
            }
            return new AjaxResult(new { result = result, message = message });
        }



        [Route("account/forgotpassword")]
        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            var result = true;
            var message = "";
            try
            {
                string newPassword;
                result = _repo.ResetPassword(model.Email, out newPassword);
                if (result)
                {
                    var msg = new SendEmailTemplate
                    {
                        FromEmail = Config.Common.NoReplyEmail,
                        Subject = Config.Common.ForgotPasswordEmailSubject,
                        TextTemplate = EmailTemplateHelper.GetEmailTemplate("ForgotPassword.txt"),
                        HtmlTemplate = EmailTemplateHelper.GetEmailTemplate("ForgotPassword.html"),
                    };
                    var m = new SmartMail(msg);
                    m.AddParameter("Password", newPassword);
                    m.To.Add(model.Email);
                    m.SendEmail();
                }
                else
                {
                    result = false;
                    message = "邮箱不存在";
                }
            }
            catch (Exception ex)
            {
                result = false;
                message = ex.Message;
            }
            return new AjaxResult(new { result = result, message = message });
        }

        [Route("account/heartbeat")]
        [HttpPost]
        public ActionResult HeartBeat()
        {
            if (User.Identity.IsAuthenticated)
            {
                _auth.Renew();
            }
            return new AjaxResult(new { result = true, message = "" });
        }


        [Route("account/breath")]
        [HttpPost]
        public ActionResult Breath()
        {
            // _auth.Renew();
            return new AjaxResult(new { flag = User.Identity.IsAuthenticated });
        }
    }
}