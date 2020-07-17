using System;
using System.Threading.Tasks;
using WCloud.Framework.Jobs;
using WCloud.Framework.Socket.Connection;
using WCloud.Framework.Socket.UserContext;

namespace WCloud.IM.Server.Jobs
{
    [JobConfiguration(CronExpression = "5 * * * *")]
    public class ServerCleanupJob : IMyHangfireJob
    {
        private readonly IWsServer wsServer;
        private readonly IUserGroups userGroups;
        private readonly IServiceProvider provider;
        public ServerCleanupJob(IWsServer wsServer, IUserGroups userGroups, IServiceProvider provider)
        {
            this.wsServer = wsServer;
            this.userGroups = userGroups;
            this.provider = provider;
        }

        public async Task ExecuteAsync()
        {
            await this.wsServer.Cleanup();
        }
    }
}
