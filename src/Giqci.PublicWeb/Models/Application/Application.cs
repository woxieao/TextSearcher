using System;
using Giqci.Chapi.Enums.App;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Giqci.PublicWeb.Models.Application
{
    public class Application : BaseApplication
    {
        public string Importer { get; set; }

        public string ImporterAddr { get; set; }

        public string ImporterContact { get; set; }

        public string ImporterPhone { get; set; }

        public string ImBroker { get; set; }

        public string ImBrokerAddr { get; set; }

        public string ImBrokerContact { get; set; }

        public string ImBrokerPhone { get; set; }

     
        public string ShippingDate { get; set; }

        public string InspectionAddr { get; set; }

        public DateTime InspectionDate { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ShippingMethod ShippingMethod { get; set; }

        public string LoadingPort { get; set; }

   
        public string DestPort { get; set; }

     
        public decimal TotalWeight { get; set; }

        public int TotalUnits { get; set; }

        public string Key { get; set; }
        public DateTime CreateDate { get; set; }
        public string StatusDescription { get; set; }

        public string DeclineReason { get; set; }

    }
}