using Lib.extension;
using Lib.helper;
using Lib.ioc;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WCloud.Framework.Jobs.Quartz_
{
    public static class QuartzBootstrap
    {
        public static IServiceCollection AddQuartz(this IServiceCollection collection,
            NameValueCollection props = null)
        {
            collection.AddDisposableSingleInstanceService(new QuartzSchedulerWrapper(props));
            return collection;
        }

        public static IScheduler QuartzScheduler(this IServiceProvider provider) =>
            provider.Resolve_<QuartzSchedulerWrapper>().Scheduler;

        public static IServiceProvider StartJobs(this IServiceProvider provider, Assembly[] ass)
        {
            if (ValidateHelper.IsEmpty(ass))
                throw new ArgumentNullException(nameof(ass));

            var con = provider.QuartzScheduler();
            if (con.IsStarted)
                throw new Exception("调度器已经启动，无法添加任务！");
            var jobs = ass.FindAllJobsAndCreateInstance_();
            if (!jobs.Any())
                throw new ArgumentException("在指定程序集中未找到任何job");

            Task.Run(async () =>
            {
                foreach (var job in jobs)
                    await con.AddJob_(job);

                await con.StartIfNotStarted_();
            }).Wait();

            return provider;
        }
    }
}
