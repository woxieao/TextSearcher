using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Web;
using Giqci.PublicWeb.Models;
using Ktech.Mvc.ActionResults;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("api")]
    public class UpLoadController : Controller
    {
        public UpLoadController()
        {
        }

        private static readonly string[] AllowFileType = {"pdf", "jpg"};
        private const string TempFilePath = "TempFiles";

        [Route("Upload")]
        [HttpPost]
        public string Upload(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var type = file.FileName.Substring(file.FileName.LastIndexOf(".", StringComparison.Ordinal) + 1);
                var fileName = string.Format("{0}.{1}", Guid.NewGuid().ToString("N"), type);

                var filePath = string.Format("/{0}/{1}", TempFilePath, fileName);
                var fileRealPath = Server.MapPath(string.Format("~{0}", filePath));
                var fileInfo = new FileInfo(fileRealPath);
                if (fileInfo.Directory != null && !fileInfo.Directory.Exists)
                {
                    fileInfo.Directory.Create();
                }
                file.SaveAs(fileRealPath);
                return filePath;
            }
            else
            {
                return string.Empty;
            }
        }

        private static string GetMd5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                var sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail, error:" + ex.Message);
            }
        }
    }
}