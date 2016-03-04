using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Giqci.Enums;
using Giqci.PublicWeb.Helpers;
using Giqci.PublicWeb.Models.Application;
using Giqci.Repositories;
using Giqci.Services;
using Ktech.Mvc.ActionResults;
using Application = Giqci.Models.Application;
using GoodsItem = Giqci.Models.GoodsItem;
using ContainerInfo = Giqci.Models.ContainerInfo;
using Newtonsoft.Json;
using Giqci.PublicWeb.Models.Goods;

namespace Giqci.PublicWeb.Controllers
{

    public class FormsController : Ktech.Mvc.ControllerBase
    {
        private readonly IPublicRepository _publicRepo;
        private readonly IMerchantRepository _merchantRepo;
        private readonly ICachedDictionaryService _cache;
        private readonly IApplicationRepository _appRepo;
        private readonly IContainerInfoRepository _conRepo;

        public FormsController(IPublicRepository publicRepo, ICachedDictionaryService cache, IMerchantRepository merchantRepo, IApplicationRepository appRepo, IContainerInfoRepository conRepo)
        {
            _publicRepo = publicRepo;
            _merchantRepo = merchantRepo;
            _cache = cache;
            _appRepo = appRepo;
            _conRepo = conRepo;
        }

        [Route("forms/app/")]
        [HttpGet]
        [Authorize]
        public ActionResult Application(string applicantCode)
        {
            var merchant = _merchantRepo.GetMerchant(User.Identity.Name);
            string _appString = "";

            ViewBag.id = 0;

            //get cookies 
            CookieHelper _cookie = new CookieHelper();
            _appString = _cookie.GetApplication("application");
            Models.Application.Application _application = JsonConvert.DeserializeObject<Models.Application.Application>(_appString);
            var model = new ApplicationPageModel { };
            if (_application == null)
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
                        Goods = new List<GoodsItem> { new GoodsItem { ManufacturerCountry = "036" } },
                        ContainerInfoList = new List<ContainerInfo> {new ContainerInfo()
                        {
                            ContainerNumber= String.Empty,
                            SealNumber = String.Empty,
                        } },
                    }
                };
            }
            else
            {
                model = new ApplicationPageModel
                {
                    Application = _application
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
            Giqci.Entities.Core.Application _application = _appRepo.GetApplication(id);
            var model = new ApplicationPageModel { };

            ViewBag.id = id;

            var goods = new List<GoodsItem>();

            List<Giqci.Entities.Core.GoodsItem> _goods = _appRepo.SelectGoodsItems(_application.Id);
            var _containerInfoList = new List<ContainerInfo>();
            var containerInfoList = _conRepo.GetContainerInfoList(_application.Id);
            foreach (var containerInfo in containerInfoList)
            {
                _containerInfoList.Add(new ContainerInfo
                {
                    SealNumber = containerInfo.SealNumber,
                    ContainerNumber = containerInfo.ContainerNumber,
                });
            }
            for (int i = 0; i < _goods.Count; i++)
            {
                GoodsItem gi = new GoodsItem();
                gi.BatchNo = _goods[i].BatchNo;
                gi.Brand = _goods[i].Brand;
                gi.C102 = _goods[i].C102;
                gi.C102Comment = _goods[i].C102Comment;
                gi.CiqCode = _goods[i].CiqCode;
                gi.Code = _goods[i].Code;
                gi.Description = _goods[i].Description;
                gi.DescriptionEn = _goods[i].DescriptionEn;
                gi.ExpiryDate = _goods[i].ExpiryDate;
                gi.HSCode = _goods[i].HSCode;
                gi.Manufacturer = _goods[i].Manufacturer;
                gi.ManufacturerCountry = _goods[i].ManufacturerCountry;
                //gi.ManufacturerCountryName = _goods[i].ManufacturerCountry;
                gi.ManufacturerDate = _goods[i].ManufacturerDate;
                gi.OtherHSCode = _goods[i].OtherHSCode;
                gi.Package = _goods[i].Package;
                gi.Quantity = _goods[i].Quantity;
                gi.Spec = _goods[i].Spec;
                goods.Add(gi);
            }

            model = new ApplicationPageModel
            {
                Application = new Models.Application.Application
                {
                    ApplicantCode = _application.ApplicantCode,
                    Applicant = merchant.Name,
                    ApplicantAddr = merchant.Address,
                    ApplicantContact = merchant.Contact,
                    ApplicantPhone = merchant.Phone,
                    ApplicantEmail = merchant.Email,
                    Billno = _application.BillNo,
                    C101 = _application.C101,
                    C102 = _application.C102,
                    C103 = _application.C103,
                    DestPort = _application.DestPort,
                    Exporter = _application.Exporter,
                    ExporterAddr = _application.ExporterAddr,
                    ExporterContact = _application.ExporterContact,
                    ExporterPhone = _application.ExporterPhone,
                    ImBroker = _application.ImBroker,
                    ImBrokerAddr = _application.ImBrokerAddr,
                    ImBrokerContact = _application.ImBrokerContact,
                    ImBrokerPhone = _application.ImBrokerPhone,
                    Importer = _application.Importer,
                    ImporterAddr = _application.ImporterAddr,
                    ImporterContact = _application.ImporterContact,
                    ImporterPhone = _application.ImporterPhone,
                    InspectionAddr = _application.InspectionAddr,
                    InspectionDate = _application.InspectionDate,
                    LoadingPort = _application.LoadingPort,
                    OtherBillno = _application.OtherBillNo,
                    OtherDestPort = _application.OtherDestPort,
                    OtherLoadingPort = _application.OtherLoadingPort,
                    PurchaseContract = _application.PurchaseContract,
                    ShippingDate = _application.ShippingDate,
                    ShippingMethod = _application.ShippingMethod,
                    TotalUnits = _application.TotalUnits,
                    TotalWeight = _application.TotalWeight,
                    TradeType = _application.TradeType,
                    Vesselcn = _application.Vesselcn,
                    Voyage = _application.Voyage,
                    Goods = goods,
                    ContainerInfoList = _containerInfoList,
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
                errors = new List<string>() { "登录状态已失效，请您重新登录系统" };
            }

            if (!errors.Any())
            {
                // submit
                if (id > 0)
                {
                    Giqci.Entities.Core.Application _application = new Entities.Core.Application()
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
                    List<Giqci.Entities.Core.GoodsItem> _goods = new List<Entities.Core.GoodsItem>();
                    for (int i = 0; i < model.Goods.Count; i++)
                    {
                        Giqci.Entities.Core.GoodsItem gi = new Entities.Core.GoodsItem();
                        gi.BatchNo = model.Goods[i].BatchNo;
                        gi.Brand = model.Goods[i].Brand;
                        gi.C102 = model.Goods[i].C102;
                        gi.C102Comment = model.Goods[i].C102Comment;
                        gi.CiqCode = model.Goods[i].CiqCode;
                        gi.Code = model.Goods[i].Code;
                        gi.Description = model.Goods[i].Description;
                        gi.DescriptionEn = model.Goods[i].DescriptionEn;
                        gi.ExpiryDate = model.Goods[i].ExpiryDate;
                        gi.HSCode = model.Goods[i].HSCode;
                        gi.Manufacturer = model.Goods[i].Manufacturer;
                        gi.ManufacturerCountry = model.Goods[i].ManufacturerCountry;
                        //gi.ManufacturerCountryName = model.Goods[i].ManufacturerCountry;
                        gi.ManufacturerDate = model.Goods[i].ManufacturerDate;
                        gi.OtherHSCode = model.Goods[i].OtherHSCode;
                        gi.Package = model.Goods[i].Package;
                        gi.Quantity = model.Goods[i].Quantity;
                        gi.Spec = model.Goods[i].Spec;
                        _goods.Add(gi);
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
                    _appRepo.UpdateApplication(id, _application, _goods, _containerInfo);
                }
                else
                {
                    appNo = _publicRepo.CreateApplication(User.Identity.Name, model);
                }
                errors = null;

                CookieHelper _cookie = new CookieHelper();
                _cookie.DeleteApplication("application");
            }
            return new KtechJsonResult(HttpStatusCode.OK, new { appNo = appNo, id = id, errors = errors });
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
                if (!Enum.IsDefined(typeof(TradeType), model.TradeType))
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
                    errors.Add("运输总数量必须大于0");
                if (model.TotalWeight <= 0)
                    errors.Add("运输总重量必须大于0");
            }
            return errors;
        }
    }
}