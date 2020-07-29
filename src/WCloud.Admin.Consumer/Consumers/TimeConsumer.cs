using Lib.extension;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WCloud.Framework.MessageBus;

namespace WCloud.Admin.Consumer.Consumers
{
    public class TimeConsumer : IMessageConsumer<TimeMessage>
    {
        private readonly ILogger logger;
        public TimeConsumer(ILogger<TimeConsumer> logger)
        {
            this.logger = logger;
        }

        public async Task Consume(IMessageConsumeContext<TimeMessage> context)
        {
            this.logger.LogDebug(context.Message?.ToJson() ?? "errrrrrrrrrrrrrrrrr");
            await Task.CompletedTask;
        }
    }
}
