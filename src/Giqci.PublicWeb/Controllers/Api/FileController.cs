﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using Ktech.Mvc.ActionResults;
using System.Web;
using Giqci.Interfaces;
using Giqci.PublicWeb.Models;
using Giqci.Tools;

namespace Giqci.PublicWeb.Controllers.Api
{
    [RoutePrefix("file")]
    public class FileController : Controller
    {
        [Route("viewfile")]
        [HttpGet]
        public ActionResult ViewFile(string fileFullPath)
        {
            return Redirect(string.Format("{0}/{1}", Config.Common.ViewFileUrl, Filer.EncryptFilePath(fileFullPath)));
        }


    }
}