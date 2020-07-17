using System;
using System.Threading.Tasks;

namespace WCloud.Framework.Socket.Handler
{
    public class EchoHandler : IMessageHandler
    {
        public string MessageType => "echo";

        public int Sort => 1;

        public Task HandleCrossServerMessage(CrossServerMessageContext context)
        {
            throw new NotImplementedException();
        }

        public async Task HandleClientMessage(ClientMessageContext context)
        {
            await context.Connection.SendMessage(context.Message);
        }
    }
}
