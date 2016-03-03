using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Giqci.Enums;
using Giqci.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Giqci.PublicWeb.Models.Application
{
    public abstract class BaseApplication
    {
        public string ApplyNo { get; set; }

        public bool C101 { get; set; }

        public bool C102 { get; set; }

        public bool C103 { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TradeType TradeType { get; set; }

        public string ApplicantCode { get; set; }

        public string Applicant { get; set; }

        public string ApplicantAddr { get; set; }

        public string ApplicantContact { get; set; }

        public string ApplicantPhone { get; set; }

        public string ApplicantEmail { get; set; }

        public string Exporter { get; set; }

        public string ExporterAddr { get; set; }

        public string ExporterContact { get; set; }

        public string ExporterPhone { get; set; }

        /// <summary>
        /// 提单号
        /// </summary>
        public string Billno { get; set; }

        public string OtherBillno { get; set; }

        public string Vesselcn { get; set; }

        /// <summary>
        /// 航次
        /// </summary>
        public string Voyage { get; set; }

        public List<GoodsItem> Goods { get; set; }

        public List<ContainerInfo> ContainerInfoList { get; set; }
        protected BaseApplication()
        {
            Goods = new List<GoodsItem>();
          //  ContainerInfoList = new List<ContainerInfo>();
        }
    }
}