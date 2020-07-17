using System;
using WCloud.Framework.MVC;

namespace WCloud.MetroAd.Api
{
    /// <summary>
    /// metro-ad
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Class)]
    public class MetroAdServiceRouteAttribute : ServiceRouteAttribute
    {
        public const string ServiceName = "metro-ad";
        public MetroAdServiceRouteAttribute() : base(ServiceName) { }
        public MetroAdServiceRouteAttribute(string group) : base(ServiceName, group) { }
    }
}
