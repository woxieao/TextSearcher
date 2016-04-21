using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Giqci.Chapi.Enums.App;
using Giqci.Chapi.Models.App;
using Giqci.Interfaces;
using Giqci.PublicWeb.Helpers;
using Giqci.PublicWeb.Models;
using Giqci.PublicWeb.Services;
using Ktech.Extensions;
using Ktech.Mvc.ActionResults;
using Newtonsoft.Json;
using Application = Giqci.Chapi.Models.App.Application;

namespace Giqci.PublicWeb.Controllers
{
    [Authorize]
    public class FormsController : Ktech.Mvc.ControllerBase
    {
        private readonly IMerchantApiProxy _merchantRepo;
        private readonly IDictService _cache;
        private readonly IMerchantApplicationApiProxy _appRepo;
        private readonly IProductApiProxy _prodApi;
        private readonly IAuthService _auth;
        private readonly IDataChecker _dataChecker;

        public FormsController(IDictService cache,
            IMerchantApiProxy merchantRepo, IMerchantApplicationApiProxy appRepo, IProductApiProxy prodApi,
            IAuthService auth, IDataChecker dataChecker)
        {
            _merchantRepo = merchantRepo;
            _cache = cache;
            _appRepo = appRepo;
            _prodApi = prodApi;
            _auth = auth;
            _dataChecker = dataChecker;
        }

        [Route("forms/app/")]
        [HttpGet]
        public ActionResult InitApplication(string applicantCode)
        {
            var merchant = _merchantRepo.GetMerchant(User.Identity.Name);
            var model = new Application
            {
                ApplicantCode = applicantCode,
                Applicant = merchant.Name,
                ApplicantAddr = merchant.Address,
                ApplicantContact = merchant.Contact,
                ApplicantPhone = merchant.Phone,
                ApplicantEmail = merchant.Email,
                InspectionDate = DateTime.Now,
                ContainerInfos = new List<ContainerInfo>()
            };
            return View("Application", model);
        }


        [Route("forms/app/{appkey}")]
        [HttpGet]
        public ActionResult Application(string appkey)
        {
            var merchant = _merchantRepo.GetMerchant(User.Identity.Name);
            var application = _appRepo.Get(merchant.Id, appkey);
            //非该登录人的申请||不是新申请
            if (application == null)
            {
                throw new ApplicationException(":(   You Can Not View This Application");
            }
            switch (application.Status)
            {
                case ApplicationStatus.New:
                {
                    return View(application);
                }
                default:
                {
                    return View("ViewApp", application);
                }
            }
        }


        [Route("forms/app")]
        [HttpPost]
        public ActionResult SubmitApplication(Application model, string appkey = "")
        {
            var errors = _dataChecker.ApplicationHasErrors(model);
            string appNo = null;
            var userName = User.Identity.Name;
            var isLogin = true;
            if (string.IsNullOrEmpty(userName))
            {
                isLogin = false;
                errors = new List<string>() {"登录状态已失效，请您重新登录系统"};
            }
            var merchantId = _merchantRepo.GetMerchant(User.Identity.Name).Id;
            if (!errors.Any())
            {
                GetTotalUnits(ref model);
                if (!string.IsNullOrEmpty(appkey))
                {
                    _appRepo.Update(merchantId, appkey, model);
                }
                else
                {
                    appNo = _appRepo.CreateApplication(_auth.GetAuth().MerchantId, model);
                }
                errors = null;
            }
            return new KtechJsonResult(HttpStatusCode.OK,
                new {appNo = appNo, appkey = appkey, isLogin = isLogin, errors = errors});
        }


        [Route("forms/list")]
        [HttpGet]
        public ActionResult UserFormsList()
        {
            return View();
        }

        protected static void GetTotalUnits(ref Application input)
        {
            input.TotalUnits = input.ShippingMethod == ShippingMethod.O
                ? (input.ContainerInfos == null ? 0 : input.ContainerInfos.Count())
                : input.TotalUnits;
        }
    }
}