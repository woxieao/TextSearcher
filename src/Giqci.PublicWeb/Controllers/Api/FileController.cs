using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using Ktech.Mvc.ActionResults;
using System.Web;
using Giqci.Interfaces;
using Giqci.PublicWeb.Models;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("file")]
    public class FileController : Controller
    {
        private readonly IFileApiProxy _fileApiProxy;

        private string GetFileType(string fileFullPath)
        {
            return fileFullPath.Substring(fileFullPath.LastIndexOf(".", StringComparison.Ordinal) + 1);
        }


        private string FileTypeToContentType(string type)
        {
            var typeList = new Dictionary<string, string>
            {
                {"bmp", "image/bmp"},
                {"gif", "image/gif"},
                {"jpeg", "image/jpeg"},
                {"jpg", "image/jpeg"},
                {"png", "image/png"},
                {"tif", "image/tiff"},
                {"tiff", "image/tiff"},
                {"doc", "application/msword"},
                {"docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                {"pdf", "application/pdf"},
                {"ppt", "application/vnd.ms-powerpoint"},
                {"pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
                {"xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {"xls", "application/vnd.ms-excel"},
                {"csv", "text/csv"},
                {"xml", "text/xml"},
                {"txt", "text/plain"},
                {"zip", "application/zip"},
                {"ogg", "application/ogg"},
                {"mp3", "audio/mpeg"},
                {"wma", "audio/x-ms-wma"},
                {"wav", "audio/x-wav"},
                {"wmv", "audio/x-ms-wmv"},
                {"swf", "application/x-shockwave-flash"},
                {"avi", "video/avi"},
                {"mp4", "video/mp4"},
                {"mpeg", "video/mpeg"},
                {"mpg", "video/mpeg"},
                {"qt", "video/quicktime"}
            };
            return typeList[type];
        }

        private string FilePathToContentType(string fileFullPath)
        {
            return FileTypeToContentType(GetFileType(fileFullPath));
        }

        public FileController(IFileApiProxy fileApiProxy)
        {
            _fileApiProxy = fileApiProxy;
        }

        [Route("viewfile")]
        [HttpGet]
        public ActionResult ViewFile(string fileFullPath)
        {
            Response.ClearContent();
            Response.ContentType = FilePathToContentType(fileFullPath);
            Response.BinaryWrite(_fileApiProxy.Get(fileFullPath));
            return View();
        }


        [Route("UploadExampleFile")]
        [HttpPost]
        public ActionResult UploadExampleFile(HttpPostedFileBase file)
        {
            var filePath = string.Format("/{0}/{1}", Config.Common.UserExampleFilePath, Guid.NewGuid().ToString("N"));
            var fileInfo = _fileApiProxy.UploadFile(file, filePath);
            return new KtechJsonResult(HttpStatusCode.OK, fileInfo);
        }
    }
}