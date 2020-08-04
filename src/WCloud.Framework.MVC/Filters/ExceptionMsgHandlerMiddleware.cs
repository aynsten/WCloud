using Lib.extension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using WCloud.Framework.Middleware;

namespace WCloud.Framework.Filters
{
    public class ExceptionMsgHandlerMiddleware : _BaseMiddleware
    {
        public ExceptionMsgHandlerMiddleware(RequestDelegate next) : base(next)
        { }

        public override async Task Invoke(HttpContext context)
        {
            var catch_all_exception = true;
            try
            {
                await this._next.Invoke(context);
            }
            catch (Exception e) when (catch_all_exception)
            {
                var err_handler = context.RequestServices.Resolve_<ICommonExceptionHandler>();
                var response = err_handler.Handle(e);
                var json = response.ToJson();

                context.Response.Clear();
                context.Response.ContentType = "application/json; charset=UTF-8";

                await context.Response.WriteAsync(json);
            }
        }
    }
}
