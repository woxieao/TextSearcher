namespace Giqci.PublicWeb.Models.Ajax
{
    public class AjaxResultPackage
    {
        //由于序列化时不同的序列化设置 enum转换的值可能为int or string 故在此用int算了
        //-1 logout
        //0  error
        //1  success
        public int status { get; set; }
        public string msg { get; set; }
        public CallBackClass callBackPackage { get; set; }
        public object data { get; set; }

    }
    public class CallBackClass
    {
        public object callBackFunc { get; set; }
        public string callBackUrl { get; set; }
    }
}