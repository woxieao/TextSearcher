using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Giqci.Entities.Core;
using Giqci.Enums;
using Giqci.Models;
using Giqci.PublicWeb.Helpers;
using Giqci.PublicWeb.Models;
using Giqci.PublicWeb.Models.Application;
using Giqci.Repositories;
using Giqci.Services;
using Ktech.Mvc.ActionResults;
using Application = Giqci.Models.Application;
using GoodsItem = Giqci.Models.GoodsItem;
using ContainerInfo = Giqci.Models.ContainerInfo;
using Newtonsoft.Json;
using Giqci.PublicWeb.Models.Goods;
using Ktech.Extensions;

namespace Giqci.PublicWeb.Controllers
{
    public class FormsController : Ktech.Mvc.ControllerBase
    {
        private readonly IPublicRepository _publicRepo;
        private readonly IMerchantRepository _merchantRepo;
        private readonly ICachedDictionaryService _cache;
        private readonly IApplicationRepository _appRepo;
        private readonly IContainerInfoRepository _conRepo;
        private readonly IExampleCertRepository _exaRepo;
        private readonly IDictionaryRepository _repoDictionary;

        public FormsController(IPublicRepository publicRepo, ICachedDictionaryService cache,
            IMerchantRepository merchantRepo, IApplicationRepository appRepo, IContainerInfoRepository conRepo,
            IExampleCertRepository exaRepo, IDictionaryRepository repoDictionary)
        {
            _publicRepo = publicRepo;
            _merchantRepo = merchantRepo;
            _cache = cache;
            _appRepo = appRepo;
            _conRepo = conRepo;
            _exaRepo = exaRepo;
            _repoDictionary = repoDictionary;
        }

        [Route("forms/app/")]
        [HttpGet]
        [Authorize]
        public ActionResult Application(string applicantCode)
        {
            var merchant = _merchantRepo.GetMerchant(User.Identity.Name);
            ViewBag.id = 0;
            //get cookies 
            var cookie = new CookieHelper();
            var appString = cookie.GetApplication("application");
            Models.Application.Application application =
                JsonConvert.DeserializeObject<Models.Application.Application>(appString);
            ApplicationPageModel model;
            if (application == null)
            {
                model = new ApplicationPageModel
                {
                    Application = new Models.Application.Application
                    {
                        ApplicantCode = applicantCode,
                        Applicant = merchant.Name,
                        ApplicantAddr = merchant.Address,
                        ApplicantContact = merchant.Contact,
                        ApplicantPhone = merchant.Phone,
                        ApplicantEmail = merchant.Email,
                        InspectionDate = DateTime.Now.AddDays(-1),
                        //Goods = new List<GoodsItem> {new GoodsItem {ManufacturerCountry = "036"}},
                        ContainerInfoList = new List<ContainerInfo>
                        {
                            new ContainerInfo()
                            {
                                ContainerNumber = String.Empty,
                                SealNumber = String.Empty,
                            }
                        },
                    }
                };
            }
            else
            {
                application.ExampleCertList = cookie.GetExampleList();
                model = new ApplicationPageModel
                {
                    Application = application
                };
            }
            ModelBuilder.SetHelperFields(_cache, model);
            return View(model);
        }


