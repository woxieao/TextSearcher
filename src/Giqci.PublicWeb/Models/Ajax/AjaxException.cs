using System;


namespace Giqci.PublicWeb.Models.Ajax
{
    public class AjaxException : Exception
    {
        private readonly static string DefaultMsg = "服务器异常";

        public AjaxException() : base(DefaultMsg)
        {

        }

        public AjaxException(string errMsg) : base(errMsg)
        {

        }
    }
}