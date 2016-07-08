using System;
using System.Web;
using System.Web.Configuration;
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

        public static string KeyToWord(this string wordKeyName, LanguageType languageType)
        {
            var result = Dict.GetLanguage(languageType, wordKeyName);
            //if can not find the result return wordKeyName
            return string.IsNullOrEmpty(result) ? wordKeyName : result;
        }



        public static string KeyToWord(this string wordKeyName)
        {
            LanguageType languageType;
            try
            {
                var lanTypeStr = LanCore.GetLanType();
                Enum.TryParse(lanTypeStr, true, out languageType);
            }
            catch
            {
                languageType = DefaultLanguage;
            }
            return wordKeyName.KeyToWord(languageType);
        }
    }

    public class LanCore
    {
        public static string GetLanType()
        {
            var lanType = HttpContext.Current.Request.RawUrl.Split('/')[1];
            return lanType;
        }
        public static string GetLanTypeUrl()
        {
            return string.Format("/{0}/", GetLanType());
        }
    }
}