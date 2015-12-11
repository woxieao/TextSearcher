using Giqci.PublicWeb.Models.Application;
using Giqci.Services;

namespace Giqci.PublicWeb.Helpers
{
    public static class ModelBuilder
    {
        public static void SetHelperFields(ICachedDictionaryService cache, ApplicationPageModel model)
        {
            model.Countries = cache.GetCountries();

            model.CommonHSCodes = cache.GetCommonHSCodes();

            model.DestPorts = cache.GetDestPorts();

            model.LoadingPorts = cache.GetLoadingPorts();
        }
    }
}