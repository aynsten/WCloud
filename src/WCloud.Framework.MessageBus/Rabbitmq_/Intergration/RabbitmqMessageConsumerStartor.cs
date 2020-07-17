using Lib.extension;
using Lib.ioc;
using Microsoft.Extensions.Logging;
using System;

namespace WCloud.Framework.MessageBus.Rabbitmq_.Intergration
{
    public class RabbitmqMessageConsumerStartor : IConsumerStartor
    {
        private readonly IServiceProvider provider;

        public RabbitmqMessageConsumerStartor(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public void StartComsume()
        {
            var consumers = this.provider.ResolveAll_<IRabbitMqConsumer>();

            foreach (var m in consumers)
            {
                try
                {
                    m.StartConsume();
                }
                catch (Exception e)
                {
                    this.provider.Resolve_<ILogger<RabbitmqMessageConsumerStartor>>().AddErrorLog("start rabbitmq consumer", e);
                }
            }
        }
    }
}
