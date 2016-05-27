namespace Giqci.PublicWeb.Models.Ajax
{
    public class AjaxResultPackage
    {
        public RequestStatus status { get; set; }
        public string msg { get; set; }
        public CallBackClass callBackPackage { get; set; }
        public object data { get; set; }

    }
    public class CallBackClass
    {
        public object callBackFunc { get; set; }
        public string callBackUrl { get; set; }
    }

    public enum RequestStatus
    {
        LogOut = -1,
        Error = 0,
        Success = 1,
    }
}