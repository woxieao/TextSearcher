using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;
using Giqci.Models;
using Giqci.PublicWeb.Models;
using Giqci.PublicWeb.Models.Account;
using Giqci.Repositories;

namespace Giqci.PublicWeb.Controllers
{
    [RoutePrefix("account")]
    public class AccountController : Controller
    {
        private IGiqciRepository _repo;

        public AccountController(IGiqciRepository repo)
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
        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [Route("reg")]
        [HttpGet]
        public ActionResult Registration()
        {
            var model = new RegistrationViewModel { Applicants = new List<Applicant> { new Applicant() } };
            return View(model);
        }

        [Route("reg")]
        [HttpPost]
        public ActionResult Registration(RegistrationViewModel input)
        {
            if (string.IsNullOrEmpty(input.Username) || input.Username.Length < 3)
            {
                input.ErrorMessage = "Invalid Username";
                return View(input);
            }
            if (string.IsNullOrEmpty(input.Password) || input.Password.Length < 3)
            {
                input.ErrorMessage = "Invalid Password";
                return View(input);
            }
            if (!input.Password.Equals(input.ConfirmPassword))
            {
                input.ErrorMessage = "Two Password are not identical";
                return View(input);
            }
            if (input.Applicants.Count < 1)
            {
                input.ErrorMessage = "You need at least one applicant";
                return View(input);
            }
            _repo.RegMerchant(input);
            FormsAuthentication.SetAuthCookie(input.Username, true);
            return Redirect("/");
        }
    }
}