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
using Giqci.PublicWeb.Helpers;

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
            try
            {
                Guid authCode;
                result = _repo.RegMerchant(input, out authCode, out message);
                if (result)
                {
                    var msg = new SendEmailTemplate
                    {
                        FromEmail = Config.Current.NoReplyEmail,
                        Subject = Config.Current.RegMerchantEmailSubject,
                        TextTemplate = EmailTemplateHelper.GetEmailTemplate("Active.txt"),
                        HtmlTemplate = EmailTemplateHelper.GetEmailTemplate("Active.html")
                    };
                    var m = new SmartMail(msg);
                    m.AddParameter("ActiveUrl",
                        string.Format(@"{0}account/active?code={1}&email={2}", Config.Current.Host, authCode, input.Email));
                    m.To.Add(input.Email);
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
                        FromEmail = Config.Current.NoReplyEmail,
                        Subject = Config.Current.FeedbackEmailSubject,
                        TextTemplate = EmailTemplateHelper.GetEmailTemplate("ForgotPassword.txt"),
                        HtmlTemplate = EmailTemplateHelper.GetEmailTemplate("ForgotPassword.html"),
                    };
                    var m = new SmartMail(msg);
                    m.AddParameter("Password", newPassword);
                    m.To.Add(model.Email);
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
    }
}
