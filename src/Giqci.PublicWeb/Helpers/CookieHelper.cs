using Giqci.Models;
using Newtonsoft.Json;
using System;
using System.Web;

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

        private void setCookies(string key, string value, int expiryDays)
        {
            var cookie = new HttpCookie(key, encrypt(value));
            cookie.Expires = DateTime.Now.AddDays(expiryDays);

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        private string getCookies(string key)
        {
            return HttpContext.Current.Request.Cookies[key] != null ? decrypt(HttpContext.Current.Request.Cookies[key].Value) : string.Empty;
        }

        private string encrypt(string value)
        {
            return value;
        }

        private string decrypt(string value)
        {
            return value;
        }
    }
}