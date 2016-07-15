using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using Giqci.Chapi.Enums.Dict;
using Giqci.Chapi.Models.App;
using Giqci.Interfaces;
using Giqci.PublicWeb.Extensions;
using Giqci.PublicWeb.Models.Ajax;
using Giqci.PublicWeb.Services;
using Giqci.Validations;
using Ktech.Mvc.ActionResults;
using Newtonsoft.Json;

namespace Giqci.PublicWeb.Controllers.AuthorizeAjax
{
    [RoutePrefix("{languageType}/api/UserProfile")]
    [AjaxAuthorize]
    public class UserProfileController : AjaxController
    {
        private readonly IAuthService _auth;
        private readonly IUserProfileApiProxy _userProfileApiProxy;
        private readonly IDictService _dict;

        public UserProfileController(IAuthService auth, IDictService dict, IUserProfileApiProxy userProfileApiProxy)
        {
            _auth = auth;
            _userProfileApiProxy = userProfileApiProxy;
            _dict = dict;
        }

        [Route("GetProfileList")]
        [HttpPost]
        public ActionResult GetProfileList()
        {
            var profileList = _userProfileApiProxy.Select(_auth.GetAuth().MerchantId);
            return new AjaxResult(new { result = profileList }, new JsonSerializerSettings());
        }

        [Route("AddProfile")]
        [HttpPost]
        public ActionResult AddProfile(UserProfile userProfile, string languageType)
        {
            var errorMsg = new List<string>();
            try
            {
                errorMsg = new UserProfileValidation(_dict, LanCore.GetCurrentLanType()).Validate(userProfile).Errors.Select(i => i.ErrorMessage).ToList();
                if (!errorMsg.Any())
                    _userProfileApiProxy.Add(_auth.GetAuth().MerchantId, userProfile);
            }
            catch
            {
                errorMsg.Add("submit_exception".KeyToWord());
            }
            return new AjaxResult(new { flag = !errorMsg.Any(), errorMsg = errorMsg });
        }

        [Route("UpdateProfile")]
        [HttpPost]
        public ActionResult UpdateProfile(UserProfile userProfile, string languageType)
        {
            var errorMsg = new List<string>();
            try
            {
                errorMsg = new UserProfileValidation(_dict, LanCore.GetCurrentLanType()).Validate(userProfile).Errors.Select(i => i.ErrorMessage).ToList();
                if (!errorMsg.Any())
                    _userProfileApiProxy.Update(_auth.GetAuth().MerchantId, userProfile);
            }
            catch
            {
                errorMsg.Add("submit_exception".KeyToWord());
            }
            return new AjaxResult(new { flag = !errorMsg.Any(), errorMsg = errorMsg });
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
            return new AjaxResult(new { flag = flag });
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
            return new AjaxResult(new { result = result }, new JsonSerializerSettings());
        }
    }
}