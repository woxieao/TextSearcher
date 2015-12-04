using System.Net;
using System.Web.Configuration;
using System.Web.Mvc;
using Giqci.PublicWeb.Models;
using Giqci.PublicWeb.Models.Api;
using Ktech.Core.Mail;
using Giqci.Repositories;
using Ktech.Mvc.ActionResults;
using Newtonsoft.Json;
using System.Web.Security;
using System;

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
        public ActionResult Register(string username, string password, string name, string address, string contact, string phone)
        {
            Giqci.Models.MerchantReg reg = new Giqci.Models.MerchantReg();
            reg.Username = username;
            reg.Password = password;
            reg.Name = name;
            reg.Address = address;
            reg.Contact = contact;
            reg.Phone = phone;
            var result = true;
            var message = "";
            try
            {
                _repo.RegMerchant(reg);
                FormsAuthentication.SetAuthCookie(reg.Username, true);
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
