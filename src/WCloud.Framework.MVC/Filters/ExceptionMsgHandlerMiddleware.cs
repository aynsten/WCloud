using Lib.extension;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Framework.MVC.Middleware;

namespace WCloud.Framework.Filters
{
    public class ExceptionMsgHandlerMiddleware : MiddlewareBase
    {
        public ExceptionMsgHandlerMiddleware(RequestDelegate next) : base(next)
        { }

        public override async Task Invoke(HttpContext context)
        {
            var __context = context.RequestServices.Resolve_<IWCloudContext<ExceptionMsgHandlerMiddleware>>();
            var catch_all_exception = true;
            try
            {
                await this._next.Invoke(context);
            }
            catch (Exception e) when (catch_all_exception)
            {
                var err_handler = context.RequestServices.Resolve_<ICommonExceptionHandler>();
                var response = err_handler.Handle(e);
                var json = __context.DataSerializer.SerializeToString(response);

                context.Response.Clear();
                context.Response.ContentType = "application/json; charset=UTF-8";

                await context.Response.WriteAsync(json);
            }
        }
    }
}
