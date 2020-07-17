using System;
using WCloud.Framework.MVC;

namespace WCloud.Member.Api
{

    [System.AttributeUsage(AttributeTargets.Class)]
    public class MemberServiceRouteAttribute : ServiceRouteAttribute
    {
        public const string ServiceName = "member";
        public MemberServiceRouteAttribute() : base(ServiceName) { }
        public MemberServiceRouteAttribute(string group) : base(ServiceName, group) { }
    }
}
