namespace Giqci.PublicWeb.Models.Ajax
{
    public class AjaxResultPackage
    {
        public RequestStatus Flag { get; set; }
        public string Msg { get; set; }
        public CallBackClass CallBackPackage { get; set; }
        public object Data { get; set; }

    }
    public class CallBackClass
    {
        public object CallBackFunc { get; set; }
        public object CallBackUrl { get; set; }
    }

    public enum RequestStatus
    {
        LogOut = -1,
        Error = 0,
        Success = 1,
    }
}