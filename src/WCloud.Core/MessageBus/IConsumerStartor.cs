using System;

namespace WCloud.Core.MessageBus
{
    /// <summary>
    /// 开启或者关闭消息消费
    /// </summary>
    public interface IConsumerStartor : IDisposable
    {
        void StartComsume();
    }
}
