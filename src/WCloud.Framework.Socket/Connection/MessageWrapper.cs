using System;

namespace WCloud.Framework.Socket.Connection
{
    public class MessageWrapper
    {
        public string MessageType { get; set; }
        public string Payload { get; set; }
        public DateTime MessageTimeUtc { get; set; }
    }
}
