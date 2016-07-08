using System.Web.Mvc;
using System.Web.Security;
using Giqci.PublicWeb.Models.Account;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Giqci.Chapi.Enums.Dict;
using Giqci.Interfaces;
using Giqci.PublicWeb.Extensions;
using Giqci.PublicWeb.Models;

namespace Giqci.PublicWeb.Controllers
{
    [RoutePrefix("{languageType}/account")]
    public class AccountController : Controller
    {
        private readonly IMerchantApiProxy _repo;

        public AccountController(IMerchantApiProxy repo)
        {
            _repo = repo;
        }


        [Route("login")]
        [HttpGet]
        public ActionResult Login(string languageType)
        {
            if (!string.IsNullOrEmpty(User.Identity.Name))
            {
                return Redirect("/" + languageType);
            }
            var model = new LoginViewModel();
            return View(model);
        }

        [Route("signoff")]
        [BaseAuthorize]
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
        [BaseAuthorize]
        public ActionResult Profile()
        {
            var m = _repo.GetMerchant(User.Identity.Name);
            return View(m);
        }

        [Route("password")]
        [HttpGet]
        [BaseAuthorize]
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

        [BaseAuthorize]
        [Route("merchant")]
        [HttpGet]
        public ActionResult MerchantList()
        {
            return View();
        }
    }
}