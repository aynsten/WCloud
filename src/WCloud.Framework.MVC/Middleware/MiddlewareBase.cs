using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace WCloud.Framework.MVC.Middleware
{
    public abstract class MiddlewareBase
    {
        protected readonly RequestDelegate _next;

        public MiddlewareBase(RequestDelegate next)
        {
            next.Should().NotBeNull();
            this._next = next;
        }

        public abstract Task Invoke(HttpContext context);
    }
}
