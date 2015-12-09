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
            Guid authCode;
            try
            {
                result = _repo.RegMerchant(input, out authCode, out message);
                string active_string = FileHelper.GetInterIDList(@"~/App_Data/Active.txt");
                if (result)
                {
                    string active_url = string.Format(@"{0}account/active?code={1}&email={2}", WebConfigurationManager.AppSettings["Host"], authCode.ToString(), input.Email);
                    var msg = new SendEmailTemplate
                    {
                        FromEmail = Config.Current.NoReplyEmail,
                        Subject = Config.Current.RegMerchantEmailSubject,
                        TextTemplate = string.Format(@"尊敬的用户,您好！您已经注册成功，请打开以下地址，并通过认证。{0}", active_url),

                        HtmlTemplate = string.Format(active_string, active_url, active_url, active_url)
                    };
                    var m = new SmartMail(msg);
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
            bool result = true;
            string message = "";
            string newpassword = "";
            try
            {
                result = _repo.ResetPassword(model.Email, out newpassword);
                string forgot_string = FileHelper.GetInterIDList(@"~/App_Data/ForgotPassword.txt");
                var msg = new SendEmailTemplate
                {
                    FromEmail = Config.Current.NoReplyEmail,
                    Subject = Config.Current.FeedbackEmailSubject,
                    TextTemplate = string.Format(@"您的密码已经重置，请及时更新，新密码为:{0}", newpassword),
                    HtmlTemplate = string.Format(forgot_string, newpassword)
                };
                var m = new SmartMail(msg);
                m.To.Add(model.Email);
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
