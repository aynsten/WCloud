using Elastic.Apm;
using Elastic.Apm.Api;
using Elastic.Apm.NetCoreAll;
using Lib.extension;
using Lib.helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using AspectCore.Configuration;
using System;
using System.Linq;
using System.Reflection;

namespace WCloud.Framework.Apm
{
    public static class Bootstrap
    {
        /*
         
            "ElasticApm": {
    "LogLevel": "Debug",
    "ServerUrls": "http://3h.elk.hiwj.cn:8200",
    "TransactionSampleRate": 1.0
  }

             */

        public static bool IsApmServerConfigAvaliable(this IConfiguration config)
        {
            var urls = config["ElasticApm:ServerUrls"];
            var server_exist = ValidateHelper.IsNotEmpty(urls);

            return server_exist;
        }

        /// <summary>
        /// apm是否可用
        /// </summary>
        public static bool IsApmAgentConfigured(this IServiceProvider provider)
        {
            var res = Elastic.Apm.Agent.IsConfigured;
            return res;
        }

        /// <summary>
        /// https://github.com/elastic/apm-agent-dotnet/pull/792
        /// </summary>
        static void __add_filter__()
        {
            var extension = new string[]
            {
                ".png",".jpg",".jpeg",".gif",
                ".js",".css",
                ".json",".txt",".exe",
                ".zip",".tar"
            };

            Agent.AddFilter((ITransaction transaction) =>
            {
                //transaction.Context.Request.Url.Protocol = "[HIDDEN]";
                var url = transaction.Context.Request.Url.Full;
                if (url?.Length > 0 && extension.Any(x => url.EndsWith(x, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return null;
                }
                return transaction;
            });
            Agent.AddFilter((ISpan span) =>
            {
                return span;
            });
        }

        public static IApplicationBuilder UseApm(this IApplicationBuilder app, IConfiguration config)
        {
            if (config.IsApmServerConfigAvaliable())
            {
                __add_filter__();
                //app.UseElasticApm(config);
                app.UseAllElasticApm(config);
            }

            return app;
        }

        public static InterceptorCollection AddMethodMetric(this InterceptorCollection interceptor,
            Assembly[] ass,
            Func<MethodInfo, bool> force_include = null,
            Func<MethodInfo, bool> except = null)
        {
            except ??= (x => false);
            force_include ??= (x => false);

            bool __filter__(MethodInfo x)
            {
                if (x.GetCustomAttributes<WCloud.Core.Apm.ApmAttribute>().Any() || force_include.Invoke(x))
                {
                    return true;
                }
                if ((!ass.Contains(x.DeclaringType.Assembly)) || except.Invoke(x))
                {
                    return false;
                }
                return true;
            }

            //这里的x是接口定义不是实现
            interceptor.AddTyped<MethodCallMetricAttribute>(x => __filter__(x));

            return interceptor;
        }
    }
}
