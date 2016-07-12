using System;
using Giqci.PublicWeb.Extensions;


namespace Giqci.PublicWeb.Models.Ajax
{
    public class AjaxException : Exception
    {
        private readonly static string DefaultMsg = "server_exception".KeyToWord();

        public AjaxException() : base(DefaultMsg)
        {

        }

        public AjaxException(string errMsg) : base(errMsg)
        {

        }
    }
}