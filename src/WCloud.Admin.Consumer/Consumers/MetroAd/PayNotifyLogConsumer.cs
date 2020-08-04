using FluentAssertions;
using Lib.extension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WCloud.Framework.MessageBus;
using WCloud.MetroAd;
using WCloud.MetroAd.Message;
using WCloud.MetroAd.Order;

namespace WCloud.Admin.Consumer.Consumers
{
    /// <summary>
    /// 支付回调通知
    /// </summary>
    public class PayNotifyLogConsumer : IMessageConsumer<PayNotifyLogMessage>, Lib.core.IFinder
    {
        private readonly IServiceProvider provider;
        public PayNotifyLogConsumer(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public async Task Consume(IMessageConsumeContext<PayNotifyLogMessage> context)
        {
            try
            {
                context.Message.UID.Should().NotBeNullOrEmpty("回调数据清闲init");

                var service = provider.Resolve_<IMetroAdRepository<PaymentNotificationEntity>>();

                await service.AddAsync(context.Message);
            }
            catch (Exception e)
            {
                provider.Resolve_<ILogger<PayNotifyLogConsumer>>().AddErrorLog(nameof(PayNotifyLogConsumer), e);
            }
        }
    }
}
