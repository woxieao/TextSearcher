using System;
using System.Web.Configuration;

namespace Giqci.PublicWeb.Models
{
    public class Config
    {
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
        }

        public struct ApiUrl
        {
            public static readonly string LogApiUrl = WebConfigurationManager.AppSettings[nameof(LogApiUrl)];
            public static readonly string FileApiUrl = WebConfigurationManager.AppSettings[nameof(FileApiUrl)];
            public static readonly string DictApiUrl = WebConfigurationManager.AppSettings[nameof(DictApiUrl)];
            public static readonly string ProdApiUrl = WebConfigurationManager.AppSettings[nameof(ProdApiUrl)];
            public static readonly string CustApiUrl = WebConfigurationManager.AppSettings[nameof(CustApiUrl)];
            public static readonly string AppApiUrl = WebConfigurationManager.AppSettings[nameof(AppApiUrl)];
        }

        public struct LogSwitch
        {
            private static bool GetLogSwitch(string logSwitchName)
            {
                var logCinfig = WebConfigurationManager.AppSettings[logSwitchName] ??
                                string.Empty;
                return logCinfig == "1" || logCinfig.ToLower() == "true";
            }

            public static readonly bool DictLogSwitch = GetLogSwitch(nameof(DictLogSwitch));
            public static readonly bool ProductsLogSwitch = GetLogSwitch(nameof(ProductsLogSwitch));
            public static readonly bool CustomersLogSwitch = GetLogSwitch(nameof(CustomersLogSwitch));
            public static readonly bool AppLogSwitch = GetLogSwitch(nameof(AppLogSwitch));
            public static readonly bool FileLogSwitch = GetLogSwitch(nameof(FileLogSwitch));
            public static readonly bool AppCacheLogSwitch = GetLogSwitch(nameof(AppCacheLogSwitch));
        }

        public struct Filer
        {
            public static readonly string[] AllowTypeList =
                (WebConfigurationManager.AppSettings[nameof(AllowTypeList)] ?? "").Split('|');

            public static readonly long FileMaxLength =
                Convert.ToInt64(WebConfigurationManager.AppSettings[nameof(FileMaxLength)]);
        }
    }
}