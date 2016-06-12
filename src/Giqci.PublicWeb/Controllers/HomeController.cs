﻿using System.Web.Mvc;
using Giqci.ApiProxy.Services;
using Giqci.Chapi.Models.Dict;
using Giqci.PublicWeb.Models;

namespace Giqci.PublicWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IIpDictionaryService _ipDictionaryService;

        public HomeController(IIpDictionaryService ipDictionaryService)
        {
            _ipDictionaryService = ipDictionaryService;
        }

        [Route("")]
        public ActionResult Index()
        {
            const string defaultPage = "forms/app";
            var ipInfo = _ipDictionaryService.GetCountry();
            var hostUrl = string.Empty;
            if (ipInfo != null)
            {
                hostUrl = Config.Hosts.HostList.ContainsKey(ipInfo.CountryCode) ? Config.Hosts.HostList[ipInfo.CountryCode] : Config.Hosts.HostList["Default"];
            }
            return Redirect(string.Format("{0}/{1}", hostUrl, defaultPage));
            // return Redirect("/forms/app");
        }

        [Route("terms")]
        public ActionResult Terms()
        {
            return View();
        }
    }
}