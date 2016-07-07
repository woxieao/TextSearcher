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
            var str = string.Empty;
            foreach (var mailData in mailInfo.data)
            {
                str += mailData.Value.val;
            }
            var log = new LoggingApiProxy(new LogConfig
            {
                LogUrl = Config.ApiUrl.LogApiUrl,
                Log = true
            });
            log.LogApiRequest("", "---------EmailTest-------------", str);
            log.LogApiRequest("", "---------EmailTest___All-------", JsonConvert.SerializeObject(Request.Form));
            return new KtechJsonResult(HttpStatusCode.Accepted, str);
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
            public Dictionary<int, MailData> data { get; set; }
        }
        public class MailData
        {
            public string val { get; set; }
            public string label { get; set; }
        }
    }
}