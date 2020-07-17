using WCloud.Core.MessageBus;

namespace WCloud.Member.Shared.MessageBody
{
    public class CopyAvatarMessage : IMessageBody
    {
        public string UserUID { get; set; }
        public string AvatarUrl { get; set; }
    }
}
