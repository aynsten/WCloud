using System;
using WCloud.Framework.MVC;

namespace WCloud.Gateway
{
    [System.AttributeUsage(AttributeTargets.Method)]
    public class GatewayRouteAttribute : MyRouteAttribute
    {
        public const string ServiceName = "gateway";
        public GatewayRouteAttribute(string template) : base(template: template) { }
        public GatewayRouteAttribute() : base(prefix: "api", service_name: ServiceName, controller: null, action: null)
        {
            //
        }
    }
}