        [Route("forms/app/{id:int}")]
        [HttpGet]
        [Authorize]
        public ActionResult Application(int id)
        {
            var merchant = _merchantRepo.GetMerchant(User.Identity.Name);
            var application = _appRepo.GetApplication(id, merchant.Id);
            //非该登录人的申请就跳转到新申请页面,避免任意用户更改非本人的申请
            if (application == null)
            {
                return Redirect("/forms/app/");
            }
            ViewBag.id = id;
            var goodsList = new List<GoodsItemModel>();
            var goodsItems = _appRepo.SelectGoodsItems(application.Id);
            var containerInfos = _conRepo.GetContainerInfoList(application.Id);
            var exampleCerts = _exaRepo.GetExampleCertList(application.Id);
            var cookieHelper = new CookieHelper();
            var exampleListStr = string.Empty;
            foreach (var exampleCerat in exampleCerts)
            {
                exampleListStr += string.Format("|{0}|", exampleCerat.CertFilePath);
            }
            cookieHelper.OverrideCookies(cookieHelper.ExampleFileListKeyName, exampleListStr);
            foreach (Entities.Core.GoodsItem goodsItem in goodsItems)
            {
                var gi = new GoodsItemModel
                {
                    BatchNo = goodsItem.BatchNo,
                    Brand = goodsItem.Brand,
                    C102 = goodsItem.C102,
                    C102Comment = goodsItem.C102Comment,
                    CiqCode = goodsItem.CiqCode,
                    Code = goodsItem.Code,
                    Description = goodsItem.Description,
                    DescriptionEn = goodsItem.DescriptionEn,
                    ExpiryDate = goodsItem.ExpiryDate,
                    HSCode = goodsItem.HSCode,
                    Manufacturer = goodsItem.Manufacturer,
                    ManufacturerCountry = goodsItem.ManufacturerCountry,
                    ManufacturerDate = goodsItem.ManufacturerDate,
                    OtherHSCode = goodsItem.OtherHSCode,
                    Package = goodsItem.Package,
                    Quantity = goodsItem.Quantity,
                    Spec = goodsItem.Spec,
                    HSCodeDesc = _repoDictionary.GetHSCode(goodsItem.HSCode).Name,
                    ManufacturerCountryDesc =
                        _repoDictionary.GetCountryDictionaryByCode(goodsItem.ManufacturerCountry).CnName
                };
                goodsList.Add(gi);
            }

            var model = new ApplicationPageModel
            {
                Application = new Models.Application.Application
                {
                    ApplicantCode = application.ApplicantCode,
                    Applicant = application.Applicant,
                    ApplicantAddr = application.ApplicantAddr,
                    ApplicantContact = application.ApplicantContact,
                    ApplicantPhone = application.ApplicantPhone,
                    ApplicantEmail = application.ApplicantEmail,
                    Billno = application.BillNo,
                    C101 = application.C101,
                    C102 = application.C102,
                    C103 = application.C103,
                    DestPort = application.DestPort,
                    Exporter = application.Exporter,
                    ExporterAddr = application.ExporterAddr,
                    ExporterContact = application.ExporterContact,
                    ExporterPhone = application.ExporterPhone,
                    ImBroker = application.ImBroker,
                    ImBrokerAddr = application.ImBrokerAddr,
                    ImBrokerContact = application.ImBrokerContact,
                    ImBrokerPhone = application.ImBrokerPhone,
                    Importer = application.Importer,
                    ImporterAddr = application.ImporterAddr,
                    ImporterContact = application.ImporterContact,
                    ImporterPhone = application.ImporterPhone,
                    InspectionAddr = application.InspectionAddr,
                    InspectionDate = application.InspectionDate,
                    LoadingPort = application.LoadingPort,
                    OtherBillno = application.OtherBillNo,
                    OtherDestPort = application.OtherDestPort,
                    OtherLoadingPort = application.OtherLoadingPort,
                    PurchaseContract = application.PurchaseContract,
                    ShippingDate = application.ShippingDate,
                    ShippingMethod = application.ShippingMethod,
                    TotalUnits = application.TotalUnits,
                    TotalWeight = application.TotalWeight,
                    TradeType = application.TradeType,
                    Vesselcn = application.Vesselcn,
                    Voyage = application.Voyage,
                    Goods = goodsList,
                    ContainerInfoList = containerInfos,
                    ExampleCertList = exampleCerts,
                }
            };

            ModelBuilder.SetHelperFields(_cache, model);
            return View(model);
        }


