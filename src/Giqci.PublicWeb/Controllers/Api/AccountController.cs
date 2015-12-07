using System.Net;
using System.Web.Mvc;
using Giqci.Repositories;
using Ktech.Mvc.ActionResults;
using System.Web.Security;
using System;
using Giqci.Models;
using Giqci.PublicWeb.Models.Account;
using Giqci.PublicWeb.Models;
using System.Web.Configuration;
using Ktech.Core.Mail;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("api")]
    public class AccountController : Controller
    {
        private IMerchantRepository _repo;

        public AccountController(IMerchantRepository repo)
        {
            _repo = repo;
        }

        [Route("account/reg")]
        [HttpPost]
        public ActionResult Register(MerchantReg input)
        {
            bool result;
            string message;
            Guid authCode;
            try
            {
                result = _repo.RegMerchant(input, out authCode, out message);
                if (result)
                {
                    FormsAuthentication.SetAuthCookie(input.Username, true);
                    var msg = new SendEmailTemplate
                    {
                        FromEmail = WebConfigurationManager.AppSettings["SendEmailFrom"],
                        Subject = WebConfigurationManager.AppSettings["RegMerchantEmailSubject"],
                        TextTemplate = string.Format(
@"您已经注册成功，请单击链接，<a href='{0}account/active?code={1}&email={2}'>去认证</a>。",
WebConfigurationManager.AppSettings["LocalHost"],
authCode.ToString(),
input.Email)
                    };
                    var m = new SmartMail(msg);
                    m.To.Add(WebConfigurationManager.AppSettings["AdminEmail"]);
                    m.SendEmail();
                }
            }
            catch (Exception ex)
            {
                result = false;
                message = ex.Message;
            }
            return new KtechJsonResult(HttpStatusCode.OK, new { result = result, message = message });
        }


        [Route("account/updateprofile")]
        [HttpPost]
        public ActionResult UpdateProfile(MerchantViewModel model)
        {
            bool result = true;
            string message = "";
            try
            {
                _repo.UpdateMerchant(User.Identity.Name, model);
            }
            catch (Exception ex)
            {
                result = false;
                message = ex.Message;
            }
            return new KtechJsonResult(HttpStatusCode.OK, new { result = result, message = message });
        }

        [Route("account/login")]
        [HttpPost]
        public ActionResult Login(LoginViewModel input)
        {
            bool result = true;
            string message = "";
            if (_repo.MerchantLogin(input.Email, input.Password))
            {
                FormsAuthentication.SetAuthCookie(input.Email, true);
            }
            else
            {
                result = false;
                message = "用户名或密码错误!";
            }
            return new KtechJsonResult(HttpStatusCode.OK, new { result = result, message = message });
        }

        [Route("account/chanagepassword")]
        [HttpPost]
        [Authorize]
        public ActionResult ChangePassword(ChangePasswordPageModel model)
        {
            bool result = true;
            string message = "";
            try
            {
                result = _repo.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
            }
            catch (Exception ex)
            {
                result = false;
                message = ex.Message;
            }
            return new KtechJsonResult(HttpStatusCode.OK, new { result = result, message = message });

        }
        [Route("account/forgotpassword")]
        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            bool result = true;
            string message = "";
            string newpassword = "";
            try
            {
                result = _repo.ResetPassword(model.Username, out newpassword);
                var msg = new SendEmailTemplate
                {
                    FromEmail = WebConfigurationManager.AppSettings["FeedbackEmailFrom"],
                    Subject = WebConfigurationManager.AppSettings["FeedbackEmailSubject"],
                    TextTemplate = string.Format("您的密码已经重置，请及时更新，新密码为:{0}", newpassword)
                };
                var m = new SmartMail(msg);
                m.To.Add(WebConfigurationManager.AppSettings["AdminEmail"]);
                m.SendEmail();
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
