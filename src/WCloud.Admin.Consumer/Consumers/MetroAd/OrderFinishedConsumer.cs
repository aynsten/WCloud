using Lib.extension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WCloud.Framework.MessageBus;
using WCloud.MetroAd.Shared.Message;

namespace WCloud.Admin.MessageConsumers
{
    public class OrderFinishedConsumer : IMessageConsumer<OrderFinishedMessage>, Lib.core.IFinder
    {
        private readonly IServiceProvider provider;
        public OrderFinishedConsumer(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public async Task Consume(IMessageConsumeContext<OrderFinishedMessage> context)
        {
            try
            {
                var order_uid = context.Message.OrderUID;

                //send mail

                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                provider.Resolve_<ILogger<OrderFinishedConsumer>>().AddErrorLog(nameof(OrderFinishedConsumer), e);
            }
        }
    }
}
