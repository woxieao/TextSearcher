using System.Web.Mvc;
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
            var hostUrl = string.Empty;
            //< !--功能正常后请删掉,以及Config中的IpRedirectSwitch-- >
            if (Config.MethodSwitch.IpRedirectSwitch)
            {
                var ipInfo = _ipDictionaryService.GetCountry();
                if (ipInfo != null)
                {
                    hostUrl = Config.Hosts.HostList.ContainsKey(ipInfo.CountryCode) ? Config.Hosts.HostList[ipInfo.CountryCode] : Config.Hosts.HostList["Default"];
                }
            }
            return Redirect(string.Format("{0}/{1}", hostUrl, defaultPage));
        }

        [Route("terms")]
        public ActionResult Terms()
        {
            return View();
        }
    }
}