using FluentAssertions;
using Lib.extension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WCloud.Admin.Message;
using WCloud.Framework.MessageBus;
using WCloud.MetroAd;
using WCloud.MetroAd.Event;

namespace WCloud.Admin.MessageConsumers
{
    public class OperationLogConsumer : IMessageConsumer<OperationLogMessage>, Lib.core.IFinder
    {
        private readonly IServiceProvider provider;
        public OperationLogConsumer(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public async Task Consume(IMessageConsumeContext<OperationLogMessage> context)
        {
            try
            {
                context.Message.UID.Should().NotBeNullOrEmpty("请初始化操作日志");

                var service = provider.Resolve_<IMetroAdRepository<OperationLogEntity>>();
                await service.AddAsync(context.Message);
            }
            catch (Exception e)
            {
                provider.Resolve_<ILogger<OperationLogConsumer>>().AddErrorLog(nameof(OperationLogConsumer), e);
            }
        }
    }
}
