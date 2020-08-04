using Lib.ioc;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using WCloud.Framework.Socket.Connection;
using WCloud.Framework.Socket.RegistrationCenter;
using WCloud.Framework.Socket.Connection;

namespace WCloud.Framework.Socket
{
    public static class SocketExtension
    {
        public static async Task StartReceiveLoopAsync(this WebSocket ws,
            Func<WebSocketReceiveResult, byte[], Task> on_message,
            CancellationToken? token = null, CancellationToken? close_token = null,
            int? buffer_size = null)
        {
            token ??= CancellationToken.None;
            close_token ??= CancellationToken.None;
            buffer_size ??= 1024 * 4;

            var buffer = new byte[buffer_size.Value];

            WebSocketReceiveResult result = null;
            while (true)
            {
                result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), token.Value);
                if (result.CloseStatus.HasValue)
                    break;
                if (result.Count <= 0)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                }

                var bs = buffer.Take(result.Count).ToArray();

                await on_message.Invoke(result, bs);
            }

            await ws.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, close_token.Value);
        }

        public static UserRegistrationInfo ToRegInfo(this WsConnection con)
        {
            var res = new UserRegistrationInfo()
            {
                UserID = con.Client.SubjectID,
                DeviceType = con.Client.DeviceType,
                Payload = new UserRegistrationInfoPayload()
                {
                    ServerInstanceID = con.Server.ServerInstanceID,
                    PingTimeUtc = DateTime.UtcNow
                }
            };
            return res;
        }
    }
}
