using System.Web.Mvc;
using System.Web.Security;
using Giqci.PublicWeb.Models.Account;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Giqci.Interfaces;

namespace Giqci.PublicWeb.Controllers
{
    [RoutePrefix("account")]
    public class AccountController : Controller
    {
        private IMerchantApiProxy _repo;

        public AccountController(IMerchantApiProxy repo)
        {
            _repo = repo;
            string tempIP = GetIP();
            WebRequest wr = WebRequest.Create("http://test.ip138.com/query/?ip=117.25.13.123&datatype=text");
            Stream s = wr.GetResponse().GetResponseStream();
            StreamReader sr = new StreamReader(s, Encoding.Default);
            string all = sr.ReadToEnd(); //读取网站的数据
        }

        private static string GetIP()
        {
            string tempip = "";
            try
            {
                WebRequest wr = WebRequest.Create("http://www.ip138.com/ips138.asp");
                Stream s = wr.GetResponse().GetResponseStream();
                StreamReader sr = new StreamReader(s, Encoding.Default);
                string all = sr.ReadToEnd(); //读取网站的数据

                int start = all.IndexOf("您的IP地址是：[") + 9;
                int end = all.IndexOf("]", start);
                tempip = all.Substring(start, end - start);
                sr.Close();
                s.Close();
            }
            catch
            {
            }
            return tempip;
        }

        [Route("login")]
        [HttpGet]
        public ActionResult Login()
        {
            if (!string.IsNullOrEmpty(User.Identity.Name))
            {
                return Redirect("/");
            }
            var model = new LoginViewModel();
            return View(model);
        }

        [Route("signoff")]
        [Authorize]
        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [Route("reg")]
        [HttpGet]
        public ActionResult Registration()
        {
            var model = new RegistrationViewModel();
            return View(model);
        }

        [Route("profile")]
        [HttpGet]
        [Authorize]
        public ActionResult Profile()
        {
            var m = _repo.GetMerchant(User.Identity.Name);
            return View(m);
        }

        [Route("password")]
        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Route("forgot")]
        [HttpGet]
        public ActionResult Forgot()
        {
            return View();
        }

        [Route("active")]
        [HttpGet]
        public ActionResult ActiveMerchant(Guid code, string email)
        {
            bool result = false;
            string message;
            try
            {
                result = _repo.Activate(email, code, out message);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                message = ex.Message;
            }
            ViewBag.result = result;
            ViewBag.message = message;
            return View();
        }

        [Authorize]
        [Route("merchant")]
        [HttpGet]
        public ActionResult MerchantList()
        {
            return View();
        }
    }
}