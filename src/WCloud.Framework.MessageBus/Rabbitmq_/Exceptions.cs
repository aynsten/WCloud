using Lib.core;
using System;

namespace WCloud.Framework.MessageBus.Rabbitmq_
{
    public class TryStartRunningConumerException : BaseException
    {
        public TryStartRunningConumerException(string msg, Exception inner = null) : base(msg, inner) { }
    }

    public class ChannelClosedException : BaseException
    {
        public ChannelClosedException(string msg, Exception inner = null) : base(msg, inner) { }
    }
}