        [Route("forms/app")]
        [HttpPost]
        public ActionResult SubmitApplication(Application model, int id = 0)
        {
            // trade type must be 电商贸易
            //model.TradeType = TradeType.C;
            var errors = validateApplication(model);
            string appNo = null;
            //set cookies 
            var cookieHelper = new CookieHelper();
            cookieHelper.SetApplication("application", model);
            // check user status
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
            {
                appNo = null;
                errors = new List<string>() {"登录状态已失效，请您重新登录系统"};
            }

            var merchantId = _merchantRepo.GetMerchant(User.Identity.Name).Id;
            if (!errors.Any())
            {
                var exampleCertList = cookieHelper.GetExampleList(true);
                // submit
                if (id > 0)
                {
                    Giqci.Entities.Core.Application application = new Entities.Core.Application()
                    {
                        ApplicantCode = model.ApplicantCode,
                        Applicant = model.Applicant,
                        ApplicantAddr = model.ApplicantAddr,
                        ApplicantContact = model.ApplicantContact,
                        ApplicantPhone = model.ApplicantPhone,
                        ApplicantEmail = model.ApplicantEmail,
                        BillNo = model.Billno,
                        C101 = model.C101,
                        C102 = model.C102,
                        C103 = model.C103,
                        DestPort = model.DestPort,
                        Exporter = model.Exporter,
                        ExporterAddr = model.ExporterAddr,
                        ExporterContact = model.ExporterContact,
                        ExporterPhone = model.ExporterPhone,
                        ImBroker = model.ImBroker,
                        ImBrokerAddr = model.ImBrokerAddr,
                        ImBrokerContact = model.ImBrokerContact,
                        ImBrokerPhone = model.ImBrokerPhone,
                        Importer = model.Importer,
                        ImporterAddr = model.ImporterAddr,
                        ImporterContact = model.ImporterContact,
                        ImporterPhone = model.ImporterPhone,
                        InspectionAddr = model.InspectionAddr,
                        InspectionDate = model.InspectionDate,
                        LoadingPort = model.LoadingPort,
                        OtherBillNo = model.OtherBillno,
                        OtherDestPort = model.OtherDestPort,
                        OtherLoadingPort = model.OtherLoadingPort,
                        PurchaseContract = model.PurchaseContract,
                        ShippingDate = model.ShippingDate,
                        ShippingMethod = model.ShippingMethod,
                        TotalUnits = model.TotalUnits,
                        TotalWeight = model.TotalWeight,
                        TradeType = model.TradeType,
                        Vesselcn = model.Vesselcn,
                        Voyage = model.Voyage,
                    };
                    List<Giqci.Entities.Core.GoodsItem> goodsItems = new List<Entities.Core.GoodsItem>();
                    foreach (GoodsItem goodsItem in model.Goods)
                    {
                        Giqci.Entities.Core.GoodsItem gi = new Entities.Core.GoodsItem
                        {
                            BatchNo = goodsItem.BatchNo,
                            Brand = goodsItem.Brand,
                            C102 = goodsItem.C102,
                            C102Comment = goodsItem.C102Comment,
                            CiqCode = goodsItem.CiqCode,
                            Code = goodsItem.Code,
                            Description = goodsItem.Description,
                            DescriptionEn = goodsItem.DescriptionEn,
                            ExpiryDate = goodsItem.ExpiryDate,
                            HSCode = goodsItem.HSCode,
                            Manufacturer = goodsItem.Manufacturer,
                            ManufacturerCountry = goodsItem.ManufacturerCountry,
                            ManufacturerDate = goodsItem.ManufacturerDate,
                            OtherHSCode = goodsItem.OtherHSCode,
                            Package = goodsItem.Package,
                            Quantity = goodsItem.Quantity,
                            Spec = goodsItem.Spec
                        };
                        goodsItems.Add(gi);
                    }
                    var _containerInfo = new List<Giqci.Entities.Core.ContainerInfo>();
                    foreach (var containerInfo in model.ContainerInfoList)
                    {
                        _containerInfo.Add(new Giqci.Entities.Core.ContainerInfo()
                        {
                            SealNumber = containerInfo.SealNumber,
                            ContainerNumber = containerInfo.ContainerNumber
                        });
                    }
                    _appRepo.UpdateApplication(id, merchantId, application, goodsItems, _containerInfo, exampleCertList);
                }
                else
                {
                    appNo = _publicRepo.CreateApplication(User.Identity.Name, model, exampleCertList);
                }
                errors = null;
                cookieHelper.DeleteApplication("application");
            }
            return new KtechJsonResult(HttpStatusCode.OK, new {appNo = appNo, id = id, errors = errors});
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
            return new KtechJsonResult(HttpStatusCode.OK, new {});
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
        [Authorize]
        public ActionResult UserFormsList()
        {
            return View();
        }

        private List<string> validateApplication(Application model)
        {
            var errors = new List<string>();
            if (model.Goods.Count == 0)
            {
                errors.Add("请至少填写一个商品");
            }
            else
            {
                for (var i = 0; i < model.Goods.Count; i++)
                {
                    var item = model.Goods[i];
                    if (string.IsNullOrEmpty(item.DescriptionEn)
                        || (string.IsNullOrEmpty(item.HSCode) && string.IsNullOrEmpty(item.OtherHSCode))
                        || string.IsNullOrEmpty(item.CiqCode)
                        || string.IsNullOrEmpty(item.Spec)
                        || string.IsNullOrEmpty(item.ManufacturerCountry)
                        || string.IsNullOrEmpty(item.Brand)
                        || item.Quantity <= 0
                        || string.IsNullOrEmpty(item.Package))
                    {
                        errors.Add("商品" + (i + 1) + "资料不完整");
                    }
                    if (!item.ExpiryDate.HasValue)
                    {
                        errors.Add("请填写商品" + (i + 1) + "的过期日期");
                    }
                    if (!item.ManufacturerDate.HasValue && string.IsNullOrEmpty(item.BatchNo))
                    {
                        errors.Add("请正确填写商品" + (i + 1) + "的生产日期或批次号");
                    }
                    if (item.Quantity <= 0)
                    {
                        errors.Add("商品" + (i + 1) + "的数量必须大于0");
                    }
                    if (item.ManufacturerDate.HasValue && item.ExpiryDate.HasValue &&
                        item.ManufacturerDate >= item.ExpiryDate)
                    {
                        errors.Add("商品" + (i + 1) + "的生产日期不得大于等于过期日期");
                    }
                }

                if (model.ShippingMethod.ToString().ToUpper() == Giqci.Enums.ShippingMethod.O.ToString())
                {
                    if (!model.ContainerInfoList.Any())
                    {
                        errors.Add("船舶资料不能为空");
                    }
                    for (var i = 0; i < model.ContainerInfoList.Count; i++)
                    {
                        var containerInfo = model.ContainerInfoList[i];
                        if (string.IsNullOrWhiteSpace(containerInfo.ContainerNumber))
                        {
                            errors.Add("船舶资料" + (i + 1) + "的装箱号不能为空");
                        }
                        if (string.IsNullOrWhiteSpace(containerInfo.SealNumber))
                        {
                            errors.Add("船舶资料" + (i + 1) + "的铅封号不能为空");
                        }
                    }
                }
                if (!model.C101 && !model.C102 && !model.C103)
                {
                    errors.Add("请选择证书类型");
                }
                if (!Enum.IsDefined(typeof (TradeType), model.TradeType))
                {
                    errors.Add("请选择商业目的");
                }
                if (string.IsNullOrEmpty(model.DestPort) &&
                    string.IsNullOrEmpty(model.OtherDestPort))
                {
                    errors.Add("请填写目标港口");
                }
                if (string.IsNullOrEmpty(model.LoadingPort) &&
                    string.IsNullOrEmpty(model.OtherLoadingPort))
                {
                    errors.Add("请填写发货港口");
                }
                if (model.InspectionDate < DateTime.Today)
                {
                    errors.Add("请填写正确预约检查日期");
                }
                if (string.IsNullOrEmpty(model.InspectionAddr))
                {
                    errors.Add("请填写检验地点");
                }
                if (model.TotalUnits <= 0)
                    errors.Add("运输总数量必须为正整数");
                if (model.TotalWeight <= 0)
                    errors.Add("运输总重量必须大于0");
            }
            return errors;
        }
    }
}