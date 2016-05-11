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
        private readonly ICertificateApiProxy _certRepo;

        public FormsController(IDictService cache,
            IMerchantApiProxy merchantRepo, IMerchantApplicationApiProxy appRepo, IProductApiProxy prodApi,
            IAuthService auth, IDataChecker dataChecker, ICertificateApiProxy certRepo)
        {
            _merchantRepo = merchantRepo;
            _cache = cache;
            _appRepo = appRepo;
            _prodApi = prodApi;
            _auth = auth;
            _dataChecker = dataChecker;
            _certRepo = certRepo;
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
                ApplicationProducts = new List<ApplicationProduct>
                {
                    new ApplicationProduct
                    {
                        HandlerType = HandlerType.Add,
                        ProductItemList = new List<ProductItem>
                        {
                            new ProductItem
                            {
                                HandlerType = HandlerType.Add,
                            }
                        }
                    },
                },
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
            ApplicationItemModifyAble(ref application);
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
                        var certs = _certRepo.Select(appkey);
                        var validCerts = certs.GroupBy(i => i.CertType);
                        ViewBag.ValidCerts =
                            validCerts.Select(certTypeList => certTypeList.OrderByDescending(i => i.SignedDate).First())
                                .ToList();
                        return View("ViewApp", application);
                    }
            }
        }


        [Route("forms/app")]
        [HttpPost]
        public ActionResult SubmitApplication(Application model)
        {
            var appkey = model.Key;
            var isNew = string.IsNullOrEmpty(appkey);
            bool isRequireCiqCode = false;
            if (!string.IsNullOrEmpty(model.DestPort))
            {
                var port = _cache.GetPort(model.DestPort);
                isRequireCiqCode = port.RequireCiqCode;
            }

            var errors = _dataChecker.ApplicationHasErrors(model, false, isRequireCiqCode);
            //todo 合并在ApplicationHasErrors中,但仅限前台网站
            if (model.InspectionDate < DateTime.Now.Date)
            {
                errors.Add("预约检查日期需大于等于今天");
            }
            if (string.IsNullOrEmpty(model.InspectorTel))
            {
                errors.Add("检验联系人电话不能为空");
            }
            if (string.IsNullOrEmpty(model.Inspector))
            {
                errors.Add("检验联系人不能为空");
            }
            if (string.IsNullOrEmpty(model.InspectionAddr))
            {
                errors.Add("检验地点不能为空");
            }
            if (!string.IsNullOrEmpty(model.Voyage) && DateTime.Parse(model.Voyage) < DateTime.Now.Date)
            {
                errors.Add("出发日期应大于等于当前时间");
            }
            if (!string.IsNullOrEmpty(model.ShippingDate) && DateTime.Parse(model.ShippingDate) < DateTime.Now.Date)
            {
                errors.Add("计划发货日期应大于等于当前时间");
            }
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
                GetTotalUnits(ref model);
                if (isNew)
                {
                    appkey = _appRepo.CreateApplication(_auth.GetAuth().MerchantId, model);
                }
                else
                {
                    _appRepo.Update(merchantId, appkey, model);
                }
                errors = null;
            }
            return new KtechJsonResult(HttpStatusCode.OK,
                new { isNew = isNew, appkey = appkey, isLogin = isLogin, errors = errors });
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

        #region 让批次号可编辑

        //todo 暂时的处理方法,以免后面需求又改动回不可编辑时直接删掉这个代码即可
        private void ApplicationItemModifyAble(ref Application app)
        {
            if (app.Status == ApplicationStatus.New)
            {
                foreach (var product in app.ApplicationProducts)
                {
                    var itemList = new List<ProductItem>();
                    var productItemList = product.ProductItemList as IList<ProductItem> ??
                                          product.ProductItemList.ToList();
                    foreach (var item in productItemList)
                    {
                        item.HandlerType = HandlerType.Delete;
                        itemList.Add(new ProductItem
                        {
                            Id = item.Id,
                            ApplicationProductId = item.ApplicationProductId,
                            ExpiryDate = item.ExpiryDate,
                            BatchNo = item.BatchNo,
                            Quantity = item.Quantity,
                            ManufacturerDate = item.ManufacturerDate,
                            HandlerType = HandlerType.Add,
                        });
                    }
                    product.ProductItemList = productItemList.Concat(itemList);
                }
            }
        }

        #endregion
    }
}