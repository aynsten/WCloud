using FluentAssertions;
using Lib.extension;
using Lib.ioc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using WCloud.Framework.Socket.Handler;

namespace WCloud.Framework.Socket.Connection
{
    /// <summary>
    /// 需要重写等号运算符
    /// </summary>
    public class WsConnection
    {
        private readonly ILogger logger;
        public WsConnection(IServiceProvider provider, IWsServer server, WsClient client)
        {
            provider.Should().NotBeNull();
            server.Should().NotBeNull();
            client.Should().NotBeNull();

            this.Provider = provider;
            this.Client = client;
            this.Server = server;

            this.logger = provider.Resolve_<ILogger<WsConnection>>();
        }

        public IServiceProvider Provider { get; }
        public IWsServer Server { get; }
        public WsClient Client { get; }

        public async Task SendMessage(MessageWrapper data)
        {
            data.Should().NotBeNull();
            var bs = this.Server.MessageSerializer.SerializeBytes(data);
            await this.Client.SocketChannel.SendAsync(new ArraySegment<byte>(bs), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        async Task __handle_message__(IMessageHandler handler, MessageWrapper data)
        {
            using (var s = this.Provider.CreateScope())
            {
                try
                {
                    var context = new ClientMessageContext()
                    {
                        HandleServiceProvider = s.ServiceProvider,
                        Connection = this,
                        Message = data
                    };
                    await handler.HandleClientMessage(context);

                    //如果需要把信息路由到其他服务器
                    var server_instance_id = context.__route_to_instances__;
                    if (server_instance_id != null && server_instance_id.Any())
                    {
                        if (server_instance_id.Contains(context.Connection.Server.ServerInstanceID))
                        {
                            //await con.SendMessage(message);
                        }

                        var tasks = server_instance_id
                            .Where(x => x != context.Connection.Server.ServerInstanceID || true)
                            .Select(x => context.Connection.Server.TransportProvider.RouteToServerInstance(x, context.Message)).ToArray();
                        if (tasks.Any())
                        {
                            await Task.WhenAll(tasks);
                        }
                    }
                    if (context.__broad_cast__ != null && context.__broad_cast__.Value)
                    {
                        await context.Connection.Server.TransportProvider.BroadCast(context.Message);
                    }
                }
                catch (Exception e)
                {
                    this.logger.AddErrorLog(e.Message, e);
                    await this.SendMessage(new MessageWrapper() { MessageType = "exception", Payload = e.Message });
                }
            }
        }

        async Task __on_message__(byte[] bs)
        {
            try
            {
                var data = this.Server.MessageSerializer.DeserializeOrDefault<MessageWrapper>(bs);

                if (data != null)
                {
                    var handler = this.Server.GetHandlerOrNothing(data.MessageType);
                    if (handler != null)
                    {
                        await this.__handle_message__(handler, data);
                    }
                    else
                    {
                        this.logger.LogInformation("no handler for this message type");
                        await this.SendMessage(new MessageWrapper() { MessageType = "no handler for this message type" });
                    }
                }
                else
                {
                    this.logger.LogInformation("message format is not supported");
                    await this.SendMessage(new MessageWrapper() { MessageType = "message format is not supported" });
                }
            }
            catch (Exception e)
            {
                this.logger.AddErrorLog(e.Message, e);
            }
        }

        public async Task CloseAsync(CancellationToken? token = null)
        {
            token ??= CancellationToken.None;
            await this.Client.SocketChannel.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, token.Value);
        }

        public async Task StartReceiveMessageLoopAsync(CancellationToken? token = null)
        {
            token ??= CancellationToken.None;

            var msg = new MessageWrapper() { MessageType = "ping" };
            await this.__on_message__(this.Server.MessageSerializer.SerializeBytes(msg));

            this.Server.ClientManager.AddConnection(this);
            try
            {
                await this.Client.SocketChannel.StartReceiveLoopAsync((res, bs) => this.__on_message__(bs));
            }
            catch (Exception e)
            {
                this.logger.AddErrorLog("接收数据失败", e);
            }

            this.Server.ClientManager.RemoveConnection(this);
            await this.Server.RegistrationProvider.RemoveRegisterInfo(this.Client.SubjectID, this.Client.DeviceType);
        }
    }
}
