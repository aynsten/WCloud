using System;
using System.Collections.Generic;
using System.Text;

namespace WCloud.Framework.MessageBus.Config
{
    public class RabbitMQ
    {
        public string ServerAndPort { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}
