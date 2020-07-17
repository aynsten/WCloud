using Lib.extension;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;
using WCloud.Framework.Socket.Connection;

namespace WCloud.Framework.Socket.Transport
{
    public class RedisTransportProvider : ITransportProvider
    {
        private readonly ILogger logger;
        private readonly IRedisDatabaseSelector redisDatabaseSelector;
        private readonly IMessageSerializer messageSerializer;
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();

        private Task refresh_key_task;
        private Task consume_task;
        private Task consume_broadcast_task;

        public RedisTransportProvider(IRedisDatabaseSelector redisDatabaseSelector, IMessageSerializer messageSerializer, ILogger<RedisTransportProvider> logger)
        {
            this.redisDatabaseSelector = redisDatabaseSelector;
            this.messageSerializer = messageSerializer;
            this.logger = logger;
        }

        const string BROAD_CAST_KEY = "message_all";

        public async Task BroadCast(MessageWrapper data)
        {
            var bs = this.messageSerializer.SerializeBytes(data);
            await this.redisDatabaseSelector.Database.PublishAsync(BROAD_CAST_KEY, bs);
        }

        public void Dispose()
        {
            this.queue?.Unsubscribe();
            this.tokenSource.Cancel();
            this.logger.LogInformation("关闭");
        }

        public async Task RouteToServerInstance(string key, MessageWrapper data)
        {
            var bs = this.messageSerializer.SerializeBytes(data);
            await this.redisDatabaseSelector.Database.ListLeftPushAsync(key, bs);
        }

        private ChannelMessageQueue queue;
        public async Task SubscribeBroadcastMessageEndpoint(Func<MessageWrapper, Task> callback)
        {
            async Task consume()
            {
                this.queue = await this.redisDatabaseSelector.Connection.GetSubscriber().SubscribeAsync(BROAD_CAST_KEY);
                while (true && !this.tokenSource.IsCancellationRequested)
                {
                    try
                    {
                        if (!queue.TryRead(out var item))
                        {
                            await Task.Delay(TimeSpan.FromMilliseconds(100), this.tokenSource.Token);
                            continue;
                        }
                        var message = this.messageSerializer.Deserialize<MessageWrapper>((byte[])item.Message);
                        await callback.Invoke(message);
                    }
                    catch (Exception e)
                    {
                        this.logger.AddErrorLog(e.Message, e);
                        await Task.Delay(TimeSpan.FromMilliseconds(100), this.tokenSource.Token);
                    }
                }
            }
            this.consume_broadcast_task = Task.Run(consume, this.tokenSource.Token);
            this.logger.LogInformation("开始订阅广播");
            await Task.CompletedTask;
        }

        async Task refresh_key(string key)
        {
            var db = this.redisDatabaseSelector.Database;
            while (true && !this.tokenSource.IsCancellationRequested)
            {
                try
                {
                    await db.KeyExpireAsync(key, TimeSpan.FromHours(6));
                    await Task.Delay(TimeSpan.FromSeconds(60), this.tokenSource.Token);
                }
                catch (Exception e)
                {
                    this.logger.AddErrorLog(e.Message, e);
                }
            }
            this.logger.LogInformation("结束刷新key");
        }

        public async Task SubscribeMessageEndpoint(string key, Func<MessageWrapper, Task> callback)
        {
            async Task consume()
            {
                while (true && !this.tokenSource.IsCancellationRequested)
                {
                    try
                    {
                        var res = await this.redisDatabaseSelector.Database.ListRightPopAsync(key);
                        if (!res.HasValue)
                        {
                            await Task.Delay(TimeSpan.FromMilliseconds(100), this.tokenSource.Token);
                            continue;
                        }
                        var data = this.messageSerializer.Deserialize<MessageWrapper>((byte[])res);
                        await callback.Invoke(data);
                    }
                    catch (Exception e)
                    {
                        this.logger.AddErrorLog(e.Message, e);
                        await Task.Delay(TimeSpan.FromMilliseconds(100), this.tokenSource.Token);
                    }
                }
            }

            this.consume_task = Task.Run(consume, this.tokenSource.Token);
            this.refresh_key_task = Task.Run(() => this.refresh_key(key), this.tokenSource.Token);
            this.logger.LogInformation("开始订阅，开始刷新key");
            await Task.CompletedTask;
        }
    }
}
