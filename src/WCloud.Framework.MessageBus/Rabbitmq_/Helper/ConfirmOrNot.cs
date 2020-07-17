using RabbitMQ.Client;
using System;

namespace WCloud.Framework.MessageBus.Rabbitmq_.Helper
{
    /// <summary>
    /// 出scope自动确认消息，无法确认就抛错
    /// 
    /// 事务（txselect）和confirm模式（confirm）都可以用来确认消息，但是后者效率高
    /// 
    /// 事务确实可以判断producer向Broker发送消息是否成功，只有Broker接受到消息，才会commit，
    /// 但是使用事务机制的话会降低RabbitMQ的性能，那么有没有更好的方法既能保障producer知道消息已经正确送到，
    /// 又能基本上不带来性能上的损失呢？从AMQP协议的层面看是没有更好的方法，但是RabbitMQ提供了一个更好的方案，
    /// 即将channel信道设置成confirm模式。
    /// </summary>
    public class ConfirmOrNot : IDisposable
    {
        private readonly IModel _channel;
        private readonly TimeSpan? _confirm_timeout;
        private bool _confirm { get; set; }

        public ConfirmOrNot(IModel channel, bool confirm, TimeSpan? confirm_timeout)
        {
            this._channel = channel ?? throw new ArgumentNullException(nameof(channel));
            this._confirm = confirm;
            this._confirm_timeout = confirm_timeout;

            if (!this._confirm)
                return;

            this._channel.ConfirmSelect();
        }

        /// <summary>
        /// 如果publish抛异常了，就没必要confirm了。confirm会浪费时间
        /// </summary>
        public void DontConfirmAnyMore() => this._confirm = false;

        public void Dispose()
        {
            if (this._confirm)
                try
                {
                    if (this._confirm_timeout == null)
                        this._channel.WaitForConfirmsOrDie();
                    else
                        this._channel.WaitForConfirmsOrDie(this._confirm_timeout.Value);
                }
                catch (Exception e)
                {
                    throw new TimeoutException("rabbitmq确认消息失败", e);
                }
        }
    }
}
