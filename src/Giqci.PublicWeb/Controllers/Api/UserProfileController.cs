using System;
using System.Collections.Generic;
using System.Linq;
using Ktech.Mvc.ActionResults;
using System.Net;
using System.Web.Mvc;
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
        public ActionResult GetProfileList(UserType? userType)
        {
            var profileList = _userProfileApiProxy.Select(_auth.GetAuth().MerchantId, userType);
            return new KtechJsonResult(HttpStatusCode.OK, new {result = profileList}, new JsonSerializerSettings());
        }

        [Route("AddProfile")]
        [HttpPost]
        public ActionResult AddProfile(UserProfile userProfile)
        {
            var errorMsg = new List<string>();
            try
            {
                errorMsg = new UserProfileValidation().Validate(userProfile).Errors.Select(i => i.ErrorMessage).ToList();
                _userProfileApiProxy.Add(_auth.GetAuth().MerchantId, userProfile);
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
                errorMsg = new UserProfileValidation().Validate(userProfile).Errors.Select(i => i.ErrorMessage).ToList();
                _userProfileApiProxy.Update(_auth.GetAuth().MerchantId, userProfile);
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
                _userProfileApiProxy.Remove(_auth.GetAuth().MerchantId, profileId);
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
                result = _userProfileApiProxy.Get(_auth.GetAuth().MerchantId, profileId);
            }
            catch
            {
                result = null;
            }
            return new KtechJsonResult(HttpStatusCode.OK, new {result = result}, new JsonSerializerSettings());
        }
    }
}