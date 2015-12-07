using Ktech.Interfaces;

namespace Giqci.PublicWeb.Models
{
    public class FeedbackEmailTemplate : IEmailTemplate
    {
        public string FromEmail { get; set; }

        public string FromEmailDisplay { get; set; }

        public string HtmlTemplate { get { return null; } }

        public string Subject { get; set; }

        public string TextTemplate { get; set; }
    }

    public class SendEmailTemplate : IEmailTemplate
    {
        public string FromEmail { get; set; }

        public string FromEmailDisplay { get; set; }

        public string HtmlTemplate { get { return null; } }

        public string Subject { get; set; }

        public string TextTemplate { get; set; }
    }
}