using FluentAssertions;
using Lib.extension;
using Lib.helper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WCloud.Core.DataSerializer;
using WCloud.Core.MessageBus;

namespace WCloud.Framework.MessageBus.Redis_
{
    public interface IRedisConsumer : IDisposable
    {
        void StartConsume();
    }

    public class RedisConsumer<T> : IRedisConsumer where T : class, IMessageBody
    {
        private Task[] tasks;

        private readonly IServiceProvider provider;
        private readonly CancellationTokenSource cancellationToken;
        private readonly IRedisDatabaseSelector redisDatabaseSelector;
        private readonly IDataSerializer serializeProvider;
        private readonly ILogger<RedisConsumer<T>> logger;

        private readonly QueueConfigAttribute config;

        public RedisConsumer(IServiceProvider provider,
            IRedisDatabaseSelector redisDatabaseSelector,
            IDataSerializer serializeProvider,
            ILogger<RedisConsumer<T>> logger)
        {
            this.provider = provider;
            this.serializeProvider = serializeProvider;
            this.logger = logger;

            this.redisDatabaseSelector = redisDatabaseSelector;

            var map = provider.ResolveMessageTypeMapping<T>();
            this.config = map.Config;

            this.cancellationToken = new CancellationTokenSource();
        }

        public void StartConsume()
        {
            this.tasks.Should().BeNullOrEmpty("消费任务已经开启");
            this.tasks = Com.Range(1).Select(x => Task.Run(this.Consume, this.cancellationToken.Token)).ToArray();
        }

        Task __wait__() => Task.Delay(TimeSpan.FromMilliseconds(100));

        async Task __fetch_and_consume__()
        {
            var bs = (byte[])await this.redisDatabaseSelector.Database.ListRightPopAsync(this.config.QueueName);

            var model = ValidateHelper.IsNotEmpty(bs) ?
                this.serializeProvider.Deserialize<T>(bs) :
                null;

            if (model == null)
            {
                await this.__wait__();
                return;
            }
            using var scope = this.provider.CreateScope();
            var all_subscribers = scope.ServiceProvider.ResolveConsumerOptional<T>();
            foreach (var c in all_subscribers)
            {
                try
                {
#if DEBUG
                    this.logger.LogInformation($"消费：{model.ToJson()}");
#endif

                    var message = new BasicMessageConsumeContext<T>(model);
                    await c.Consume(message);
                }
                catch (Exception e)
                {
                    this.logger.AddErrorLog(e.Message, e);
                }
            }
        }

        async Task Consume()
        {
            while (!this.cancellationToken.Token.IsCancellationRequested)
            {
                try
                {
                    await this.__fetch_and_consume__();
                }
                catch (Exception e)
                {
                    this.logger.AddErrorLog($"redis消费：{e.Message}", e);
                    await this.__wait__();
                }
            }
        }

        public void Dispose()
        {
            this.cancellationToken.Cancel();
            this.cancellationToken.Dispose();
        }
    }
}
