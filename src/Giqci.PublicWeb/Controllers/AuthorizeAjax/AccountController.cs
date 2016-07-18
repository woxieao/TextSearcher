using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Giqci.Chapi.Models.Customer;
using Giqci.Interfaces;
using Giqci.PublicWeb.Extensions;
using Giqci.PublicWeb.Models.Account;
using Giqci.PublicWeb.Models.Ajax;
using Giqci.PublicWeb.Services;

namespace Giqci.PublicWeb.Controllers.AuthorizeAjax
{
    [RoutePrefix("{languageType}/api")]
    [AjaxAuthorize]
    public class AccountController : AjaxController
    {
        private readonly IMerchantApiProxy _repo;

        private readonly IAuthService _auth;

        public AccountController(IMerchantApiProxy repo, IAuthService auth)
        {
            _repo = repo;
            _auth = auth;
        }


        [Route("account/updateprofile")]
        [HttpPost]
        public ActionResult UpdateProfile(Merchant model)
        {
            bool result = true;
            string message = "";
            try
            {
                _repo.Update(_auth.GetAuth().MerchantId, model);
            }
            catch (Exception ex)
            {
                result = false;
                message = ex.Message;
            }
            return new AjaxResult(new { result = result, message = message });
        }

        [Route("account/chanagepassword")]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordPageModel model)
        {
            bool result = true;
            string message = "";
            try
            {
                var reg = new Regex("^[a-z0-9A-Z]{6,20}$");
                if (!reg.IsMatch(model.NewPassword))
                {
                    throw new AjaxException("password_can_only_be_numbers_or_lett".KeyToWord());
                }
                result = _repo.ChangePassword(_auth.GetAuth().MerchantId, model.OldPassword, model.NewPassword);
            }
            catch (Exception ex)
            {
                result = false;
                message = ex.Message;
            }
            return new AjaxResult(new { result = result, message = message });
        }
    }
}