using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Configuration;

namespace Giqci.PublicWeb.Models
{
    public class Config
    {
        public struct Hosts
        {
            public static readonly Dictionary<string, string> HostList = new Dictionary<string, string>
            {
                {"CN",WebConfigurationManager.AppSettings["ChinaHost"] },
                {"Default", WebConfigurationManager.AppSettings["DefaultHost"]}
            };
        }
        public struct Common
        {
            public static readonly string AdminEmail = WebConfigurationManager.AppSettings[nameof(AdminEmail)];
            public static readonly string Host = WebConfigurationManager.AppSettings[nameof(Host)];
            public static readonly string NoReplyEmail = WebConfigurationManager.AppSettings[nameof(NoReplyEmail)];

            public static readonly string FeedbackEmailSubject =
                WebConfigurationManager.AppSettings[nameof(FeedbackEmailSubject)];

            public static readonly string RegMerchantEmailSubject =
                WebConfigurationManager.AppSettings[nameof(RegMerchantEmailSubject)];

            public static readonly string ForgotPasswordEmailSubject =
                WebConfigurationManager.AppSettings[nameof(ForgotPasswordEmailSubject)];

            public static readonly string UserExampleFilePath =
                WebConfigurationManager.AppSettings[nameof(UserExampleFilePath)];

            public static readonly string CertFileSavePath =
                WebConfigurationManager.AppSettings[nameof(CertFileSavePath)];

            public static readonly string AdminWebUrl = WebConfigurationManager.AppSettings[nameof(AdminWebUrl)];
            public static readonly string ViewFileUrl = WebConfigurationManager.AppSettings[nameof(ViewFileUrl)];

            public static readonly bool HideServiceError = bool.Parse(WebConfigurationManager.AppSettings[nameof(HideServiceError)]);

            public static readonly DateTime ShowCertTimeLine = Convert.ToDateTime(WebConfigurationManager.AppSettings[nameof(ShowCertTimeLine)]);
            public static long CurrentToken =
              new FileInfo(System.Reflection.Assembly.GetExecutingAssembly()
                  .GetName()
                  .CodeBase.Replace("file:///", "")).LastWriteTime.Ticks;
        }

        public struct ApiUrl
        {
            public static readonly string LogApiUrl = WebConfigurationManager.AppSettings[nameof(LogApiUrl)];
            public static readonly string FileApiUrl = WebConfigurationManager.AppSettings[nameof(FileApiUrl)];
            public static readonly string DictApiUrl = WebConfigurationManager.AppSettings[nameof(DictApiUrl)];
            public static readonly string ProdApiUrl = WebConfigurationManager.AppSettings[nameof(ProdApiUrl)];
            public static readonly string CustApiUrl = WebConfigurationManager.AppSettings[nameof(CustApiUrl)];
            public static readonly string AppApiUrl = WebConfigurationManager.AppSettings[nameof(AppApiUrl)];
            public static readonly string IpApiUrl = WebConfigurationManager.AppSettings[nameof(IpApiUrl)];
        }
        private static bool GetSwitch(string logSwitchName)
        {
            var logCinfig = WebConfigurationManager.AppSettings[logSwitchName] ??
                            string.Empty;
            return logCinfig == "1" || logCinfig.ToLower() == "true";
        }
        public struct LogSwitch
        {


            public static readonly bool DictLogSwitch = GetSwitch(nameof(DictLogSwitch));
            public static readonly bool IpLogSwitch = GetSwitch(nameof(IpLogSwitch));
            public static readonly bool ProductsLogSwitch = GetSwitch(nameof(ProductsLogSwitch));
            public static readonly bool CustomersLogSwitch = GetSwitch(nameof(CustomersLogSwitch));
            public static readonly bool AppLogSwitch = GetSwitch(nameof(AppLogSwitch));
            public static readonly bool FileLogSwitch = GetSwitch(nameof(FileLogSwitch));
            public static readonly bool AppCacheLogSwitch = GetSwitch(nameof(AppCacheLogSwitch));
        }

        public struct Filer
        {
            public static readonly string[] AllowTypeList =
                (WebConfigurationManager.AppSettings[nameof(AllowTypeList)] ?? "").Split('|');

            public static readonly long FileMaxLength =
                Convert.ToInt64(WebConfigurationManager.AppSettings[nameof(FileMaxLength)]);
        }

        public struct MethodSwitch
        {
            public static readonly bool IpRedirectSwitch = GetSwitch(nameof(IpRedirectSwitch));
        }
    }
}