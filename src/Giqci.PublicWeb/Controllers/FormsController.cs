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

        public FormsController(IDictService cache,
            IMerchantApiProxy merchantRepo, IMerchantApplicationApiProxy appRepo, IProductApiProxy prodApi,
            IAuthService auth)
        {
            _merchantRepo = merchantRepo;
            _cache = cache;
            _appRepo = appRepo;
            _prodApi = prodApi;
            _auth = auth;
        }

        [Route("forms/app/")]
        [HttpGet]
        public ActionResult Application(string applicantCode)
        {
            var merchant = _merchantRepo.GetMerchant(User.Identity.Name);
            ViewBag.id = 0;
            var model = new Application
            {
                ApplicantCode = applicantCode,
                Applicant = merchant.Name,
                ApplicantAddr = merchant.Address,
                ApplicantContact = merchant.Contact,
                ApplicantPhone = merchant.Phone,
                ApplicantEmail = merchant.Email,
                InspectionDate = DateTime.Now,
                ContainerInfos = new List<ContainerInfo>
                {
                    new ContainerInfo()
                    {
                        ContainerNumber = String.Empty,
                        SealNumber = String.Empty,
                    }
                }
            };
            return View(model);
        }


        [Route("forms/app/{id:int}")]
        [HttpGet]
        public ActionResult Application(int id)
        {
            var merchant = _merchantRepo.GetMerchant(User.Identity.Name);
            var application = _appRepo.Get(merchant.Id, id);
            //非该登录人的申请||不是新申请
            if (application == null)
            {
                throw new ApplicationException(":(   You Can Not View This Application");
            }
            ViewBag.id = id;
            switch (application.Status)
            {
                case ApplicationStatus.New:
                    {
                        var cookieHelper = new CookieHelper();
                        var exampleListStr = string.Empty;
                        foreach (var exampleCerat in application.ExampleCertList)
                        {
                            exampleListStr += string.Format("|{0}|", exampleCerat.CertFilePath);
                        }
                        cookieHelper.OverrideCookies(cookieHelper.ExampleFileListKeyName, exampleListStr);
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
        public ActionResult SubmitApplication(Application model, int id = 0)
        {
            //todo  var errors = _appRepo.HasError(model);
            var errors = new List<string>();
            string appNo = null;
            var userName = User.Identity.Name;
            var isLogin = true;
            if (string.IsNullOrEmpty(userName))
            {
                isLogin = false;
                errors = new List<string>() { "登录状态已失效，请您重新登录系统" };
            }

            var merchantId = _merchantRepo.GetMerchant(User.Identity.Name).Id;
            if (!errors.Any())
            {
                var exampleCertList = GetExampleList(true);
                model.ExampleCertList = exampleCertList;
                if (id > 0)
                {
                    _appRepo.Update(merchantId, id, model);
                }
                else
                {
                    appNo = _appRepo.CreateApplication(_auth.GetAuth().MerchantId, model);
                }
                errors = null;
            }
            return new KtechJsonResult(HttpStatusCode.OK, new { appNo = appNo, id = id, isLogin = isLogin, errors = errors });
        }

        [Route("forms/uploadexample")]
        [HttpPost]
        public ActionResult UploadExample(HttpPostedFileBase file0, HttpPostedFileBase file1, HttpPostedFileBase file2,
            HttpPostedFileBase file3, HttpPostedFileBase file4)
        {
            var cookie = new CookieHelper();
            var currentExampleListStr = cookie.GetExampleListStr();
            var pathList = string.Format("{0}{1}{2}{3}{4}{5}",
                FormatExampleCookieStr(SaveFile(file0)),
                FormatExampleCookieStr(SaveFile(file1)),
                FormatExampleCookieStr(SaveFile(file2)),
                FormatExampleCookieStr(SaveFile(file3)),
                FormatExampleCookieStr(SaveFile(file4)),
                currentExampleListStr);
            cookie.OverrideCookies(cookie.ExampleFileListKeyName, pathList);
            return new KtechJsonResult(HttpStatusCode.OK, new { });
        }

        public List<ExampleCert> GetExampleList(bool deleteExampleList = false)
        {
            var cookie = new CookieHelper();
            var exampleFilePathList = cookie.GetExampleListStr().Split('|');
            var list = new List<ExampleCert>();
            foreach (var exampleFilePath in exampleFilePathList)
            {
                if (!string.IsNullOrWhiteSpace(exampleFilePath))
                {
                    list.Add(new ExampleCert { CertFilePath = exampleFilePath });
                }
            }
            if (deleteExampleList)
            {
                cookie.DeleteExampleList();
            }
            return list;
        }

        private string FormatExampleCookieStr(string exampleStr)
        {
            return string.IsNullOrWhiteSpace(exampleStr) ? string.Empty : string.Format("|{0}|", exampleStr);
        }

        private string SaveFile(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var fileName = string.Format("{0}{1}", Guid.NewGuid().ToString("n"),
                    file.FileName.Substring(file.FileName.LastIndexOf(".", StringComparison.Ordinal)));
                var filePath = string.Format("/{0}/{1}", Config.Current.UserExampleFilePath, fileName);
                var fileRealPath = Server.MapPath(string.Format("~{0}", filePath));
                var fileInfo = new FileInfo(fileRealPath);
                if (fileInfo.Directory != null && !fileInfo.Directory.Exists)
                {
                    fileInfo.Directory.Create();
                }
                file.SaveAs(fileRealPath);
                return filePath;
            }
            else
            {
                return string.Empty;
            }
        }

        [Route("forms/list")]
        [HttpGet]
        public ActionResult UserFormsList()
        {
            return View();
        }
    }
}