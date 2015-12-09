using System.IO;
using System.Web;

namespace Giqci.PublicWeb.Helpers
{
    public static class EmailTemplateHelper
    {
        public static string GetEmailTemplate(string filename)
        {
            var path = HttpContext.Current.Server.MapPath("~/App_Data/EmailTemlate/" + filename);
            return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
        }
    }
}