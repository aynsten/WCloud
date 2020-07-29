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
            this.provider.StartConcume();
        }
    }
}
