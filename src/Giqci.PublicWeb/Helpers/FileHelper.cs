using Giqci.PublicWeb.Models.Application;
using Giqci.PublicWeb.Services;
using System;
using System.IO;

namespace Giqci.PublicWeb.Helpers
{
    public static class FileHelper
    {
        public static string GetInterIDList(string strfile)
        {
            string strout = "";
            string FilePath = System.Web.HttpContext.Current.Server.MapPath(strfile);
            if (File.Exists(FilePath))
            {
                StreamReader sr = new StreamReader(System.Web.HttpContext.Current.Server.MapPath(strfile), System.Text.Encoding.Default);
                String input = sr.ReadToEnd();
                sr.Close();
                strout = input;
            }
            return strout;
        }
    }
}