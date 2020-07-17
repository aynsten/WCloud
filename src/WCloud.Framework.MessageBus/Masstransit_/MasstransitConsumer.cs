using Lib.extension;
using Lib.ioc;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WCloud.Core.MessageBus;

namespace WCloud.Framework.MessageBus.Masstransit_
{
    /// <summary>
    /// masstransit好变态
    /// 建议每个消息对应一个新的consumer
    /// 也可以共享，但是要consumer线程安全
    /// </summary>
    public interface IMyMasstransitConsumer : IConsumer { }

    public interface IMyMasstransitConsumer<T> : IMyMasstransitConsumer, IConsumer<T> where T : class, IMessageBody { }


    /// <summary>
    /// 通过反射创建对象
    /// 或者通过ioc容器拿到对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BasicMasstransitConsumer<T> : IMyMasstransitConsumer<T> where T : class, IMessageBody
    {
        private readonly IServiceProvider provider;
        public BasicMasstransitConsumer(IServiceProvider provider)
        {
            this.provider = provider;
        }

        //public BasicMasstransitConsumer(IServiceProvider provider) { }

        async Task IConsumer<T>.Consume(ConsumeContext<T> context)
        {
            using (var scope = IocContext.Instance.Scope())
            {
                var logger = scope.ServiceProvider.Resolve_<ILogger<BasicMasstransitConsumer<T>>>();

                var all_subscribers = scope.ServiceProvider.ResolveConsumerOptional<T>();
                foreach (var c in all_subscribers)
                {
                    try
                    {
#if DEBUG
                        logger.LogInformation($"消费：{context.Message?.ToJson()}");
#endif

                        var message = new BasicMessageConsumeContext<T>(context.Message);
                        await c.Consume(message);
                    }
                    catch (Exception e)
                    {
                        logger.AddErrorLog(e.Message, e);
                    }
                }
            }
        }
    }
}
