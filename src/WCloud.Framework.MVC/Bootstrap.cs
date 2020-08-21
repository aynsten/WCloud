using FluentAssertions;
using Lib.helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Volo.Abp.VirtualFileSystem;
using WCloud.Core;
using WCloud.Framework.Filters;
using WCloud.Framework.Logging;

namespace WCloud.Framework.MVC
{
    public static class Bootstrap
    {
        public static IServiceProvider CurrentServiceProvider(this HttpContext context)
        {
            var res = context.RequestServices ?? throw new Exception("无法获取当前ioc scope");
            return res;
        }

        public static IServiceCollection AddWCloudMvc(this IServiceCollection collection)
        {
            var config = collection.GetConfiguration();
            var env = collection.GetHostingEnvironment();
            //解决中文被编码
            collection.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));
            //use httpcontext
            collection.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            collection.RemoveAll<IHttpContextAccessor>().AddHttpContextAccessor();
            //异常捕捉
            collection.AddSingleton<ICommonExceptionHandler, CommonExceptionHandler>();
            //加密相关
            collection.AddFileBasedDataProtection_(config, env);
            //添加mvc组件
            collection.AddRouting().AddMvc(option => option.AddExceptionHandler()).AddJsonProvider_();
            return collection;
        }

        public static IWCloudBuilder AddWCloudMvc(this IWCloudBuilder builder)
        {
            builder.Services.AddWCloudMvc();
            return builder;
        }

        public static string NLogConfigFilePath(this IWebHostEnvironment env)
        {
            var res = Path.Combine(env.ContentRootPath, "nlog.config");
            return res;
        }

        public static IWCloudBuilder AddLoggingAll(this IWCloudBuilder builder)
        {
            var nlog_config_file_path = builder.Services.GetHostingEnvironment().NLogConfigFilePath();
            LoggingStartup.AddLoggingAll(builder, nlog_config_file_path);
            return builder;
        }

        static IDataProtectionBuilder AddFileBasedDataProtection_(this IServiceCollection collection, IConfiguration config, IWebHostEnvironment env)
        {
            var app_name = config.GetAppName() ?? "shared_app";

            var builder = collection
                .AddDataProtection()
                .SetApplicationName(applicationName: app_name)
                .AddKeyManagementOptions(option =>
                {
                    option.AutoGenerateKeys = true;
                    option.NewKeyLifetime = TimeSpan.FromDays(1000);
                });
            builder.PersistKeysToFileSystem(new DirectoryInfo(env.ContentRootPath));

            return builder;
        }

        /// <summary>
        /// 拦截通用错误，并给约定返回
        /// 这个中间件的注册必须紧跟use mvc之前
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseMsgExceptionHandlerMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMsgHandlerMiddleware>();
            return app;
        }

        public static MvcOptions AddExceptionHandler(this MvcOptions option)
        {
            //abp在AbpAspNetCoreMvcModule就添加了异常处理的过滤器
            //TODO 这里的优先级要考虑下
            option.Filters.Add<CommonExceptionFilter>();
            return option;
        }

        public static IMvcBuilder AddJsonProvider_(this IMvcBuilder builder)
        {
            builder.AddNewtonsoftJson(option =>
            {
                option.SerializerSettings.Converters = JsonHelper._setting.Converters;
                option.SerializerSettings.NullValueHandling = JsonHelper._setting.NullValueHandling;
                //key大小写
                option.SerializerSettings.ContractResolver = JsonHelper._setting.ContractResolver;
                //option.UseCamelCasing(true);
            });
            return builder;
        }

        public static IServiceCollection AbpReplaceEmbeddedByPhysical<T>(this IServiceCollection services)
        {
            var app = services.GetHostingEnvironment();
            if (app.IsDevelopment())
            {
                services.Configure<AbpVirtualFileSystemOptions>(options =>
                {
                    var ns = typeof(T).Namespace;

                    var path = new DirectoryInfo(app.ContentRootPath).Parent.FullName;
                    path = Path.Combine(path, ns);
                    File.Exists(path).Should().BeTrue(path);

                    options.FileSets.ReplaceEmbeddedByPhysical<T>(path);
                });
            }

            return services;
        }
    }
}
