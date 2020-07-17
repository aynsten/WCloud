using Lib.extension;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Framework.Jobs;
using WCloud.Framework.Socket.Connection;
using WCloud.Framework.Socket.RegistrationCenter;
using WCloud.Framework.Socket.UserContext;

namespace WCloud.IM.Server.Jobs
{
    [JobConfiguration(CronExpression = "*/3 * * * *", JobId = "server-ping")]
    public class ServerPingJob : IMyHangfireJob
    {
        private readonly IWsServer wsServer;
        private readonly IUserGroups userGroups;
        private readonly IServiceProvider provider;
        private readonly IRegistrationProvider registrationProvider;
        public ServerPingJob(IWsServer wsServer, IUserGroups userGroups, IServiceProvider provider, IRegistrationProvider registrationProvider)
        {
            this.wsServer = wsServer;
            this.userGroups = userGroups;
            this.provider = provider;
            this.registrationProvider = registrationProvider;
        }

        public async Task ExecuteAsync()
        {
            using var s = this.provider.CreateScope();

            var user_uids = this.wsServer.ClientManager.AllConnections().Select(x => x.Client.SubjectID).WhereNotEmpty().ToArray();

            var group_uids = await userGroups.GetUsersGroups(user_uids);
            group_uids = group_uids.Distinct().ToArray();

            foreach (var g in group_uids)
            {
                var info = new GroupRegistrationInfo()
                {
                    GroupUID = g,
                    ServerInstance = this.wsServer.ServerInstanceID,
                    Payload = new GroupRegistrationInfoPayload() { }
                };
                await this.registrationProvider.RegisterGroupInfo(info);
            }
        }
    }
}
