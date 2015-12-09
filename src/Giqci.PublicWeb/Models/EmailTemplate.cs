using Ktech.Interfaces;

namespace Giqci.PublicWeb.Models
{
    public class SendEmailTemplate : IEmailTemplate
    {
        public string FromEmail { get; set; }

        public string FromEmailDisplay { get; set; }

        public string HtmlTemplate { get; set; }

        public string Subject { get; set; }

        public string TextTemplate { get; set; }
    }
}