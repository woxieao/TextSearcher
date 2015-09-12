using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Giqci.PublicWeb.Models.Application;
using Giqci.PublicWeb.Services;

namespace Giqci.PublicWeb.Helpers
{
    public static class ModelBuilder
    {
        public static void SetHelperFields(ICacheService cache, ApplicationViewModel model)
        {
            model.Countries = cache.GetCountries().Select(i => new SelectListItem { Text = i.Value, Value = i.Key, Selected = i.Key == "036" }).ToList();

            model.CommonHSCodes = new List<SelectListItem>();
            foreach (var item in cache.GetCommonHSCodes().GroupBy(i => i.Category))
            {
                var group = new SelectListGroup { Name = item.Key };
                model.CommonHSCodes.AddRange(item.Select(i => new SelectListItem { Text = i.Name, Value = i.Code, Group = group }).ToList());
            }

            model.DestPorts = new List<SelectListItem>();
            foreach (var item in cache.GetDestPorts().GroupBy(i => i.Country))
            {
                var group = new SelectListGroup { Name = item.Key };
                model.DestPorts.AddRange(item.OrderBy(i=>i.Name).Select(i => new SelectListItem { Text = i.Name, Value = i.Code, Group = group }).ToList());
            }

            model.LoadingPorts = new List<SelectListItem>();
            foreach (var item in cache.GetLoadingPorts().GroupBy(i => i.Country))
            {
                var group = new SelectListGroup { Name = item.Key };
                model.LoadingPorts.AddRange(item.OrderBy(i => i.Name).Select(i => new SelectListItem { Text = i.Name, Value = i.Code, Group = group }).ToList());
            }
        }
    }
}