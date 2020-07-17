using Lib.ioc;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace WCloud.Framework.MessageBus.Masstransit_
{
    public class MasstransitConsumerStartor : IConsumerStartor
    {
        private readonly IServiceProvider provider;

        public MasstransitConsumerStartor(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public void StartComsume()
        {
            var bus = provider.Resolve_<IBusControl>();

            async Task Start()
            {
                await bus.StartAsync();
                await bus.Publish(new MyMessage { Value = "publish:消息总线启动成功" });
                //await bus.Send(new MyMessage { Value = "send:消息总线启动成功" });

                //await bus.SendBadTestMessage();
            }

            Task.Run(async () => await Start()).Wait();
        }
    }
}
