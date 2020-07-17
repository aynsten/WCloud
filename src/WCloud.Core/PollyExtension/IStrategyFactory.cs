using Lib.extension;
using Microsoft.Extensions.Logging;
using Polly;
using System;

namespace WCloud.Core.PollyExtension
{
    public class SomeApiModel { }

    /// <summary>
    /// 熔断超时重试降级策略
    /// </summary>
    public interface IStrategyFactory
    {
        IAsyncPolicy<SomeApiModel> SomeApiStrategy { get; }
    }

    public class StrategyFactory : IStrategyFactory
    {
        private readonly ILogger _logger;
        public StrategyFactory(ILogger<StrategyFactory> logger)
        {
            this._logger = logger;

            this._SomeApiStrategy = Policy.WrapAsync<SomeApiModel>(
            //一分钟内有超过10个接口调用，并且超过一般的调用失败就熔断30秒
            Policy<SomeApiModel>.Handle<Exception>()
            .AdvancedCircuitBreakerAsync(0.5, TimeSpan.FromMinutes(1), 10, TimeSpan.FromSeconds(30),
                onBreak: (res, time) =>
                {
                    this._logger.AddErrorLog($"{nameof(SomeApiStrategy)}被熔断", e: res.Exception);
                },
                onReset: () =>
                {
                    this._logger.AddErrorLog($"{nameof(SomeApiStrategy)}从熔断状态恢复");
                }),
            //重试
            Policy<SomeApiModel>.Handle<Exception>()
            .WaitAndRetryAsync(3, i => TimeSpan.FromMilliseconds(200 * i)),
            //响应超时
            Policy.TimeoutAsync<SomeApiModel>(TimeSpan.FromSeconds(5)));
        }

        private readonly IAsyncPolicy<SomeApiModel> _SomeApiStrategy;
        public IAsyncPolicy<SomeApiModel> SomeApiStrategy => this._SomeApiStrategy;
    }
}
