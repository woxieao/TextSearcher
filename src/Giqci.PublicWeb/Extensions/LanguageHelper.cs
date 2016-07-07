using System;
using System.Web;
using Giqci.ApiProxy.Logging;
using Giqci.ApiProxy.Services;
using Giqci.Chapi.Enums.Dict;
using Giqci.Chapi.Models.Logging;
using Giqci.Interfaces;
using Giqci.PublicWeb.Models;

namespace Giqci.PublicWeb.Extensions
{
    public static class LanguageHelper
    {
        private const LanguageType DefaultLanguage = LanguageType.Cn;
        private static readonly IDictService Dict;

        static LanguageHelper()
        {
            var httpclient = RestClientFactory.Create(Config.ApiUrl.DictApiUrl, false);
            var log = new LoggingApiProxy(new LogConfig()
            {
                Log = Config.LogSwitch.DictLogSwitch,
                LogUrl = Config.ApiUrl.DictApiUrl
            });
            Dict = new CachedDictService(httpclient, log);
        }

        public static string KeyToWord(this string wordKeyName)
        {
            LanguageType languageType;
            try
            {
                languageType = (LanguageType)Convert.ToInt32(HttpContext.Current.Request.Cookies["languageType"]);
            }
            catch
            {
                languageType = DefaultLanguage;
            }
            return Dict.GetLanguage(languageType, wordKeyName);
        }
    }
}