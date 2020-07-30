using WCloud.Core;
using WCloud.Core.MessageBus;

namespace WCloud.Member.Shared.MessageBody
{
    [QueueConfig(ConfigSet.MessageBus.Queue.User)]
    public class CopyAvatarMessage : IMessageBody
    {
        public string UserUID { get; set; }
        public string AvatarUrl { get; set; }
    }
}
