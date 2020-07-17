using Lib.extension;
using Lib.ioc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WCloud.Framework.Jobs.Quartz_;

namespace WCloud.Framework.Jobs
{
    public class CleanEsJob : QuartzJobBase
    {
        public override string Name => "清理es";

        public override bool AutoStart => true;

        public override Quartz.ITrigger Trigger => this.TriggerInterval(TimeSpan.FromSeconds(10), name: "trigger_per_10s");

        public override async Task Execute(Quartz.IJobExecutionContext context)
        {
            using (var s = IocContext.Instance.Scope())
            {
                try
                {
                    await Task.CompletedTask;
                }
                catch (Exception e)
                {
                    var logger = s.ServiceProvider.Resolve_<ILogger<CleanEsJob>>();
                    logger.AddErrorLog(e.Message, e);
                }
            }
        }
    }
}
