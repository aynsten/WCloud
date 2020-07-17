using FluentAssertions;
using Lib.extension;
using Lib.helper;
using Quartz;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WCloud.Framework.Jobs.Quartz_
{
    /// <summary>
    /// quartz扩展
    /// 所谓分布式部署就是把task类和参数序列化后持久到数据库中。
    /// 其实感觉不算是分布式，只是对运行的任务和计划添加了“断点续传”功能。
    /// 创建任务的proj和scheduler proj必须在一个项目中，
    /// 否则scheduler从db中读取的job可能会加载失败（load class）
    /// </summary>
    public static class QuartzExtension
    {
        public static TriggerBuilder Start_(this TriggerBuilder builder, DateTimeOffset? when = null)
        {
            var res = when == null ?
            builder.StartNow() :
            builder.StartAt(when.Value);

            return res;
        }

        /// <summary>
        /// 获取task信息
        /// </summary>
        /// <returns></returns>
        public static async Task<List<JobModel>> GetAllTasks_(this IScheduler manager)
        {
            //所有任务
            var jobKeys = await manager.GetAllJobKeys_();
            //正在运行的任务
            var runningJobs = await manager.GetCurrentlyExecutingJobs();

            var list = new List<JobModel>();
            foreach (var jobKey in jobKeys)
            {
                var job = new JobModel()
                {
                    JobName = jobKey.Name,
                    JobGroup = jobKey.Group,
                    IsRunning = runningJobs.Any(x => x.JobDetail.Key == jobKey),
                    Triggers = new List<JobTriggerModel>(),
                };

                var triggers = await manager.GetTriggersOfJob(jobKey);
                foreach (var x in triggers)
                {
                    var trigger = new JobTriggerModel()
                    {
                        TriggerName = x.Key.Name,
                        TriggerGroup = x.Key.Group,
                        StartTime = x.StartTimeUtc.LocalDateTime,
                        PreTriggerTime = x.GetPreviousFireTimeUtc()?.LocalDateTime,
                        NextTriggerTime = x.GetNextFireTimeUtc()?.LocalDateTime,
                    };
                    trigger.JobStatus = (await manager.GetTriggerState(x.Key)).GetTriggerState();
                    job.Triggers.Add(trigger);
                }

                list.Add(job);
            }
            return list;
        }

        public static async Task<IEnumerable<JobKey>> GetAllJobKeys_(this IScheduler manager)
        {
            var res = await manager.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
            return res.AsEnumerable();
        }

        public static async Task AddJob_(this IScheduler manager, QuartzJobBase job)
        {
            await manager.AddJob_(job.GetType(), job.CachedTrigger, job.Name, job.Group);
        }

        public static async Task AddJob_(this IScheduler manager, Type job_type, ITrigger trigger, string name, string group = null)
        {
            job_type.Should().NotBeNull();
            trigger.Should().NotBeNull();
            name.Should().NotBeNullOrEmpty();

            var builder = JobBuilder.Create(job_type);

            builder = builder.WithIdentity(name, group ?? "default");

            var job = builder.Build();

            //job key的特征其实就是name和group
            //new JobKey(name: "", group: "");

            var exist_jobs = await manager.GetAllJobKeys_();

            if (!exist_jobs.Contains(job.Key))
            {
                await manager.ScheduleJob(job, trigger);
            }
        }

        public static async Task StartIfNotStarted_(this IScheduler manager, TimeSpan? delay = null)
        {
            if (!manager.IsStarted)
            {
                if (delay == null)
                {
                    await manager.Start();
                }
                else
                {
                    await manager.StartDelayed(delay.Value);
                }
            }
        }

        /// <summary>
        /// 如果任务开启就关闭
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="waitForJobsToComplete"></param>
        public static async Task ShutdownIfStarted_(this IScheduler manager, bool waitForJobsToComplete = false)
        {
            if (manager.IsStarted)
            {
                await manager.Shutdown(waitForJobsToComplete: waitForJobsToComplete);
            }
        }

        /// <summary>
        /// 找到任务
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Type[] FindJobTypes_(this Assembly a)
        {
            var res = a.GetTypes().Where(x => x.IsNormalClass() && x.IsAssignableTo_<QuartzJobBase>()).ToArray();
            return res;
        }

        public static List<QuartzJobBase> FindAllJobsAndCreateInstance_(this IEnumerable<Assembly> ass)
        {
            if (ass.Select(x => x.FullName).Distinct().Count() != ass.Count())
                throw new Exception("无法启动任务：传入重复的程序集");

            return ass.SelectMany(x =>
            x.FindJobTypes_().Select(m => (QuartzJobBase)Activator.CreateInstance(m))).ToList();
        }

        public static TriggerBuilder WithIdentity_(this TriggerBuilder builder, string name, string group = null)
        {
            var res = ValidateHelper.IsNotEmpty(group) ?
                builder.WithIdentity(name, group) :
                builder.WithIdentity(name);
            return res;
        }

        /// <summary>
        /// 获取状态的描述
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static string GetTriggerState(this TriggerState state)
        {
            switch (state)
            {
                case TriggerState.Blocked:
                    return "阻塞";
                case TriggerState.Complete:
                    return "完成";
                case TriggerState.Error:
                    return "错误";
                case TriggerState.None:
                    return "无状态";
                case TriggerState.Normal:
                    return "正常";
                case TriggerState.Paused:
                    return "暂停";
                default:
                    return state.ToString();
            }
        }
    }
}
