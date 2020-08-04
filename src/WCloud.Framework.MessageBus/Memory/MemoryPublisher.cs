using System;
using System.Threading;
using System.Threading.Tasks;
using Lib.extension;
using Microsoft.Extensions.Logging;
using WCloud.Core.MessageBus;

namespace WCloud.Framework.MessageBus.Memory
{
    public class MemoryPublisher : IMessagePublisher
    {
        private readonly ILogger logger;
        private readonly IServiceProvider provider;
        public MemoryPublisher(ILogger<MemoryPublisher> logger,IServiceProvider provider)
        {
            this.logger = logger;
            this.provider = provider;
        }

        public async Task PublishAsync<T>(T model, CancellationToken cancellationToken) where T : class, IMessageBody
        {
            await this.PublishAsync(string.Empty, model, cancellationToken);
        }

        public async Task PublishAsync<T>(string key, T model, CancellationToken cancellationToken) where T : class, IMessageBody
        {
            var consumers = this.provider.ResolveConsumerOptional<T>();
            foreach (var c in consumers)
            {
                try
                {
                    var message = new BasicMessageConsumeContext<T>(model);
                    await c.Consume(message);
                }
                catch (Exception e)
                {
                    this.logger.AddErrorLog(e.Message,e);
                }
            }
        }
    }
}
