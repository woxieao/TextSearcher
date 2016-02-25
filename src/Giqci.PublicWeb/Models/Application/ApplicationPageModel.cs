﻿using Giqci.Models.Dict;

namespace Giqci.PublicWeb.Models.Application
{
    public class ApplicationPageModel
    {
        public Giqci.Models.Application Application { get; set; }

        public bool AcceptTerms { get; set; }

        public Country[] Countries { get; set; }

        public HSCode[] CommonHSCodes { get; set; }

        public Port[] DestPorts { get; set; }

        public Port[] LoadingPorts { get; set; }
    }
}