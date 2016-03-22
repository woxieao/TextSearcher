using Giqci.Chapi.Dict.Models;
using Giqci.Chapi.Prod.Models;

namespace Giqci.PublicWeb.Models.Goods
{
    public class ProductPageModel
    {
        public Product Goods { get; set; }

        public Country[] Countries { get; set; }

        public HSCode[] CommonHSCodes { get; set; }

        public string HSCodeDesc { get; set; }

        public string ManufacturerCountryDesc { get; set; }
    }
}