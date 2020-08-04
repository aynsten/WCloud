using Lib.extension;
using Lib.ioc;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace WCloud.Framework.MessageBus.Redis_
{
    public class RedisConsumerStartor : IConsumerStartor
    {
        private readonly IServiceProvider provider;

        public RedisConsumerStartor(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public void Dispose()
        {
            using var s = this.provider.CreateScope();
            var logger = s.ServiceProvider.ResolveLogger<RedisConsumerStartor>();

            var consumers = s.ServiceProvider.ResolveAll_<IRedisConsumer>();

            foreach (var m in consumers)
            {
                try
                {
                    m.Dispose();
                }
                catch (Exception e)
                {
                    logger.AddErrorLog("stop redis consumer", e);
                }
            }
        }

        public void StartComsume()
        {
            using var s = this.provider.CreateScope();
            var logger = s.ServiceProvider.ResolveLogger<RedisConsumerStartor>();

            var consumers = s.ServiceProvider.ResolveAll_<IRedisConsumer>();

            foreach (var m in consumers)
            {
                try
                {
                    m.StartConsume();
                }
                catch (Exception e)
                {
                    logger.AddErrorLog("start redis consumer", e);
                }
            }
        }
    }
}
