using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace WCloud.Framework.Startup
{
    public static class DataProtectionStartup
    {
        public static IDataProtectionBuilder AddFileBasedDataProtection_(this IServiceCollection collection,
            IConfiguration config,
            IWebHostEnvironment env)
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
    }
}
