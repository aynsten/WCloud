using Lib.extension;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Framework.Socket.Connection;

namespace WCloud.Framework.Socket.Connection
{
    public class ClientManager : IDisposable
    {
        private readonly ConnectionCollection<WsConnection> connections = new ConnectionCollection<WsConnection>();

        public ConnectionCollection<WsConnection> AllConnections() => this.connections;

        public void AddConnection(WsConnection con)
        {
            this.connections.Add(con);
        }

        public void RemoveConnection(WsConnection con)
        {
            this.connections.Remove(con);
        }

        public void RemoveWhere(Func<WsConnection, bool> where)
        {
            this.connections.RemoveWhere_(where);
        }

        public void RemoveAll()
        {
            this.connections.RemoveAll(x => true);
        }

        public void Dispose()
        {
            var close_tasks = this.connections.Select(x => x.CloseAsync()).ToArray();
            Task.WhenAll(close_tasks).Wait();
            this.RemoveAll();
        }
    }
}
