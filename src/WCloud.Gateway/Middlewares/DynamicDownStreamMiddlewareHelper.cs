using Lib.extension;
using Lib.ioc;
using Ocelot.Middleware;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WCloud.Gateway.Middlewares
{
    public static class DynamicDownStreamMiddlewareHelper
    {
        public static void __config_ocelot__(OcelotPipelineConfiguration option)
        {
            //只要想办法在管道上游设置re-route就可以了
            option.PreErrorResponderMiddleware = PreErrorResponderMiddleware;
            option.PreAuthenticationMiddleware = PreAuthenticationMiddleware;
            option.AuthenticationMiddleware = AuthenticationMiddleware;
            option.PreAuthorisationMiddleware = PreAuthorisationMiddleware;
            option.AuthorisationMiddleware = AuthorisationMiddleware;
            option.PreQueryStringBuilderMiddleware = PreQueryStringBuilderMiddleware;
        }

        static async Task PreErrorResponderMiddleware(DownstreamContext context, Func<Task> next)
        {
            await next.Invoke();
        }
        static async Task PreAuthenticationMiddleware(DownstreamContext context, Func<Task> next)
        {
            await next.Invoke();
        }
        static async Task AuthenticationMiddleware(DownstreamContext context, Func<Task> next)
        {
            await next.Invoke();
        }
        static async Task PreAuthorisationMiddleware(DownstreamContext context, Func<Task> next)
        {
            await next.Invoke();
        }
        static async Task AuthorisationMiddleware(DownstreamContext context, Func<Task> next)
        {
            await next.Invoke();
        }

        /// <summary>
        /// 用来转发请求到单独的部署实例
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        static async Task PreQueryStringBuilderMiddleware(DownstreamContext context, Func<Task> next)
        {
            if (context.HttpContext.Request.Path.StartsWithSegments("/api/unknow"))
            {
                var client = context.HttpContext.RequestServices.Resolve_<IHttpClientFactory>().CreateClient("gateway");
                var req = context.DownstreamRequest.ToHttpRequestMessage();

                var paths = context.DownstreamRequest.AbsolutePath.Split('/').WhereNotEmpty().ToArray();
                paths[1] = "admin";

                req.RequestUri = new Uri($"http://localhost:5000/" + string.Join("/", paths));

                var res = await client.SendAsync(req);

                context.DownstreamResponse = new DownstreamResponse(res);

                return;
            }
            await next.Invoke();
        }
    }
}
