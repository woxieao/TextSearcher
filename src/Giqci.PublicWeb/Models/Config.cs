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
                RegMerchantEmailSubject = WebConfigurationManager.AppSettings["RegMerchantEmailSubject"]
            };
        }

        public static Config Current { get; private set; }

        public string AdminEmail { get; set; }

        public string Host { get; set; }

        public string NoReplyEmail { get; set; }

        public string FeedbackEmailSubject { get; set; }

        public string RegMerchantEmailSubject { get; set; }
    }
}