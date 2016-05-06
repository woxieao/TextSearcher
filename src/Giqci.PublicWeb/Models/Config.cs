using System.Web.Configuration;

namespace Giqci.PublicWeb.Models
{
    public class Config
    {
        private Config()
        {
        }

        static Config()
        {
            Current = new Config
            {
                AdminEmail = WebConfigurationManager.AppSettings["AdminEmail"],
                Host = WebConfigurationManager.AppSettings["Host"],
                NoReplyEmail = WebConfigurationManager.AppSettings["NoReplyEmail"],
                FeedbackEmailSubject = WebConfigurationManager.AppSettings["FeedbackEmailSubject"],
                RegMerchantEmailSubject = WebConfigurationManager.AppSettings["RegMerchantEmailSubject"],
                ForgotPasswordEmailSubject = WebConfigurationManager.AppSettings["ForgotPasswordEmailSubject"],
                UserExampleFilePath = WebConfigurationManager.AppSettings["UserExampleFilePath"],
                CertFileSavePath = WebConfigurationManager.AppSettings["CertFileSavePath"],
                AdminWebUrl = WebConfigurationManager.AppSettings["AdminWebUrl"],
            };
        }

        public static Config Current { get; private set; }
        public string AdminEmail { get; set; }
        public string Host { get; set; }
        public string NoReplyEmail { get; set; }
        public string FeedbackEmailSubject { get; set; }
        public string RegMerchantEmailSubject { get; set; }
        public string ForgotPasswordEmailSubject { get; set; }
        public string UserExampleFilePath { get; private set; }
        public string CertFileSavePath { get; private set; }
        public string AdminWebUrl { get; private set; }
    }
}