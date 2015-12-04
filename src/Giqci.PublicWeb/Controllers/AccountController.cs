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

        [Route("login")]
        [HttpPost]
        public ActionResult Login(LoginViewModel input)
        {
            if (_repo.MerchantLogin(input.Username, input.Password))
            {
                FormsAuthentication.SetAuthCookie(input.Username, true);
                return Redirect("/");
            }
            input.ErrorMessage = "Invalid Username or Password!";
            return View(input);
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

        [Route("profile")]
        [HttpPost]
        [Authorize]
        public ActionResult UpdateProfile(MerchantViewModel model)
        {
            _repo.UpdateMerchant(User.Identity.Name, model);
            return Redirect("/account/profile");
        }

        [Route("password")]
        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Route("password")]
        [HttpPost]
        [Authorize]
        public ActionResult ChangePassword(ChangePasswordPageModel model)
        {
            _repo.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
            return Redirect("/account/password");
        }
    }
}