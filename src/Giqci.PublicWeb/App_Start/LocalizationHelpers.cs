using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Giqci.ApiProxy.Logging;
using Giqci.ApiProxy.Services;
using Giqci.Chapi.Enums.Dict;
using Giqci.Chapi.Models.Logging;
using Giqci.Interfaces;
using Giqci.PublicWeb.Models;

namespace Giqci.PublicWeb
{
    public static class LocalizationHelpers
    {
        private static readonly IDictService _dict;

        static LocalizationHelpers()
        {
            var httpclient = RestClientFactory.Create(Config.ApiUrl.DictApiUrl, false);
            var log = new LoggingApiProxy(new LogConfig()
            {
                Log = Config.LogSwitch.DictLogSwitch,
                LogUrl = Config.ApiUrl.DictApiUrl
            });
            _dict = new CachedDictService(httpclient, log);
        }

        public static string Language(this HtmlHelper helper, string key)
        {
            LanguageType language = Config.Common.Language;
            return _dict.GetLanguage(language, key);
        }
    }
}