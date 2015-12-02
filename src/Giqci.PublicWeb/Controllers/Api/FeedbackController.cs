using System.Net;
using System.Web.Configuration;
using System.Web.Mvc;
using Giqci.PublicWeb.Models;
using Giqci.PublicWeb.Models.Api;
using Ktech.Core.Mail;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("api")]
    public class FeedbackController : Controller
    {
        [Route("feedback")]
        [HttpPost]
        public ActionResult SendFeedback(Feedback input)
        {
            var msg = new FeedbackEmailTemplate
            {
                FromEmail = WebConfigurationManager.AppSettings["FeedbackEmailFrom"],
                Subject = WebConfigurationManager.AppSettings["FeedbackEmailFromDisplay"],
                TextTemplate = string.Format(
                    @"Firstname: {0}
Lastname: {1}
Email: {2}
Subject: {3}
Message: {4}", input.firstname, input.lastname, input.email, input.subject, input.message)
            };
            var m = new SmartMail(msg);
            m.SendEmail();
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}