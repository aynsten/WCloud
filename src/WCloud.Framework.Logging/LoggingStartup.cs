﻿using Lib.extension;
using Lib.helper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using WCloud.Core;
using WCloud.Framework.IO;

namespace WCloud.Framework.Logging
{
    public static class LoggingStartup
    {
        public static IWCloudBuilder AddLoggingAll(this IWCloudBuilder builder, string nlog_config_file_path)
        {
            builder.Services.AddLoggingAll(builder.Configuration, nlog_config_file_path);
            return builder;
        }

        public static IServiceCollection AddLoggingAll(this IServiceCollection collection, IConfiguration config, string nlog_config_file_path)
        {
            collection.AddLogging(builder =>
            {
                builder.ClearProviders();
#if DEBUG
                //console debug
                builder.AddConsole().AddDebug();
#endif
                builder.__add_nlog__(config, nlog_config_file_path);
                builder.__try_add_sentry__(config);
                builder.__add_logger_filter__(config);
            });
            return collection;
        }

        /// <summary>
        /// nlog
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="config"></param>
        /// <param name="env"></param>
        public static ILoggingBuilder __add_nlog__(this ILoggingBuilder builder, IConfiguration config, string config_file)
        {
            var nlog_config_path = config_file;//Path.Combine(env.ContentRootPath, "nlog.config");
            if (File.Exists(nlog_config_path))
            {
                var xml = File.ReadAllText(nlog_config_path);

                var log_path = config["log_base_dir"];
                if (ValidateHelper.IsNotEmpty(log_path) && Directory.Exists(log_path))
                {
#if xx
                        var app_name = new[] {
                            config.GetAppName() ,
                            System.Reflection.Assembly.GetEntryAssembly()?.GetName()?.Name,
                            "app"
                        }.FirstNotEmpty_();

                        log_path = Path.Combine(log_path, app_name);
#endif

                    new DirectoryInfo(log_path).CreateIfNotExist();

                    var nlog_config_data = NLog.Config.XmlLoggingConfiguration.CreateFromXmlString(xml);
                    nlog_config_data.Variables["log_base_dir"] = log_path;

                    builder.AddNLog(nlog_config_data);
                }
            }
            return builder;
        }

        public static string GetSentryDsn(this IConfiguration config)
        {
            var dsn = config["sentry_dsn"];

            return dsn;
        }

        /// <summary>
        /// sentry
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="config"></param>
        public static ILoggingBuilder __try_add_sentry__(this ILoggingBuilder builder, IConfiguration config)
        {
            //sentry
            var sentry_dsn = config.GetSentryDsn();
            if (ValidateHelper.IsNotEmpty(sentry_dsn))
            {
                builder.AddSentry(option =>
                {
                    option.Dsn = sentry_dsn;
                    option.MinimumEventLevel = LogLevel.Error;
                    option.MinimumBreadcrumbLevel = LogLevel.Error;
                });
            }
            return builder;
        }

        public static ILoggingBuilder __add_logger_filter__(this ILoggingBuilder builder, IConfiguration config)
        {
            var ignore_logger = (config["ignore_logger"] ?? string.Empty).Split(',').WhereNotEmpty().ToArray();

            bool __filter__(string logger, LogLevel level)
            {
                if (ValidateHelper.IsNotEmpty(logger))
                {
                    if (ignore_logger.Any(x => logger.StartsWith(x, StringComparison.OrdinalIgnoreCase)))
                    {
                        if (level < LogLevel.Warning)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            if (ignore_logger.Any())
            {
                builder.AddFilter(__filter__);
            }
            return builder;
        }

    }
}
