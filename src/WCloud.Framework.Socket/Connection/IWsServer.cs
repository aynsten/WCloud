using System;
using System.Threading.Tasks;
using WCloud.Framework.Socket.Handler;
using WCloud.Framework.Socket.Persistence;
using WCloud.Framework.Socket.RegistrationCenter;
using WCloud.Framework.Socket.Transport;

namespace WCloud.Framework.Socket.Connection
{
    public interface IWsServer : IDisposable
    {
        ClientManager ClientManager { get; }

        /// <summary>
        /// 用于路由
        /// </summary>
        string ServerInstanceID { get; }

        IMessageSerializer MessageSerializer { get; }
        IRegistrationProvider RegistrationProvider { get; }
        IPersistenceProvider PersistenceProvider { get; }
        ITransportProvider TransportProvider { get; }
        IMessageHandler[] MessageHandlers { get; }

        IMessageHandler GetHandlerOrNothing(string type);
        Task Cleanup();
        Task Start();
    }
}
