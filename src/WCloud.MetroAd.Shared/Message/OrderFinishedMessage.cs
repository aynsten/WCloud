using WCloud.Core.MessageBus;

namespace WCloud.MetroAd.Shared.Message
{
    public class OrderFinishedMessage : IMessageBody
    {
        public string OrderUID { get; set; }
    }
}
