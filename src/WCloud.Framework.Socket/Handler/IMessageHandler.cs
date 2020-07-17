using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Framework.Socket.Connection;

namespace WCloud.Framework.Socket.Handler
{
    public class ClientMessageContext
    {
        public IServiceProvider HandleServiceProvider { get; set; }
        public WsConnection Connection { get; set; }
        public MessageWrapper Message { get; set; }

        public bool? __broad_cast__ { get; private set; }

        public void BroadCast()
        {
            this.__broad_cast__ = true;
        }

        public string[] __route_to_instances__ { get; private set; }

        /// <summary>
        /// 路由消息到目标服务器
        /// </summary>
        /// <param name="server_instance_arr"></param>
        public void RouteToServerInstances(string[] server_instance_arr)
        {
            this.__route_to_instances__ = server_instance_arr.Distinct().ToArray();
        }
    }

    public class CrossServerMessageContext
    {
        public IServiceProvider HandleServiceProvider { get; set; }
        public IWsServer Server { get; set; }
        public MessageWrapper Message { get; set; }
    }

    public interface IMessageHandler
    {
        string MessageType { get; }
        int Sort { get; }
        Task HandleClientMessage(ClientMessageContext context);
        Task HandleCrossServerMessage(CrossServerMessageContext context);
    }
}
