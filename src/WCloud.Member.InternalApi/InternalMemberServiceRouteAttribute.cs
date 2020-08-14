using Microsoft.AspNetCore.Mvc;
using System;

namespace WCloud.Member.InternalApi
{
    [AttributeUsage(AttributeTargets.Class)]
    public class InternalMemberServiceRouteAttribute : RouteAttribute
    {
        public const string ServiceName = "member";
        public InternalMemberServiceRouteAttribute() : base(template: $"internal-api/{ServiceName}") { }
        public InternalMemberServiceRouteAttribute(string group) : base(template: $"internal-api/{ServiceName}/{group}") { }
    }
}
