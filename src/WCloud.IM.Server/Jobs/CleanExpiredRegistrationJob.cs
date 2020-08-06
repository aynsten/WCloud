using System.Threading.Tasks;
using WCloud.Framework.Jobs;

namespace WCloud.IM.Server.Jobs
{
    [JobConfiguration(CronExpression = "0 0 * * *")]
    public class CleanExpiredRegistrationJob : IMyHangfireJob
    {
        public async Task ExecuteAsync()
        {
            //
        }
    }
}
