using System;
using WCloud.Framework.MVC;

namespace WCloud.Gateway
{
    [System.AttributeUsage(AttributeTargets.Method)]
    public class GatewayRouteAttribute : ServiceRouteAttribute
    {
        public const string ServiceName = "gateway";
        public GatewayRouteAttribute() : base(ServiceName) { }
        public GatewayRouteAttribute(string group) : base(ServiceName, group) { }
    }
}
