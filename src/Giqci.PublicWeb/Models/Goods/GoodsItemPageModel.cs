﻿using Giqci.Chapi.Models.Dict;

namespace Giqci.PublicWeb.Models.Goods
{
    public class GoodsItemPageModel
    {
     //   public Giqci.Models.GoodsItem goods { get; set; }

        public string HSCodeDesc { get; set; }

        public string ManufacturerCountryDesc { get; set; }

        public Country[] Countries { get; set; }

        public HsCode[] CommonHSCodes { get; set; }

    }
}