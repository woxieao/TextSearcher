using System.Web.Mvc;
using System.Web.Security;
using Giqci.PublicWeb.Models.Account;
using System;
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
        }

        [Route("login")]
        [HttpGet]
        public ActionResult Login()
        {
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
    }
}