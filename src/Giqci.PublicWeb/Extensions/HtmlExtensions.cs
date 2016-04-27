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
    }
}