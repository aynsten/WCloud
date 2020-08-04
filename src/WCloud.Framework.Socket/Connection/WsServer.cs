using FluentAssertions;
using Lib.ioc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Framework.Socket.Handler;
using WCloud.Framework.Socket.Persistence;
using WCloud.Framework.Socket.RegistrationCenter;
using WCloud.Framework.Socket.Transport;

namespace WCloud.Framework.Socket.Connection
{
    public class WsServer : IWsServer
    {
        public ClientManager ClientManager { get; }

        /// <summary>
        /// 用于路由
        /// </summary>
        public string ServerInstanceID { get; }

        public IServiceProvider ServiceProvider { get; }
        public IMessageSerializer MessageSerializer { get; }
        public IRegistrationProvider RegistrationProvider { get; }
        public IPersistenceProvider PersistenceProvider { get; }
        public ITransportProvider TransportProvider { get; }
        public IMessageHandler[] MessageHandlers { get; }

        public WsServer(IServiceProvider provider, string server_instance_id)
        {
            server_instance_id.Should().NotBeNullOrEmpty();

            this.ClientManager = new ClientManager();
            this.ServerInstanceID = server_instance_id;

            this.ServiceProvider = provider;
            this.MessageSerializer = provider.Resolve_<IMessageSerializer>();
            this.RegistrationProvider = provider.Resolve_<IRegistrationProvider>();
            this.PersistenceProvider = provider.Resolve_<IPersistenceProvider>();
            this.TransportProvider = provider.Resolve_<ITransportProvider>();
            this.MessageHandlers = provider.ResolveAll_<IMessageHandler>();
        }

        async Task __message_from_transport__(MessageWrapper message)
        {
            var handler = this.GetHandlerOrNothing(message.MessageType);
            if (handler != null)
            {
                using (var s = this.ServiceProvider.CreateScope())
                {
                    var context = new CrossServerMessageContext()
                    {
                        HandleServiceProvider = s.ServiceProvider,
                        Server = this,
                        Message = message
                    };
                    await handler.HandleCrossServerMessage(context);
                }
            }
        }

        public async Task Start()
        {
            //路由到这台服务器的消息
            var queue_key = this.ServerInstanceID;
            await this.TransportProvider.SubscribeMessageEndpoint(queue_key, this.__message_from_transport__);
            //路由到所有服务器的消息
            await this.TransportProvider.SubscribeBroadcastMessageEndpoint(this.__message_from_transport__);
        }

        public void Dispose()
        {
            Task.Run(this.Cleanup).Wait();
            this.ClientManager.Dispose();
            this.TransportProvider.Dispose();
        }

        public IMessageHandler GetHandlerOrNothing(string type)
        {
            var handler = this.MessageHandlers
                .OrderByDescending(x => x.Sort)
                .FirstOrDefault(x => x.MessageType == type);
            return handler;
        }

        public async Task Cleanup()
        {
            //
        }
    }
}
