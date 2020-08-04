using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WCloud.Framework.Filters
{
    public class CommonExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var e = context.Exception;

            if (e == null || context.ExceptionHandled)
                return;

            var err_handler = context.HttpContext.RequestServices.Resolve_<ICommonExceptionHandler>();
            var response = err_handler.Handle(e);

            context.HttpContext.Response.Clear();

            var result = new JsonResult(response);
            if (response.HttpStatusCode != null)
            {
                result.StatusCode = (int)response.HttpStatusCode.Value;
            }

            /*
             
            var res = new ContentResult()
            {
                Content = response.ToJson(),
                ContentType = "application/json"
            };
             */

            context.Result = result;
            context.Exception = null;
            context.ExceptionHandled = true;
        }
    }
}