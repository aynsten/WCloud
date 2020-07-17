using Lib.helper;
using Quartz;
using System;
using System.Threading.Tasks;

namespace WCloud.Framework.Jobs.Quartz_
{
    public abstract class QuartzJobBase : IJob
    {
        /// <summary>
        /// 任务名，不能重复
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 分组，重写来修改
        /// </summary>
        public virtual string Group { get => "default_group"; }

        /// <summary>
        /// 是否自动启动
        /// </summary>
        public abstract bool AutoStart { get; }

        /// <summary>
        /// 调度规则，多次调用将多次创建，多次调用请使用CachedTrigger
        /// </summary>
        public abstract ITrigger Trigger { get; }

        private readonly object trigger_lock = new object();
        private ITrigger _trigger;

        /// <summary>
        /// 触发器只创建一次
        /// </summary>
        public virtual ITrigger CachedTrigger
        {
            get
            {
                if (this._trigger == null)
                {
                    lock (this.trigger_lock)
                    {
                        if (this._trigger == null)
                        {
                            this._trigger = this.Trigger ?? throw new ArgumentNullException("job触发器不能为空");
                        }
                    }
                }
                return this._trigger;
            }
        }


        /// <summary>
        /// 任务的具体实现
        /// </summary>
        /// <param name="context"></param>
        public abstract Task Execute(IJobExecutionContext context);

        string TriggerName(string name) => name ?? Com.GetUUID();

        /// <summary>
        /// Cron表达式
        /// </summary>
        /// <param name="cron"></param>
        /// <returns></returns>
        protected ITrigger TriggerWithCron(string cron, string name = null) =>
            BuildTrigger(t => t.TriggerWithCron_(cron, this.TriggerName(name)).Build());

        /// <summary>
        /// 每天固定时间
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <returns></returns>
        protected ITrigger TriggerDaily(int hour, int minute, string name = null) =>
            BuildTrigger(t => t.TriggerDaily_(hour, minute, this.TriggerName(name)).Build());

        /// <summary>
        /// 每月的某一时间
        /// </summary>
        /// <param name="dayOfMonth"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <returns></returns>
        protected ITrigger TriggerMonthly(int dayOfMonth, int hour, int minute, string name = null) =>
            BuildTrigger(t => t.TriggerMonthly_(dayOfMonth, hour, minute, this.TriggerName(name)).Build());

        /// <summary>
        /// 每周某一天执行
        /// </summary>
        /// <param name="dayOfWeek"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <returns></returns>
        protected ITrigger TriggerWeekly(DayOfWeek dayOfWeek, int hour, int minute, string name = null) =>
            BuildTrigger(t => t.TriggerWeekly_(dayOfWeek, hour, minute, this.TriggerName(name)).Build());

        /// <summary>
        /// 隔几秒
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        protected ITrigger TriggerInterval(TimeSpan interval,
            int? repeat_count = null, DateTimeOffset? start = null, string name = null) =>
            BuildTrigger(t => t.TriggerInterval_(interval,
                repeat_count: repeat_count,
                start: start,
                name: this.TriggerName(name)).Build());

        /// <summary>
        /// 创建trigger
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        protected ITrigger BuildTrigger(Func<TriggerBuilder, ITrigger> func) => func(TriggerBuilder.Create());

    }

    public abstract class QuartzJobBase_ : QuartzJobBase
    {
        public override async Task Execute(IJobExecutionContext context)
        {
            this.ExecuteJob(context);
            await Task.FromResult(1);
        }

        public abstract void ExecuteJob(IJobExecutionContext context);
    }
}