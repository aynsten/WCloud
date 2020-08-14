using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;

namespace WCloud.Framework.MVC
{
    /// <summary>
    /// api/service_name
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
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

        public ServiceRouteAttribute(string prefix, string name, string group) : base(template: $"{prefix}/{name}/{group}")
        {
            prefix.Should().NotBeNullOrEmpty();
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
}
