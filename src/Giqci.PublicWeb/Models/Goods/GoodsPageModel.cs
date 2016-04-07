using Giqci.Chapi.Models.Dict;
using Giqci.Chapi.Models.Product;

namespace Giqci.PublicWeb.Models.Goods
{
    public class ProductPageModel
    {
        public Product Goods { get; set; }

        public Country[] Countries { get; set; }

        public HsCode[] CommonHSCodes { get; set; }

        public string HSCodeDesc { get; set; }

        public string ManufacturerCountryDesc { get; set; }
    }
}