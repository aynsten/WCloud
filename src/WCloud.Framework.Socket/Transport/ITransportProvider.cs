using System;
using System.Threading.Tasks;
using WCloud.Framework.Socket.Connection;

namespace WCloud.Framework.Socket.Transport
{
    public interface ITransportProvider : IDisposable
    {
        Task RouteToServerInstance(string instance_key, MessageWrapper data);
        Task BroadCast(MessageWrapper data);
        Task SubscribeMessageEndpoint(string key, Func<MessageWrapper, Task> callback);
        Task SubscribeBroadcastMessageEndpoint(Func<MessageWrapper, Task> callback);
    }
}
