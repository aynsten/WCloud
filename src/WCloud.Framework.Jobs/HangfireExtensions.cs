using FluentAssertions;
using Hangfire;
using Lib.extension;
using Lib.ioc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace WCloud.Framework.Jobs
{
    public static class HangfireExtensions
    {
        static Type[] __jobs__(Assembly[] ass)
        {
            var tps = ass.GetAllTypes()
                .Where(x => x.IsNormalClass())
                .Where(x => x.IsAssignableTo_<IMyHangfireJob>())
                .ToArray();
            return tps;
        }

        static readonly MethodInfo execute_method = typeof(IMyHangfireJob).GetMethods().FirstOrDefault();

        static readonly MethodInfo add_or_update = typeof(RecurringJob).GetMethods()
            .Where(x => x.IsGenericMethod)
            .FirstOrDefault();

        [Obsolete]
        static void __add_to_recurring_job_list__(Type[] @interfaces)
        {
            foreach (var t in @interfaces)
            {
                var param = Expression.Parameter(t, "x");
                var call = Expression.Call(param, execute_method);
                //构造 x=>x.dosomething();
                var exp = Expression.Lambda(typeof(Func<,>).MakeGenericType(t, typeof(Task)), call, param);

                add_or_update.MakeGenericMethod(t).Invoke(null, new object[] { exp });
            }
        }

        class JobsContainer : ReadOnlyCollection<Type>
        {
            public JobsContainer(IList<Type> list) : base(list) { }
        }

        public static IServiceCollection AddHangfireJobs_(this IServiceCollection services, Assembly[] ass)
        {
            ass.Should().NotBeNullOrEmpty();
            var jobs = __jobs__(ass);

            services.AddSingleton(new JobsContainer(jobs));

            foreach (var m in jobs)
            {
                services.AddSingleton(m);
                foreach (var i in m.GetInterfaces())
                {
                    services.AddSingleton(i, m);
                }
            }

            return services;
        }

        static void __add_recurring_job__<T>(IRecurringJobManager manager) where T : IMyHangfireJob
        {
            var t = typeof(T);
            t.IsClass.Should().BeTrue();

            var job_config = t.GetCustomAttribute<JobConfigurationAttribute>();
            job_config.Should().NotBeNull();
            job_config.ValidateOrThrow();

            var job_id = job_config.JobId ?? $"{t.Namespace}.{t.Name}";

            manager.AddOrUpdate<T>(job_id, x => x.ExecuteAsync(), () => job_config.CronExpression);
        }

        public static IApplicationBuilder StartHangfireJobs_(this IApplicationBuilder builder)
        {
            var ms = typeof(HangfireExtensions).GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
            var reg_func = ms.FirstOrDefault(x => x.Name == nameof(__add_recurring_job__));
            reg_func.Should().NotBeNull();

            using var s = builder.ApplicationServices.CreateScope();

            var jobs = s.ServiceProvider.Resolve_<JobsContainer>();
            var job_manager = s.ServiceProvider.Resolve_<IRecurringJobManager>();

            foreach (var m in jobs)
            {
                var f = reg_func.MakeGenericMethod(m);
                f.Invoke(obj: null, parameters: new object[] { job_manager });
            }

            return builder;
        }
    }
}
