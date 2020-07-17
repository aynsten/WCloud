using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Lib.extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace WCloud.Framework.Startup
{
    public static class CorsStartup
    {
        /// <summary>
        /// 允许任意跨域请求
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseDevCors(this IApplicationBuilder app)
        {
            void __config_dev__(CorsPolicyBuilder builder)
            {
                builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                //.AllowCredentials()
                .AllowAnyOrigin();
            }

            app.UseCors(__config_dev__);

            return app;
        }

        /// <summary>
        /// 使用生产环境的跨域配置
        /// </summary>
        /// <param name="app"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseProductionCors(this IApplicationBuilder app, IConfiguration config)
        {

            void __config_production__(CorsPolicyBuilder builder)
            {
                var origin_config = config["CorsHosts"] ?? string.Empty;

                var origins = origin_config.Split(',').WhereNotEmpty().Distinct().ToArray();

                origins.Should().NotBeNullOrEmpty("请配置跨域所需的origin hosts");

                builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .WithOrigins(origins: origins);
            }

            app.UseCors(__config_production__);

            return app;
        }
    }
}
