using Giqci.PublicWeb.Models.Application;
using Giqci.PublicWeb.Services;

namespace Giqci.PublicWeb.Helpers
{
    public static class ModelBuilder
    {
        public static void SetHelperFields(ICacheService cache, ApplicationPageModel model)
        {
            model.Countries = cache.GetCountries();

            model.CommonHSCodes = cache.GetCommonHSCodes();

            model.DestPorts = cache.GetDestPorts();

            model.LoadingPorts = cache.GetLoadingPorts();
        }
    }
}