using System.Web.Mvc;
using Giqci.PublicWeb.Models;
using Giqci.Tools;

namespace Giqci.PublicWeb.Controllers
{
    [RoutePrefix("{languageType}/file")]
    public class FileController : Controller
    {
        [Route("viewfile")]
        [HttpGet]
        public ActionResult ViewFile(string fileFullPath)
        {
            return Redirect(string.Format("{0}/{1}", Config.Common.ViewFileUrl, Filer.EncryptFilePath(fileFullPath)));
        }
    }
}