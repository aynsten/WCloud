using System;
using WCloud.Framework.MVC;

namespace WCloud.CommonService.Api
{
    [System.AttributeUsage(AttributeTargets.Class)]
    public class CommonServiceRouteAttribute : ServiceRouteAttribute
    {
        public const string ServiceName = "common-service";
        public CommonServiceRouteAttribute() : base(ServiceName) { }
        public CommonServiceRouteAttribute(string group) : base(ServiceName, group) { }
    }
}
