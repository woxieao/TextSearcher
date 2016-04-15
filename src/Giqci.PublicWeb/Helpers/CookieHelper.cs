using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web;
using Giqci.Chapi.Models.App;

namespace Giqci.PublicWeb.Helpers
{
    public class CookieHelper
    {
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

        private void setCookies(string key, string value, int expiryDays = 7)
        {
            var cookie = new HttpCookie(key, encrypt(value)) {Expires = DateTime.Now.AddDays(expiryDays)};

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

        public void OverrideCookies(string keyName, string keyValue, int expiryDays = 7)
        {
            var cookie = new HttpCookie(keyName, encrypt(keyValue));
            cookie.Expires = DateTime.Now.AddDays(expiryDays);
            HttpContext.Current.Response.Cookies.Set(cookie);
            //deleteCookies(keyName);
            //setCookies(keyName, keyValue);
        }
    }
}