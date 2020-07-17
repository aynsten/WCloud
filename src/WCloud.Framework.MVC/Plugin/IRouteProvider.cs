using Microsoft.AspNetCore.Routing;

namespace WCloud.Framework.MVC.Plugin
{
    public interface IRouteProvider
    {
        void RegisterRoutes(RouteCollection routes);

        int Priority { get; }
    }
}
