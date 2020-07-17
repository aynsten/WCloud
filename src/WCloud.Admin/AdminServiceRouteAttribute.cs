using System;
using WCloud.Framework.MVC;

namespace WCloud.Admin
{
    [System.AttributeUsage(AttributeTargets.Method)]
    public class AdminRouteAttribute : MyRouteAttribute
    {
        public const string ServiceName = AdminServiceRouteAttribute.ServiceName;

        public AdminRouteAttribute(string template) : base(template: template) { }
        public AdminRouteAttribute() : base(prefix: "api", service_name: ServiceName, controller: null, action: null)
        {
            //
        }
    }

    /// <summary>
    /// api/service_name
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Class)]
    public class AdminServiceRouteAttribute : ServiceRouteAttribute
    {
        public const string ServiceName = "admin";
        public AdminServiceRouteAttribute() : base(ServiceName) { }
        public AdminServiceRouteAttribute(string group) : base(ServiceName, group) { }
    }
}
