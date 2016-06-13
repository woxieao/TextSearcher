using System;
using System.Web;
using Newtonsoft.Json;

namespace Giqci.PublicWeb.Extensions
{
    public static class HtmlExtensions
    {
        public static HtmlString RawJson(object obj)
        {
            //输出为字符串时"\\"会转义掉,故需添加一层转义
            var str = JsonConvert.SerializeObject(obj).Replace("\\", "\\\\");
            str = HttpUtility.HtmlEncode(str);
            return new HtmlString(string.Format("'{0}'", str));
        }

        private static readonly string[] _monthList = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        public static string DateToString(this DateTime date)
        {
            return date.Day.ToString("D2") + " " + _monthList[date.Month - 1] + " " + date.Year;
        }
    }
}