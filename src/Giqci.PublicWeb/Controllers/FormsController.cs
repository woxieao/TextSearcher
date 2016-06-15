using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Giqci.Chapi.Enums.App;
using Giqci.Chapi.Models.App;
using Giqci.Interfaces;
using Giqci.PublicWeb.Services;
using Ktech.Mvc.ActionResults;
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
        [Route("forms/app/print/{appkey}")]
        [HttpGet]
        public ActionResult PrintApplication(string appkey)
        {
            var merchant = _merchantRepo.GetMerchant(User.Identity.Name);
            var application = _appRepo.Get(merchant.Id, appkey);
            ApplicationItemModifyAble(ref application);
            //非该登录人的申请||不是新申请
            if (application == null)
            {
                throw new ApplicationException(":(   You Can Not View This Application");
            }
            var certs = _certRepo.Select(appkey);
            var validCerts = certs.GroupBy(i => i.CertType);
            ViewBag.ValidCerts =
                validCerts.Select(certTypeList => certTypeList.OrderByDescending(i => i.SignedDate).First())
                    .ToList();
            ViewBag.IsPrint = true;
            return View("PrintApp", application);
        }


        [Route("forms/list")]
        [HttpGet]
        public ActionResult UserFormsList()
        {
            return View();
        }



        #region 让批次号可编辑

        //todo 暂时的处理方法,以免后面需求又改动回不可编辑时直接删掉这个代码即可
        private void ApplicationItemModifyAble(ref Application app)
        {
            //todo  see...?暂时的处理方法,以免后面需求又改动回可编辑时直接解除注释这个代码即可
            //if (app.Status == ApplicationStatus.New)
            //{
            //    foreach (var product in app.ApplicationProducts)
            //    {
            //        var itemList = new List<ProductItem>();
            //        var productItemList = product.ProductItemList as IList<ProductItem> ??
            //                              product.ProductItemList.ToList();
            //        foreach (var item in productItemList)
            //        {
            //            item.HandlerType = HandlerType.Delete;
            //            itemList.Add(new ProductItem
            //            {
            //                Id = item.Id,
            //                ApplicationProductId = item.ApplicationProductId,
            //                ExpiryDate = item.ExpiryDate,
            //                BatchNo = item.BatchNo,
            //                HandlerType = HandlerType.Add,
            //            });
            //        }
            //        product.ProductItemList = productItemList.Concat(itemList);
            //    }
            //}
        }

        #endregion
    }
}