using FluentAssertions;
using Lib.extension;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;

namespace WCloud.Framework.Redis.elk
{
    internal class ElkRedisLogger : ILogger
    {
        private readonly ElkRedisOption _option;
        private readonly IConnectionMultiplexer _connection;
        private readonly string _logger_name;

        private IDatabase __db__;
        private readonly object _locker_ = new object();
        private IDatabase Database
        {
            get
            {
                if (this.__db__ == null)
                {
                    lock (this._locker_)
                    {
                        if (this.__db__ == null)
                        {
                            this.__db__ = this._connection.SelectDatabase(this._option.Database);
                        }
                    }
                }

                return this.__db__;
            }
        }

        public ElkRedisLogger(IConnectionMultiplexer connection, ElkRedisOption option, string logger_name)
        {
            connection.Should().NotBeNull();
            option.Should().NotBeNull();
            logger_name.Should().NotBeNullOrEmpty();

            this._connection = connection;
            this._option = option;
            this._logger_name = logger_name;
        }

        class EmptyDisposable : IDisposable
        {
            public void Dispose() { }
        }

        private readonly EmptyDisposable _disposer = new EmptyDisposable();

        public IDisposable BeginScope<TState>(TState state) => this._disposer;

        public bool IsEnabled(LogLevel logLevel)
        {
            if (this._option.MinLevel != null && logLevel < this._option.MinLevel.Value)
            {
                return false;
            }
            if (this._option.MaxLevel != null && logLevel > this._option.MaxLevel.Value)
            {
                return false;
            }
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!this.IsEnabled(logLevel))
            {
                return;
            }
            try
            {
                var msg = formatter?.Invoke(state, exception);

                var obj = new
                {
                    _event = new
                    {
                        eventId.Id,
                        eventId.Name
                    },
                    level = logLevel.LogLevelAsString(),
                    msg,
                    ex = exception?.GetInnerExceptionAsList(),
                    _logger_name
                };

                var values = new RedisValue[] { obj.ToJson() };
                this.Database.ListLeftPush(this._option.Key, values);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
    }
}
