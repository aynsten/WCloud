using System;
using System.Threading.Tasks;
using WCloud.Framework.Socket.Connection;

namespace WCloud.Framework.Socket.Handler.Impl
{
    public class PingHandler : IMessageHandler
    {
        public string MessageType => "ping";

        public int Sort => 1;

        public async Task HandleClientMessage(ClientMessageContext context)
        {
            var info = context.Connection.ToRegInfo();
            await context.Connection.Server.RegistrationProvider.RegisterUserInfo(info);

            context.Connection.Client.PingTimeUtc = info.Payload.PingTimeUtc;

            await context.Connection.SendMessage(new MessageWrapper() { MessageType = "ping_success" });
        }

        public Task HandleCrossServerMessage(CrossServerMessageContext context)
        {
            throw new NotImplementedException();
        }
    }
}
