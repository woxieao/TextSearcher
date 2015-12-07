using System.Web.Mvc;
using System.Web.Security;
using Giqci.Models;
using Giqci.PublicWeb.Models.Account;
using Giqci.Repositories;

namespace Giqci.PublicWeb.Controllers
{
    [RoutePrefix("account")]
    public class AccountController : Controller
    {
        private IMerchantRepository _repo;

        public AccountController(IMerchantRepository repo)
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
    }
}