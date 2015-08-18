using System.Web.Mvc;
using System.Web.Routing;

namespace Giqci.PublicWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapMvcAttributeRoutes();
        }
    }
}
