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
            //todo 统一前后台
            var errors = HasError(model);
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
            return new KtechJsonResult(HttpStatusCode.OK,
                new {appNo = appNo, id = id, isLogin = isLogin, errors = errors});
        }


        [Route("forms/list")]
        [HttpGet]
        public ActionResult UserFormsList()
        {
            return View();
        }

        protected List<string> HasError(Application item)
        {
            var errors = new List<string>();
            try
            {
                #region Application Check

                if (!item.C101 && !item.C102 && !item.C103)
                {
                    errors.Add("请选择证书类型");
                }
                if (!Enum.IsDefined(typeof (TradeType), item.TradeType))
                {
                    errors.Add("请选择商业目的");
                }
                if (string.IsNullOrEmpty(item.DestPort))
                {
                    errors.Add("请填写目标港口");
                }
                if (string.IsNullOrEmpty(item.LoadingPort))
                {
                    errors.Add("请填写发货港口");
                }
                if (item.DestPort == item.LoadingPort)
                {
                    errors.Add("目标港口不能与发货港口相同");
                }
                if (string.IsNullOrEmpty(item.InspectionAddr))
                {
                    errors.Add("请填写检验地点");
                }
                if (item.ShippingMethod == ShippingMethod.A)
                {
                    if (item.TotalUnits <= 0)
                        errors.Add("运输总数量必须大于0");
                }
                if (item.TotalWeight <= 0)
                    errors.Add("运输总重量必须大于0");
                if (string.IsNullOrEmpty(item.Importer))
                {
                    errors.Add("请填写进口商全称");
                }
                if (string.IsNullOrEmpty(item.ImporterAddr))
                {
                    errors.Add("请填写进口商地址");
                }
                if (string.IsNullOrEmpty(item.Exporter))
                {
                    errors.Add("请填写出口商全称");
                }
                if (string.IsNullOrEmpty(item.ExporterAddr))
                {
                    errors.Add("请填写出口商地址");
                }

                #endregion

                #region Product Check

                var applicationGoodsList = item.ApplicationProducts;
                if (applicationGoodsList == null || !applicationGoodsList.Any())
                {
                    errors.Add("商品列表不能为空");
                    return errors;
                }

                if (applicationGoodsList.Count(i => string.IsNullOrWhiteSpace(i.CiqCode)) > 0 &&
                    applicationGoodsList.Count(i => !string.IsNullOrWhiteSpace(i.CiqCode)) > 0)
                {
                    errors.Add("在同一个申请中,多个商品的备案号只能全部为空,或者全不为空");
                    return errors;
                }
                var batchNoList = new List<string>();
                var zcodeStartList = new List<string>();
                var zcodeEndList = new List<string>();
                foreach (var applicationGoods in applicationGoodsList)
                {
                    var index1 = applicationGoodsList.IndexOf(applicationGoods) + 1;

                    if (!string.IsNullOrEmpty(applicationGoods.CiqCode) &&
                        applicationGoodsList.Count(i => i.CiqCode == applicationGoods.CiqCode) > 1)
                    {
                        errors.Add(string.Format("商品{0}备案号[{1}]不可重复添加", index1, applicationGoods.CiqCode));
                    }
                    var productItemList = applicationGoods.ProductItemList == null
                        ? null
                        : applicationGoods.ProductItemList.ToList();
                    var applicationZCodeList = applicationGoods.ApplicationZCodes == null
                        ? null
                        : applicationGoods.ApplicationZCodes.ToList();
                    if (string.IsNullOrEmpty(applicationGoods.DescriptionEn)
                        || string.IsNullOrEmpty(applicationGoods.HSCode)
                        || string.IsNullOrEmpty(applicationGoods.Spec)
                        || string.IsNullOrEmpty(applicationGoods.ManufacturerCountry)
                        || string.IsNullOrEmpty(applicationGoods.Brand)
                        || string.IsNullOrEmpty(applicationGoods.Package))
                    {
                        errors.Add(string.Format("商品{0}资料不完整", index1));
                    }
                    if (productItemList == null || !productItemList.Any())
                    {
                        errors.Add(string.Format("商品{0}批次资料不能为空", index1));
                        return errors;
                    }
                    foreach (var productItem in productItemList)
                    {
                        batchNoList.Add(productItem.BatchNo);
                        if (batchNoList.Count(i => i == productItem.BatchNo) > 1)
                        {
                            errors.Add("商品批次号不可重复");
                            return errors;
                        }
                        var index2 = productItemList.IndexOf(productItem) + 1;
                        if (!productItem.ExpiryDate.HasValue)
                        {
                            errors.Add(string.Format("请填写商品{0}[{1}]的保质期", index1, index2));
                        }
                        if (!string.IsNullOrEmpty(item.ShippingDate) &&
                            productItem.ExpiryDate < DateTime.Parse(item.ShippingDate))
                        {
                            errors.Add(string.Format("商品{0}[{1}]的保质期不能小于发货日期", index1, index2));
                        }
                        if (productItem.ExpiryDate < item.InspectionDate)
                        {
                            errors.Add(string.Format("商品{0}[{1}]的保质期不能小于预约检查日期", index1, index2));
                        }
                        if (!productItem.ManufacturerDate.HasValue && string.IsNullOrEmpty(productItem.BatchNo))
                        {
                            errors.Add(string.Format("请正确填写商品{0}[{1}]的生产日期或批次号", index1, index2));
                        }
                        if (productItem.ManufacturerDate.HasValue && productItem.ManufacturerDate > DateTime.Now)
                        {
                            errors.Add(string.Format("商品{0}[{1}]的生产日期不能大于当前时间", index1, index2));
                        }
                        if (productItem.ManufacturerDate.HasValue &&
                            productItem.ManufacturerDate >= productItem.ExpiryDate)
                        {
                            errors.Add(string.Format("商品{0}[{1}]的生产日期不能大于等于保质期", index1, index2));
                        }
                        if (productItem.Quantity <= 0)
                        {
                            errors.Add(string.Format("商品{0}[{1}]的数量必须大于0", index1, index2));
                        }
                    }
                    if (applicationZCodeList != null)
                    {
                        foreach (var applicationZCode in applicationZCodeList)
                        {
                            var index3 = applicationZCodeList.IndexOf(applicationZCode);
                            zcodeStartList.Add(applicationZCode.ZCodeStart);
                            if (zcodeStartList.Count(i => i == applicationZCode.ZCodeStart) > 1)
                            {
                                errors.Add(string.Format("商品{0}[{1}]真知码首码不可重复", index1, index3));
                                break;
                            }
                            zcodeEndList.Add(applicationZCode.ZCodeEnd);
                            if (zcodeEndList.Count(i => i == applicationZCode.ZCodeEnd) > 1)
                            {
                                errors.Add(string.Format("商品{0}[{1}]真知码尾码不可重复", index1, index3));
                                break;
                            }
                            if (string.IsNullOrEmpty(applicationZCode.ZCodeStart) ||
                                string.IsNullOrEmpty(applicationZCode.ZCodeEnd) ||
                                applicationZCode.ZCodeStart.Length != 16 ||
                                applicationZCode.ZCodeEnd.Length != 16)
                            {
                                errors.Add(string.Format("商品{0}[{1}]的真知码首码和尾码长度只能为16位", index1, index3));
                            }
                        }
                    }
                }

                #endregion

                #region ContainerInfo Check

                if (item.ShippingMethod == ShippingMethod.O)
                {
                    var containerInfoList = item.ContainerInfos;
                    if ((containerInfoList == null || !containerInfoList.Any()))
                    {
                        errors.Add("当运输方式为海运时,装箱信息不能为空");
                        return errors;
                    }

                    foreach (var containerInfo in containerInfoList)
                    {
                        if (containerInfoList.Count(i => i.ContainerNumber == containerInfo.ContainerNumber) > 1)
                        {
                            errors.Add("装箱号不能重复");
                            return errors;
                        }
                        if (containerInfoList.Count(i => i.SealNumber == containerInfo.SealNumber) > 1)
                        {
                            errors.Add("铅封号不能重复");
                            return errors;
                        }
                        if (string.IsNullOrWhiteSpace(containerInfo.ContainerNumber) ||
                            string.IsNullOrWhiteSpace(containerInfo.SealNumber))
                        {
                            errors.Add(string.Format("装箱信息{0} [装箱号/铅封号]不能为空",
                                containerInfoList.IndexOf(containerInfo) + 1));
                        }
                    }
                }
            }
            catch
            {
                errors.Add("检测提交信息时异常,请检测后再提交");
            }

            #endregion

            return errors;
        }

        protected static void GetTotalUnits(ref Application input)
        {
            input.TotalUnits = input.ShippingMethod == ShippingMethod.O
                ? (input.ContainerInfos == null ? 0 : input.ContainerInfos.Count())
                : input.TotalUnits;
        }
    }
}