using System;
using System.Collections.Generic;
using System.Linq;
using Ktech.Mvc.ActionResults;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using Giqci.Chapi.Enums.App;
using Giqci.Chapi.Models.App;
using Giqci.Interfaces;
using Giqci.PublicWeb.Services;
using Giqci.Validations;
using Newtonsoft.Json;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("api/UserProfile")]
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly IAuthService _auth;
        private readonly IUserProfileApiProxy _userProfileApiProxy;

        public UserProfileController(IAuthService auth, IUserProfileApiProxy userProfileApiProxy)
        {
            _auth = auth;
            _userProfileApiProxy = userProfileApiProxy;
        }

        [Route("GetProfileList")]
        [HttpPost]
        public ActionResult GetProfileList()
        {
            var auth = _auth.GetAuth();
            if (auth == null)
            {
                FormsAuthentication.SignOut();
                return Redirect("~/account/login");
            }
            var profileList = _userProfileApiProxy.Select(auth.MerchantId);
            return new KtechJsonResult(HttpStatusCode.OK, new {result = profileList}, new JsonSerializerSettings());
        }

        [Route("AddProfile")]
        [HttpPost]
        public ActionResult AddProfile(UserProfile userProfile)
        {
            var errorMsg = new List<string>();
            try
            {
                var auth = _auth.GetAuth();
                if (auth == null)
                {
                    FormsAuthentication.SignOut();
                    return Redirect("~/account/login");
                }
                errorMsg = new UserProfileValidation().Validate(userProfile).Errors.Select(i => i.ErrorMessage).ToList();
                if (!errorMsg.Any())
                    _userProfileApiProxy.Add(auth.MerchantId, userProfile);
            }
            catch
            {
                errorMsg.Add("提交异常");
            }
            return new KtechJsonResult(HttpStatusCode.OK, new {flag = !errorMsg.Any(), errorMsg = errorMsg});
        }

        [Route("UpdateProfile")]
        [HttpPost]
        public ActionResult UpdateProfile(UserProfile userProfile)
        {
            var errorMsg = new List<string>();
            try
            {
                var auth = _auth.GetAuth();
                if (auth == null)
                {
                    FormsAuthentication.SignOut();
                    return Redirect("~/account/login");
                }
                errorMsg = new UserProfileValidation().Validate(userProfile).Errors.Select(i => i.ErrorMessage).ToList();
                if (!errorMsg.Any())
                    _userProfileApiProxy.Update(auth.MerchantId, userProfile);
            }
            catch
            {
                errorMsg.Add("提交异常");
            }
            return new KtechJsonResult(HttpStatusCode.OK, new {flag = !errorMsg.Any(), errorMsg = errorMsg});
        }

        [Route("RemoveProfile")]
        [HttpPost]
        public ActionResult RemoveProfile(int profileId)
        {
            var flag = true;
            try
            {
                var auth = _auth.GetAuth();
                if (auth == null)
                {
                    FormsAuthentication.SignOut();
                    return Redirect("~/account/login");
                }
                _userProfileApiProxy.Remove(auth.MerchantId, profileId);
            }
            catch
            {
                flag = false;
            }
            return new KtechJsonResult(HttpStatusCode.OK, new {flag = flag});
        }

        [Route("GetProfileDeatil")]
        [HttpPost]
        public ActionResult GetProfileDeatil(int profileId)
        {
            UserProfile result;
            try
            {
                var auth = _auth.GetAuth();
                if (auth == null)
                {
                    FormsAuthentication.SignOut();
                    return Redirect("~/account/login");
                }
                result = _userProfileApiProxy.Get(auth.MerchantId, profileId);
            }
            catch
            {
                result = null;
            }
            return new KtechJsonResult(HttpStatusCode.OK, new {result = result}, new JsonSerializerSettings());
        }
    }
}