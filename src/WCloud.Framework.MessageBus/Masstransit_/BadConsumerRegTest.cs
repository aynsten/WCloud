using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.RabbitMqTransport;

namespace WCloud.Framework.MessageBus.Masstransit_
{
    public class BadConsumerRegTestMessage { }

    public class BadConsumerRegTest : IConsumer<BadConsumerRegTestMessage>
    {
        public async Task Consume(ConsumeContext<BadConsumerRegTestMessage> context)
        {
            Console.WriteLine("bad message");
            await Task.CompletedTask;
        }
    }

    /// <summary>
    /// 死外国佬不相信他代码有bug
    /// https://github.com/MassTransit/MassTransit/issues/1502
    /// </summary>
    public static class BadConsumerRegTestExtension
    {
        [Obsolete("test")]
        public static void TestBadConsumerReg(this IRabbitMqBusFactoryConfigurator config, IRabbitMqHost host)
        {
            config.ReceiveEndpoint("bad_consumer_reg_test", endpoint =>
            {
                Func<IConsumer> factory = () => new BadConsumerRegTest();

                endpoint.Consumer(() => factory.Invoke());
            });
        }

        [Obsolete("test")]
        public static async Task SendBadTestMessage(this IBusControl bus)
        {
            await bus.Publish(new BadConsumerRegTestMessage() { });
            await bus.Send(new BadConsumerRegTestMessage() { });
        }
    }
}
