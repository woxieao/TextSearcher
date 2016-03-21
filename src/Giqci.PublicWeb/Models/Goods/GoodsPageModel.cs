using Giqci.Chapi.Dict.Models;

namespace Giqci.PublicWeb.Models.Goods
{
    public class ProductPageModel
    {
        public Giqci.Models.Prod.ProductViewModel Goods { get; set; }

        public Country[] Countries { get; set; }

        public HSCode[] CommonHSCodes { get; set; }

        public string HSCodeDesc { get; set; }

        public string ManufacturerCountryDesc { get; set; }
    }
}