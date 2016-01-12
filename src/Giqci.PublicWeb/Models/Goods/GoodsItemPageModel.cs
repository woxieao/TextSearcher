using Giqci.Models.Dict;

namespace Giqci.PublicWeb.Models.Goods
{
    public class GoodsItemPageModel
    {
        public Giqci.Models.GoodsItem goods { get; set; }

        public Country[] Countries { get; set; }

        public HSCode[] CommonHSCodes { get; set; }

    }
}