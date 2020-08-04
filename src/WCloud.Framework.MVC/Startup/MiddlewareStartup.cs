using Lib.extension;
using Lib.helper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Reflection;

namespace WCloud.Framework.Startup
{
    public static class MiddlewareStartup
    {
        public static IApplicationBuilder UseAliveCheck(this IApplicationBuilder app)
        {
            app.Map("/alive", builder =>
            {
                builder.Run(async context =>
                {
                    var headers = context.Request.Headers.ToDictionary_(x => x.Key, x => x.Value);

                    var payload = new
                    {
                        headers,
                        server_time_utc = DateTime.UtcNow,
                        alived = true
                    }.ToJson();

                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync(payload);
                });
            });
            return app;
        }

        /// <summary>
        /// 为了方便把配置导入Apollo
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseConfigAsKV(this IApplicationBuilder app)
        {
            app.Map("/config-kv", option =>
            {
                option.Run(async context =>
                {
                    var config = context.RequestServices.ResolveConfig_();
                    var dict = config.ConfigAsKV();
                    var pairs = dict.OrderBy(x => x.Key).Select(x => $"{x.Key}={x.Value}");

                    var data = string.Join("\n", pairs);

                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync(data);
                });
            });
            return app;
        }

        public static IApplicationBuilder UseDbTableAsJson(this IApplicationBuilder app, Assembly[] ass)
        {
            if (ValidateHelper.IsEmpty(ass))
                throw new ArgumentNullException(nameof(ass));

            var models = ass.GetAllTypes().Where(x => x.IsNormalClass() && x.IsAssignableTo_<Lib.data.IDBTable>()).ToArray();

            var dict = models.ToDictionary_(x => x.FullName, x => Activator.CreateInstance(x));
            var json = dict.ToJson();

            app.Map("/table-define", option =>
            {
                option.Run(async context =>
                {
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(json);
                });
            });

            return app;
        }

        /// <summary>
        /// 捕获空白页
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseDefault404Response(this IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var data = new
                {
                    utc_now = DateTime.UtcNow,
                    message = "未匹配到mvc路由"
                }.ToJson();

                context.Response.StatusCode = 404;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(data);
            });
            return app;
        }
    }
}