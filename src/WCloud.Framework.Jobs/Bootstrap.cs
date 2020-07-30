using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;

namespace WCloud.Framework.Jobs
{
    internal static class Bootstrap
    {
        public static IApplicationBuilder UseQuartzWebUI(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.ToString().StartsWith("/quartz", StringComparison.CurrentCultureIgnoreCase))
                {
                    var valid = context.User?.Identity?.IsAuthenticated ?? false;
                    if (!valid)
                    {
                        context.Response.StatusCode = 403;
                        await context.Response.WriteAsync("无权限");
                    }
                }
                await next();
            });
            return app;
        }

        static void test()
        {
            //Hangfire.GlobalConfiguration.Configuration.UseStorage();

            IBackgroundJobClient client = new BackgroundJobClient();

            var jobid = client.Enqueue(() => Console.WriteLine(string.Empty));
            jobid = client.Schedule(() => Console.WriteLine(string.Empty), TimeSpan.FromHours(10));

            //client.Create()

            var server = new BackgroundJobServer(new BackgroundJobServerOptions() { });
            //server.Start();
            //server.Stop();
            server.Dispose();
        }
    }
}
