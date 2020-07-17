using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace WCloud.Framework.Middleware
{
    public abstract class _BaseMiddleware
    {
        protected readonly RequestDelegate _next;

        public _BaseMiddleware(RequestDelegate next) => 
            this._next = next ?? throw new ArgumentNullException(nameof(next));

        public abstract Task Invoke(HttpContext context);
    }
}
