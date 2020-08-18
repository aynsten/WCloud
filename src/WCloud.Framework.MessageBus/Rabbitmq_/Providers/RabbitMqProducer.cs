using Lib.extension;
using Lib.helper;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using WCloud.Core.DataSerializer;

namespace WCloud.Framework.MessageBus.Rabbitmq_.Providers
{
    /// <summary>
    /// send a message
    /// 事务（txselect）和confirm模式（confirm）都可以用来确认消息，但是后者效率高
    /// 
    /// 事务确实可以判断producer向Broker发送消息是否成功，只有Broker接受到消息，才会commit，
    /// 但是使用事务机制的话会降低RabbitMQ的性能，那么有没有更好的方法既能保障producer知道消息已经正确送到，
    /// 又能基本上不带来性能上的损失呢？从AMQP协议的层面看是没有更好的方法，但是RabbitMQ提供了一个更好的方案，
    /// 即将channel信道设置成confirm模式。
    /// </summary>
    public class RabbitMqProducer : IRabbitMqProducer
    {
        private readonly IConnection _connection;
        private readonly IDataSerializer _serializer;

        public RabbitMqProducer(IConnection _connection, IDataSerializer _serializer)
        {
            this._connection = _connection;
            this._serializer = _serializer;
        }

        private IBasicProperties CreateBasicProperties(IModel _channel, SendMessageOption option)
        {
            if (option == null)
                throw new ArgumentNullException(nameof(option));

            var basicProperties = _channel.CreateBasicProperties();
            var header = new Dictionary<string, object>();

            //参数
            if (ValidateHelper.IsNotEmpty(option.Properties))
            {
                header.AddDict(option.Properties.ToDictionary(x => x.Key, x => x.Value));
            }
            //延迟
            if (option.Delay != null)
            {
                header["x-delay"] = Math.Abs((long)option.Delay.Value.TotalMilliseconds);
            }
            //持久化
            if (option.Persistent)
            {
                basicProperties.DeliveryMode = (byte)2;
                basicProperties.Persistent = true;
            }
            else
            {
                basicProperties.DeliveryMode = (byte)1;
                basicProperties.Persistent = false;
            }
            //优先级
            if (option.Priority != null)
            {
                basicProperties.Priority = (byte)option.Priority.Value;
            }

            //headers
            basicProperties.Headers = header;

            return basicProperties;
        }

        public void SendMessage<T>(string exchange, string routeKey, T message, SendMessageOption option = null)
        {
            option ??= new SendMessageOption() { };
            using var _channel = this._connection.CreateModel();
            var bs = this._serializer.Serialize(message);
            var props = this.CreateBasicProperties(_channel, option);

            using var confirm = new ConfirmOrNot(_channel, option.Confirm, option.ConfirmTimeout);
            try
            {
                _channel.BasicPublish(exchange: exchange,
                  routingKey: routeKey,
                  basicProperties: props,
                  body: bs);
            }
            catch
            {
                confirm.DontConfirmAnyMore();
                throw;
            }
        }


#if DEBUG
        void SendMessage<T>(string queue, T message, SendMessageOption option = null)
        {
            option ??= new SendMessageOption() { };
            using var _channel = this._connection.CreateModel();
            var bs = this._serializer.Serialize(message);
            var props = this.CreateBasicProperties(_channel, option);

            using var confirm = new ConfirmOrNot(_channel, option.Confirm, option.ConfirmTimeout);
            try
            {
                //这里queue为什么也是routing key
                _channel.BasicPublish(exchange: string.Empty,
                    routingKey: queue,
                    basicProperties: props,
                    body: bs);
            }
            catch
            {
                confirm.DontConfirmAnyMore();
                throw;
            }
        }
        [Obsolete]
        void TransactionTest()
        {
            using var _channel = this._connection.CreateModel();
            _channel.TxSelect();
            try
            {
                //publish
                //publish
                _channel.TxCommit();
            }
            catch
            {
                _channel.TxRollback();
            }
        }
#endif
    }
}
