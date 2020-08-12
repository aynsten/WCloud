using System;
using WCloud.Framework.MVC;

namespace WCloud.Admin
{
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
