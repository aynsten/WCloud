using Lib.extension;
using Lib.helper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WCloud.Framework.MessageBus.Rabbitmq_.Providers
{
    /// <summary>
    /// 批量消费
    /// </summary>
    public abstract class RabbitMqBatchConsumerBase : IRabbitMqConsumer
    {
        protected readonly IServiceProvider provider;
        protected readonly ILogger logger;
        protected readonly IConnection _connection;
        protected readonly BatchConsumeOption _option;
        protected readonly CancellationTokenSource _token;

        protected List<Task> _threads { get; set; }
        public Func<bool> Pause { get; set; }

        public RabbitMqBatchConsumerBase(IServiceProvider provider, ILogger logger, IConnection connection, BatchConsumeOption option)
        {
            this.provider = provider;
            this.logger = logger;
            this._connection = connection;
            this._option = option ?? throw new ArgumentNullException(nameof(option));
            this._option.Valid();

            this._token = new CancellationTokenSource();
        }

        public void StartConsume()
        {
            this._threads = Com.Range(this._option.ConcurrencySize ?? 1).Select(x => Task.Run(this.FetchDataInBatch)).ToList();
        }

        public abstract Task OnConsume(IServiceProvider provider, IReadOnlyList<BasicGetResult> data, Action<BasicGetResult, bool> ack_callback);

        void AckMessage(IModel _channel, BasicGetResult x, bool success)
        {
            if (this._option.AutoAck)
            {
                return;
            }
            if (success)
            {
                _channel.BasicAck(x.DeliveryTag, multiple: true);
            }
            else
            {
                _channel.BasicNack(x.DeliveryTag, multiple: true, requeue: true);
            }
        }

        /// <summary>
        /// 3年或者10万公里逻辑
        /// </summary>
        /// <returns></returns>
        async IAsyncEnumerable<ReadOnlyCollection<BasicGetResult>> __batch_message__(IModel _channel)
        {
            var batch_data = new List<BasicGetResult>();

            var breaker = Policy.Handle<Exception>().AdvancedCircuitBreakerAsync(
                failureThreshold: 3,
                samplingDuration: TimeSpan.FromMinutes(1),
                minimumThroughput: 1,
                durationOfBreak: TimeSpan.FromMinutes(1));

            async Task<bool> __sleep__()
            {
                var _continue = false;
                if (!_channel.IsOpen)
                {
                    try
                    {
                        await breaker.ExecuteAsync(async () =>
                        {
                            this.logger.AddErrorLog("rabbitmq消费异常：channel已经关闭，等待恢复");
                            await Task.Delay(TimeSpan.FromSeconds(10));
                            throw new Exception("错误日志熔断，避免写入太多日志");
                        });
                    }
                    catch
                    {
                        //do nothing
                    }
                    _continue = true;
                }

                //是否暂停
                else if (this.Pause?.Invoke() ?? false)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    _continue = true;
                }
                return _continue;
            }

            var cursor = DateTime.UtcNow;

            bool Timeout(DateTime now) => (cursor + this._option.BatchTimeout) < now;
            while (true)
            {
                if (this._token.IsCancellationRequested)
                    break;

                var _continue = await __sleep__();
                if (_continue)
                    continue;

                //从队列中读取数据放入列表
                var data = _channel.BasicGet(this._option.QueueName, autoAck: this._option.AutoAck);
                if (data == null)
                {
                    //there is no data in queue,await 100 miliseconds
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                    continue;
                }
                batch_data.Add(data);

                //如果当前列表里的数据满足条件就yield
                var now = DateTime.UtcNow;
                if ((batch_data.Count >= this._option.BatchSize || Timeout(now)) && batch_data.Any())
                {
                    //consume
                    var d = batch_data.AsReadOnly();
                    yield return d;
                    //batch consuming finished ,reset cursor
                    batch_data = new List<BasicGetResult>() { };
                    cursor = now;
                }
            }
        }

        /// <summary>
        /// acquire multiple data from queue
        /// </summary>
        /// <returns></returns>
        async Task FetchDataInBatch()
        {
            using var _channel = this._connection.CreateModel();
            await foreach (var data in this.__batch_message__(_channel))
            {
                using var s = this.provider.CreateScope();
                try
                {
                    await this.OnConsume(s.ServiceProvider, data, (x, success) => this.AckMessage(_channel, x, success));
                }
                catch (Exception e)
                {
                    this.logger.AddErrorLog($"rabbitmq批量消费发生异常:{e.Message}", e);
                }
            }
        }

        public void Cancel() => this._token.Cancel();

        public void Dispose()
        {
            try
            {
                //require stop,instead of force those threads killed
                this.Cancel();

                if (this._threads?.Any() ?? false)
                {
                    if (this._option.DisposeTimeout != null)
                    {
                        //timeout mode
                        this._threads.Add(Task.Delay(this._option.DisposeTimeout.Value));
                        Task.WhenAny(this._threads).Wait();
                    }
                    else
                    {
                        //wait forever mode
                        Task.WhenAll(this._threads).Wait();
                    }
                }
            }
            catch (Exception e)
            {
                this.logger.AddErrorLog(e.Message, e);
            }
            try
            {
                this._connection?.Dispose();
            }
            catch (Exception e)
            {
                this.logger.AddErrorLog(e.Message, e);
            }
            try
            {
                this._token.Dispose();
            }
            catch (Exception e)
            {
                this.logger.AddErrorLog(e.Message, e);
            }
        }
    }
}
