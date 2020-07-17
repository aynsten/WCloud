using System;
using System.Net.WebSockets;

namespace WCloud.Framework.Socket.Connection
{
    public class WsClient
    {
        public WebSocket SocketChannel { get; }
        public WsClient(WebSocket ws)
        {
            this.SocketChannel = ws;
        }
        public string SubjectID { get; set; }
        public string DeviceType { get; set; }
        public string ConnectionID { get; set; }

        public DateTime PingTimeUtc { get; set; }
    }
}
