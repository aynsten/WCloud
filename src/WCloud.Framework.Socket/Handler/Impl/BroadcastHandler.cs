using System.Linq;
using System.Threading.Tasks;

namespace WCloud.Framework.Socket.Handler
{
    public class BroadcastHandler : IMessageHandler
    {
        public string MessageType => "broadcast";

        public int Sort => 1;

        public async Task HandleCrossServerMessage(CrossServerMessageContext context)
        {
            var tasks = context.Server.ClientManager.AllConnections().Select(x => x.SendMessage(context.Message)).ToArray();
            await Task.WhenAll(tasks);
        }

        public async Task HandleClientMessage(ClientMessageContext context)
        {
            context.BroadCast();
            await Task.CompletedTask;
        }
    }
}
