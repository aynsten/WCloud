using FluentAssertions;
using Lib.extension;
using Microsoft.AspNetCore.Mvc;
using System;

namespace WCloud.Framework.MVC
{
    /// <summary>
    /// api/service_name
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Class)]
    public class ServiceRouteAttribute : RouteAttribute
    {
        public ServiceRouteAttribute(string name) : base(template: $"api/{name}")
        {
            name.Should().NotBeNullOrEmpty();
        }

        public ServiceRouteAttribute(string name, string group) : base(template: $"api/{name}/{group}")
        {
            name.Should().NotBeNullOrEmpty();
            group.Should().NotBeNullOrEmpty();
        }
    }

    /// <summary>
    /// api-path
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Method)]
    public class ApiRouteAttribute : RouteAttribute
    {
        public ApiRouteAttribute() : base(template: "[controller]/[action]") { }

        public ApiRouteAttribute(string template) : base(template: template)
        {
            template.Should().NotBeNullOrEmpty();
        }
    }

    [System.AttributeUsage(AttributeTargets.Method)]
    public class MyRouteAttribute : RouteAttribute
    {
        public MyRouteAttribute(string template) : base(template: template) { }

        public MyRouteAttribute() : this("api", null, null, null) { }

        static string __template__(string prefix, string service_name, string controller, string action)
        {
            var path = new[] {
                prefix,
                service_name,
                controller ?? "[controller]",
                action ?? "[action]"
            };

            var res = string.Join("/", path.WhereNotEmpty());
            return res;
        }

        public MyRouteAttribute(string prefix, string service_name, string controller, string action) :
            base(template: __template__(prefix, service_name, controller, action))
        {
            //
        }
    }
}
