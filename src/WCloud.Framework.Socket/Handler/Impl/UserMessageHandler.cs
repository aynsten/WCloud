using System;
using System.Linq;
using System.Threading.Tasks;

namespace WCloud.Framework.Socket.Handler
{
    public class UserToUserPayload
    {
        public string Sender { get; set; }
        public string Reciever { get; set; }
        public string Message { get; set; }
        public DateTime SendTimeUtc { get; set; }
    }

    public class UserMessageHandler : IMessageHandler
    {
        public string MessageType => "user_to_user";

        public int Sort => 1;

        public async Task HandleCrossServerMessage(CrossServerMessageContext context)
        {
            var payload = context.Server.MessageSerializer.Deserialize<UserToUserPayload>(context.Message.Payload);
            var tasks = context.Server.ClientManager.AllConnections()
                .Where(x => x.Client.SubjectID == payload.Reciever)
                .Select(x => x.SendMessage(context.Message)).ToArray();
            if (tasks.Any())
            {
                await Task.WhenAll(tasks);
            }
            else
            {
                //消息路由过来但是本地没有对应的连接
            }
        }

        public async Task HandleClientMessage(ClientMessageContext context)
        {
            var payload = context.Connection.Server.MessageSerializer.Deserialize<UserToUserPayload>(context.Message.Payload);
            var server_instance_id = await context.Connection.Server.RegistrationProvider.GetUserServerInstances(payload.Reciever);

            context.RouteToServerInstances(server_instance_id);
        }
    }
}
