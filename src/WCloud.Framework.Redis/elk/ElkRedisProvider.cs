using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;

namespace WCloud.Framework.Logging.elk
{
    internal class ElkRedisProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ILogger> _loggers = new ConcurrentDictionary<string, ILogger>();

        private readonly IConnectionMultiplexer _con;
        private readonly Func<string, ILogger> _logger_getter;

        public ElkRedisProvider(IConnectionMultiplexer connection, ElkRedisOption option)
        {
            this._con = connection;
            this._logger_getter = key => new ElkRedisLogger(this._con, option, key);
        }

        public ILogger CreateLogger(string categoryName)
        {
            var loggerName = categoryName ?? "default-logger";
            var logger = this._loggers.GetOrAdd(key: loggerName, valueFactory: this._logger_getter);
            return logger;
        }

        public void Dispose()
        {
            this._loggers.Clear();
        }
    }
}
