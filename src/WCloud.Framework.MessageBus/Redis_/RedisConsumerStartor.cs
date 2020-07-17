using System;
using Lib.extension;
using Lib.ioc;
using Microsoft.Extensions.Logging;

namespace WCloud.Framework.MessageBus.Redis_
{
    public class RedisConsumerStartor : IConsumerStartor
    {
        private readonly IServiceProvider provider;

        public RedisConsumerStartor(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public void StartComsume()
        {
            var consumers = this.provider.ResolveAll_<IRedisConsumer>();

            foreach (var m in consumers)
            {
                try
                {
                    m.StartConsume();
                }
                catch (Exception e)
                {
                    this.provider.Resolve_<ILogger<RedisConsumerStartor>>().AddErrorLog("start redis consumer", e);
                }
            }

        }
    }
}
