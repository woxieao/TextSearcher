using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Giqci.Models.Dict;

namespace Giqci.PublicWeb.Models.Goods
{
    public class GoodsPageModel
    {
        public Giqci.Models.GoodsModel Goods { get; set; }

        public Country[] Countries { get; set; }

        public HSCode[] CommonHSCodes { get; set; }

        public string HSCodeDesc { get; set; }

        public string ManufacturerCountryDesc { get; set; }
    }
}