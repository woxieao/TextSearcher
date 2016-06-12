﻿using Giqci.Interfaces;
using Giqci.PublicWeb.Models.Application;
using Giqci.PublicWeb.Models.Goods;

namespace Giqci.PublicWeb.Helpers
{
    public static class ModelBuilder
    {
        //public static void SetHelperFields(IDictService dict, Application model)
        //{
        //    model.Countries = dict.GetCountries();

        //    model.CommonHSCodes = dict.SearchHSCodes("", 20);

        //    model.DestPorts = dict.SearchPorts("", 20);

        //    model.LoadingPorts = dict.SearchPorts("", 20);
        //}


        public static void SetHelperGoods(IDictService dict, GoodsItemPageModel model)
        {
            model.Countries = dict.GetCountries();

            model.CommonHSCodes = dict.SearchHSCodes("", 20);
        }


        public static void SetHelperGoodsModel(IDictService dict, ProductPageModel model)
        {
            model.Countries = dict.GetCountries();

            model.CommonHSCodes = dict.SearchHSCodes("", 20);
        }
    }
}