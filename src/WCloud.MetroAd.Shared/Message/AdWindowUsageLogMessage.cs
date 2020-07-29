using WCloud.Core;
using WCloud.Core.MessageBus;

namespace WCloud.MetroAd.Shared.Message
{
    [QueueConfig(ConfigSet.MessageBus.Queue.MetroAd)]
    public class AdWindowUsageLogMessage : IMessageBody
    {
        public string OrderUID { get; set; }
    }
}
