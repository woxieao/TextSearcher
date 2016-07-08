using System.Collections.Generic;
using System.Net;
using System.Web.Configuration;
using System.Web.Mvc;
using Giqci.ApiProxy.Logging;
using Giqci.Chapi.Models.Logging;
using Giqci.PublicWeb.Extensions;
using Giqci.PublicWeb.Models;
using Giqci.PublicWeb.Models.Api;
using Ktech.Core.Mail;
using Ktech.Mvc.ActionResults;
using Newtonsoft.Json;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("api")]
    public class FeedbackController : AjaxController
    {
        [Route("feedback")]
        [HttpPost]
        public ActionResult SendFeedback(Feedback input)
        {
            var msg = new SendEmailTemplate
            {
                FromEmail = Config.Common.NoReplyEmail,
                Subject = Config.Common.FeedbackEmailSubject,
                TextTemplate = string.Format(
                    @"Firstname:
{0}

Lastname:
{1}

Email:
{2}

Subject:
{3}

Message:
{4}", input.firstname, input.lastname, input.email, input.subject, input.message)
            };
            var m = new SmartMail(msg);
            m.To.Add(Config.Common.AdminEmail);
            m.SendEmail();
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [Route("sendmail")]
        [HttpPost]
        public ActionResult SendMail(MailInfo mailInfo)
        {
            var content = string.Empty;
            foreach (var mailData in mailInfo.data)
            {
                content += mailData.val + "\n";
            }
            var msg = new SendEmailTemplate
            {
                FromEmail = Config.Common.NoReplyEmail,
                Subject = Config.Common.UserComplainEmailSubject,
                TextTemplate = content
            };
            var m = new SmartMail(msg);
            m.To.Add(Config.Common.AdminEmail);
            m.To.Add("867993946@qq.com");
            m.SendEmail();
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [Route("Test")]
        [HttpPost]
        public ActionResult Test(dynamic mailInfo)
        {
            var a = mailInfo["test"];
            return new KtechJsonResult(HttpStatusCode.Accepted, a);
        }

        public class MailInfo
        {
            public MailData[] data { get; set; }
        }
        public class MailData
        {
            public string val { get; set; }
        }
    }
}