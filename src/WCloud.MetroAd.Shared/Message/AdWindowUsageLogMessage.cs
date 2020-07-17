using WCloud.Core.MessageBus;

namespace WCloud.MetroAd.Shared.Message
{
    public class AdWindowUsageLogMessage : IMessageBody
    {
        public string OrderUID { get; set; }
    }
}
