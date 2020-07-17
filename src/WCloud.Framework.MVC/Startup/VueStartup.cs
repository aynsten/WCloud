using Lib.cache;
using Lib.extension;
using Lib.helper;
using Lib.ioc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core.Cache;
using WCloud.Framework.MVC.Extension;
//using Microsoft.AspNetCore.Rewrite;

namespace WCloud.Framework.Startup
{
    class VueStartupLogger { }

    /// <summary>
    /// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/url-rewriting?tabs=aspnetcore2x&view=aspnetcore-2.2
    /// </summary>
    public static class VueStartup
    {
        public static IApplicationBuilder UseAppHtmlRender(this IApplicationBuilder app, string[] apps)
        {
            if (ValidateHelper.IsEmpty(apps))
                throw new ArgumentNullException(nameof(apps));
            if (apps.Any(x => x == null))
                throw new ArgumentException("apps存在null");

            app.Use(async (context, next) =>
            {
                async Task SendFile(string file_path)
                {
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "text/html";
                    await context.Response.SendFileAsync(file_path);
                }

                async Task SendHtmlOr404(string html)
                {
                    context.Response.Clear();
                    context.Response.ContentType = "text/html";
                    if (html == null)
                    {
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync("app not found");
                    }
                    else
                    {
                        context.Response.StatusCode = 200;
                        await context.Response.WriteAsync(html);
                    }
                }

                string path = context.Request.Path;
                var app_name = path?.Split('/').WhereNotEmpty().FirstOrDefault();
                var app_dir = apps.FirstOrDefault(x => x.Equals(app_name, StringComparison.InvariantCultureIgnoreCase));

                if (ValidateHelper.IsNotEmpty(app_dir))
                {
                    var env = context.RequestServices.ResolveHostingEnvironment_();
                    var logger = context.RequestServices.Resolve_<ILogger<VueStartupLogger>>();
                    var cache = context.RequestServices.Resolve_<ICacheProvider>();
                    var keyManager = context.RequestServices.Resolve_<ICacheKeyManager>();

                    var p = System.IO.Path.Combine(env.WebRootPath, app_dir, "index.html");

                    async Task<string> ReadHtml()
                    {
                        if (!System.IO.File.Exists(p))
                        {
                            logger.AddErrorLog($"{p}不存在");
                            return null;
                        }
                        return await System.IO.File.ReadAllTextAsync(p);
                    }

                    var key = keyManager.Html(p);

                    var expired = env.IsDevelopment() ? TimeSpan.FromSeconds(10) : TimeSpan.FromMinutes(10);

                    //测试环境不使用缓存
                    var html_data = await cache.GetOrSetAsync_(key, ReadHtml, expire: expired);

                    await SendHtmlOr404(html_data);
                }
                else
                {
                    await next.Invoke();
                }
            });
            return app;
        }

        /*
        public static IApplicationBuilder UseVueRewrite(this IApplicationBuilder app)
        {
            var rewrite = new RewriteOptions()
                .AddRedirect(@"^V(/.*)?$", "/v$1")
                .AddRewrite(@"^v(/.*)?$", "/app/index.html", true)
                .Add(new VueRewriteRule());

            app.UseRewriter(rewrite);
            return app;
        }

        class VueRewriteRule : IRule
        {
            public void ApplyRule(RewriteContext context)
            {
                //do nothing
            }
        }
        */
    }
}
