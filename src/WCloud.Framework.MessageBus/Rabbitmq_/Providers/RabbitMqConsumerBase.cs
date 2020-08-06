using FluentAssertions;
using Lib.core;
using Lib.data;
using Lib.extension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading.Tasks;
using WCloud.Core.MessageBus;

namespace WCloud.Framework.MessageBus.Rabbitmq_.Providers
{
    /// <summary>
    /// 测试ok
    /// </summary>
    public abstract class RabbitMqConsumerBase<T> : IRabbitMqConsumer where T : class
    {
        protected readonly IServiceProvider provider;
        protected readonly ILogger logger;
        protected readonly ISerializeProvider _serializer;
        protected readonly ConsumeOption _option;

        protected readonly IModel _channel;
        protected readonly AsyncEventingBasicConsumer _consumer;

        public RabbitMqConsumerBase(IServiceProvider provider, ILogger logger, IConnection connection, ConsumeOption option)
        {
            provider.Should().NotBeNull();
            logger.Should().NotBeNull();
            connection.Should().NotBeNull();
            option.Should().NotBeNull();

            this.provider = provider;
            this.logger = logger;
            this._option = option;

            this._serializer = provider.ResolveSerializer();
            this._channel = connection.CreateModel();

            //qos
            if (this._option.ConcurrencySize != null)
            {
                this._channel.BasicQos(prefetchSize: 0, prefetchCount: this._option.ConcurrencySize.Value, global: false);
            }

            //异步消费
            this._consumer = new AsyncEventingBasicConsumer(this._channel);

            //注册消费事件
            this._consumer.Received += async (sender, args) =>
            {
                using var s = this.provider.CreateScope();

                var body = this.__deserialize__(args.Body.ToArray());
                if (body == null)
                {
                    return;
                }
                var context = new BasicMessageConsumeContext<T>(body);
                if (!this._option.AutoAck)
                {
                    context.AckHandler = async (success) =>
                    {
                        if (success)
                            this._channel.BasicAck(args.DeliveryTag, multiple: true);
                        else
                            this._channel.BasicNack(args.DeliveryTag, multiple: true, requeue: true);
                        await Task.CompletedTask;
                    };
                }

                try
                {
                    await this.OnMessageReceived(context);
                    await context.Ack(true);
                }
                catch (Exception e)
                {
                    //log errors
                    this.logger.AddErrorLog($"rabbitmq消费发生异常:{e.Message}", e);
                    await context.Ack(false);
                }
            };
        }

        T __deserialize__(byte[] bs)
        {
            try
            {
                var res = Policy.Handle<Exception>().Retry(3).Execute(() => this._serializer.Deserialize<T>(bs));
                return res;
            }
            catch (Exception e)
            {
                this.logger.AddErrorLog(e.Message, e);
                return default;
            }
        }

        public void StartConsume()
        {
            if (this._consumer.IsRunning)
            {
                throw new TryStartRunningConumerException("尝试开启一个正在运行的消费");
            }
            if (!this._channel.IsOpen)
            {
                throw new ChannelClosedException("无法开启消费，通道已经关闭");
            }

            var consumerTag = $"{Environment.MachineName}|{this._option.QueueName}|{this._option.ConsumerName}";
            this._channel.BasicConsume(
                queue: this._option.QueueName,
                autoAck: this._option.AutoAck,
                consumerTag: consumerTag,
                consumer: this._consumer);
        }

        public abstract Task OnMessageReceived(IMessageConsumeContext<T> context);

        public void Dispose()
        {
            try
            {
                this._channel?.Close();
            }
            catch (Exception e)
            {
                this.logger.AddErrorLog(e.Message, e);
            }
            try
            {
                this._channel?.Dispose();
            }
            catch (Exception e)
            {
                this.logger.AddErrorLog(e.Message, e);
            }
        }
    }
}
