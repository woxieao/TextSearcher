using System.Net;
using System.Web.Mvc;
using Ktech.Mvc.ActionResults;
using System.Web;
using Giqci.Interfaces;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("file")]
    public class FileController : Controller
    {
        private readonly IFileApiProxy _fileApiProxy;

        public FileController(IFileApiProxy fileApiProxy)
        {
            _fileApiProxy = fileApiProxy;
        }

        public ActionResult ViewFile(string filePath)
        {
            Response.ClearContent();
            //todo
            Response.ContentType = "image/Gif";
            Response.BinaryWrite(_fileApiProxy.Get(filePath));
            return View();
        }


        [Route("UploadFile")]
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            var fileInfo = _fileApiProxy.UploadFile(file);
            return new KtechJsonResult(HttpStatusCode.OK, new {fileInfo = fileInfo});
        }
    }
}