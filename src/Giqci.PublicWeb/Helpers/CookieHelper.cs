using Giqci.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Giqci.PublicWeb.Helpers
{
    public class CookieHelper
    {
        public readonly string ExampleFileListKeyName = "ExampleFileList";

        public bool SetApplication(string key, Application application)
        {
            setCookies(key, JsonConvert.SerializeObject(application), 7);
            return true;
        }

        public string GetApplication(string key)
        {
            return getCookies(key);
        }

        public bool DeleteApplication(string key)
        {
            deleteCookies(key);
            return true;
        }

        #region 上传的样张文件路径记录在cookies里面

        public void SetExampleList(string exampleFilePath)
        {
            setCookies(ExampleFileListKeyName, exampleFilePath);
        }

        public string GetExampleListStr()
        {
            return getCookies(ExampleFileListKeyName);
        }

        public List<ExampleCertView> GetExampleList(bool deleteExampleList = false)
        {
            var exampleFilePathList = getCookies(ExampleFileListKeyName).Split('|');
            var list = new List<ExampleCertView>();
            foreach (var exampleFilePath in exampleFilePathList)
            {
                if (!string.IsNullOrWhiteSpace(exampleFilePath))
                {
                    list.Add(new ExampleCertView {CertFilePath = exampleFilePath});
                }
            }
            if (deleteExampleList)
            {
                DeleteExampleList();
            }
            return list;
        }

        public void DeleteExampleList()
        {
            deleteCookies(ExampleFileListKeyName);
        }

        #endregion

        private void setCookies(string key, string value, int expiryDays = 7)
        {
            var cookie = new HttpCookie(key, encrypt(value));
            cookie.Expires = DateTime.Now.AddDays(expiryDays);

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        private void deleteCookies(string key)
        {
            //HttpContext.Current.Response.Cookies.Remove(key);
            if (HttpContext.Current.Request.Cookies[key] != null)
            {
                HttpContext.Current.Response.Cookies[key].Expires = DateTime.Now.AddDays(-1);
            }
        }

        private string getCookies(string key)
        {
            return HttpContext.Current.Request.Cookies[key] != null
                ? decrypt(HttpContext.Current.Request.Cookies[key].Value)
                : string.Empty;
        }

        private string encrypt(string value)
        {
            value = HttpUtility.UrlEncode(value);
            return value;
        }

        private string decrypt(string value)
        {
            value = HttpUtility.UrlDecode(value);
            return value;
        }

        public void OverrideCookies(string keyName, string keyValue)
        {
            deleteCookies(keyName);
            setCookies(keyName, keyValue);
        }
    }
}