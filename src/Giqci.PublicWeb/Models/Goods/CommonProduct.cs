using System;
using Giqci.Chapi.Models.Dict;
using Giqci.Chapi.Models.Product;

namespace Giqci.PublicWeb.Models.Goods
{
    public class CommonProduct : Product
    {
        public int Id { get; set; }
        public string Code { get; set; }

        public DateTime CreateTime { get; set; }

        public int? CustomProductId { get; set; }

        public string ProductKey { get; set; }
    }
}