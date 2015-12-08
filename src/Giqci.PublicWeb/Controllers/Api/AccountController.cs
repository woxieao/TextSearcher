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
                    string active_url = string.Format(@"{0}account/active?code={1}&email={2}", WebConfigurationManager.AppSettings["Host"], authCode.ToString(), input.Email);
                    var msg = new NoReplyEmail
                    {
                        FromEmail = Config.Current.NoReplyEmail,
                        Subject = Config.Current.RegMerchantEmailSubject,
                        TextTemplate = string.Format(
@"尊敬的用户,您好！您已经注册成功，请打开以下地址，并通过认证。{0}", active_url),

                        HtmlTemplate = string.Format(@"<!DOCTYPE HTML PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>
<html xmlns='http://www.w3.org/1999/xhtml'>
<head>
    <meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />
    <title>用户认证</title>
    <meta name='viewport' content='width=device-width, initial-scale=1.0' />
</head>
<body style='margin: 0; padding: 0;'>
    <table border='0' cellpadding='0' cellspacing='0' width='100%'>
        <tr>
            <td style='padding: 10px 0 30px 0;'>
                <table align='center' border='0' cellpadding='0' cellspacing='0' width='800' style='border: 1px solid #cccccc; border-collapse: collapse;'>
                    <tr>
                        <td align='left' bgcolor='#eee' style='padding: 20px 5px; color: #153643; font-size: 28px; font-weight: bold;'>
                            <img src='http://www.giqci.com/wp-content/themes/giqci/img/giqci-logo.png' alt='' width='380' height='61' style='display: block;' />
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 40px 30px 40px 30px;'>
                            <table border='0' cellpadding='0' cellspacing='0' width='100%'>
                                <tr>
                                    <td style='color: #153643; font-size: 24px;'>
                                        <p style='font-size: 18px; color: #4d4d4d; line-height: 1.5; margin-left: 40px; font-family: Microsoft YaHei; '>
                                            <b>尊敬的用户,您好！</b>
                                        </p>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <p style='font-size: 14px; color: #4d4d4d; line-height: 1.5; margin-left: 40px; font-family: Microsoft YaHei; '>
                                            很高兴您来到并加入，认证请点击以下按钮，
                                        </p>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <p style='font-size: 14px; color: #4d4d4d; line-height: 1.5; margin-left: 40px; font-family: Microsoft YaHei; '>
                                            <a href='{0}' target='_blank' style='background: none #0b76b7;text-decoration:none;padding:8px 14px;color:#fff;'>点击认证</a>
                                        </p>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <p style='font-size: 14px; color: #4d4d4d; line-height: 1.5; margin-left: 40px; font-family: Microsoft YaHei; '>
                                            如果点击无效，请复制下方网页地址到浏览器地址栏中打开：<br />
                                            <span style='font-size:12px;'><a href='{1}'>{2}</a></span>
                                        </p>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <p style='font-size: 14px; color: #4d4d4d; line-height: 1.5; margin-left: 40px; font-family: Microsoft YaHei; '>
                                            如果您错误的收到此邮件，可以不处理该邮件。此为系统邮件，无需回复。
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td bgcolor='#0b76b7' style='padding:10px;'>
                            <table border='0' cellpadding='0' cellspacing='0' width='100%'>
                                <tr>
                                    <td style='color: #ffffff; font-family: Arial, sans-serif; font-size: 14px;' width='75%'>
                                        &copy; 2015 Copyright GIQCI<br />
                                    </td>
                                    <td align='right' width='25%'></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>", active_url, active_url, active_url)
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
                var msg = new NoReplyEmail
                {
                    FromEmail = Config.Current.NoReplyEmail,
                    Subject = Config.Current.FeedbackEmailSubject,
                    TextTemplate = string.Format(@"您的密码已经重置，请及时更新，新密码为:{0}", newpassword),
                    HtmlTemplate = string.Format(@"<!DOCTYPE HTML PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>
<html xmlns='http://www.w3.org/1999/xhtml'>
<head>
    <meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />
    <title>密码重置</title>
    <meta name='viewport' content='width=device-width, initial-scale=1.0' />
</head>
<body style='margin: 0; padding: 0;'>
    <table border='0' cellpadding='0' cellspacing='0' width='100%'>
        <tr>
            <td style='padding: 10px 0 30px 0;'>
                <table align='center' border='0' cellpadding='0' cellspacing='0' width='800' style='border: 1px solid #cccccc; border-collapse: collapse;'>
                    <tr>
                        <td align='left' bgcolor='#eee' style='padding: 20px 5px; color: #153643; font-size: 28px; font-weight: bold;'>
                            <img src='http://www.giqci.com/wp-content/themes/giqci/img/giqci-logo.png' alt='' width='380' height='61' style='display: block;' />
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 40px 30px 40px 30px;'>
                            <table border='0' cellpadding='0' cellspacing='0' width='100%'>
                                <tr>
                                    <td style='color: #153643; font-size: 24px;'>
                                        <p style='font-size: 18px; color: #4d4d4d; line-height: 1.5; margin-left: 40px; font-family: Microsoft YaHei; '>
                                            <b>尊敬的用户,您好！</b>
                                        </p>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <p style='font-size: 14px; color: #4d4d4d; line-height: 1.5; margin-left: 40px; font-family: Microsoft YaHei; '>
                                            您通过密码重置功能重置您的密码，请使用新密码登录，并及时修改您的密码。<br/>新密码如下：
                                        </p>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <p style='font-size: 14px; color: #4d4d4d; line-height: 1.5; margin-left: 40px; font-family: Microsoft YaHei; '>
                                            <a href='javascript:;' style='background: none #0b76b7;text-decoration:none;padding:8px 14px;color:#fff;'>{0}</a>
                                        </p>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <p style='font-size: 14px; color: #4d4d4d; line-height: 1.5; margin-left: 40px; font-family: Microsoft YaHei; '>
                                            如果您错误的收到此邮件，可以不处理该邮件。此为系统邮件，无需回复。
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td bgcolor='#0b76b7' style='padding:10px;'>
                            <table border='0' cellpadding='0' cellspacing='0' width='100%'>
                                <tr>
                                    <td style='color: #ffffff; font-family: Arial, sans-serif; font-size: 14px;' width='75%'>
                                        &copy; 2015 Copyright GIQCI<br />
                                    </td>
                                    <td align='right' width='25%'></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>", newpassword)
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
