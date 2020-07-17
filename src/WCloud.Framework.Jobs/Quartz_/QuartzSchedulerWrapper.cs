using Lib.ioc;
using Quartz;
using Quartz.Impl;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace WCloud.Framework.Jobs.Quartz_
{
    public class QuartzSchedulerWrapper : ISingleInstanceService
    {
        private readonly NameValueCollection _props;
        private readonly ISchedulerFactory _factory;
        private readonly IScheduler _scheduler;

        public QuartzSchedulerWrapper(NameValueCollection props)
        {
            this._props = props;
            this._factory = this._props?.Count > 0 ? new StdSchedulerFactory(this._props) : new StdSchedulerFactory();
            this._scheduler = Task.Run(() => this._factory.GetScheduler()).Result;
        }

        public NameValueCollection Props => this._props;
        public ISchedulerFactory Factory => this._factory;
        public IScheduler Scheduler => this._scheduler;

        public int DisposeOrder => int.MinValue;

        public void Dispose()
        {
            this._scheduler?.ShutdownIfStarted_();
        }
    }
}
