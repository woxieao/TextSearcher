using Giqci.Models;
using Giqci.PublicWeb.Models.Application;
using Giqci.PublicWeb.Services;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Web;

namespace Giqci.PublicWeb.Helpers
{
    public class CookieHelper
    {
        public bool SetApplication(string key, Application application)
        {
            SetCookies(key, JsonConvert.SerializeObject(application));
            return true;
        }

        public string GetApplication(string key)
        {
            return GetCookies(key);
        }

        private void SetCookies(string key, string value)
        {
            HttpCookie aCookie = new HttpCookie(key);
            aCookie.Value = System.Web.HttpContext.Current.Server.UrlEncode(value);
            aCookie.Expires = DateTime.Now.AddDays(1);
            HttpContext.Current.Response.Cookies.Add(aCookie);
        }

        private string GetCookies(string key)
        {
            string value = "";
            if (HttpContext.Current.Request.Cookies[key] != null)
            {
                value = System.Web.HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.Cookies[key].Value);
            }
            return value;
        }
    }
}