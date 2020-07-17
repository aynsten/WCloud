using Microsoft.Extensions.Logging;
using Quartz.Logging;
using System;

namespace WCloud.Framework.Jobs.Quartz_
{
    public class QuartzLogProvider : ILogProvider, IDisposable
    {
        private readonly ILoggerFactory _loggerFactory;

        public QuartzLogProvider(ILoggerFactory loggerFactory) => _loggerFactory = loggerFactory;

        public Logger GetLogger(string name)
        {
            var _logger = this._loggerFactory.CreateLogger(name);

            Func<string, Exception, string> Formatter = (msg, ex) => msg;

            bool Log(Quartz.Logging.LogLevel logLevel, Func<string> messageFunc, Exception exception = null, params object[] formatParameters)
            {
                var level = GetLogLevel(logLevel);
                if (!_logger.IsEnabled(level))
                    return false;

                var message = messageFunc?.Invoke();
                if (message != null)
                    _logger.Log(level, 0, string.Format(message, formatParameters), exception, Formatter);

                return true;
            }

            return Log;
        }


        private static Microsoft.Extensions.Logging.LogLevel GetLogLevel(Quartz.Logging.LogLevel logLevel)
        {
            switch (logLevel)
            {
                case Quartz.Logging.LogLevel.Fatal:
                    return Microsoft.Extensions.Logging.LogLevel.Critical;
                case Quartz.Logging.LogLevel.Error:
                    return Microsoft.Extensions.Logging.LogLevel.Error;
                case Quartz.Logging.LogLevel.Warn:
                    return Microsoft.Extensions.Logging.LogLevel.Warning;
                case Quartz.Logging.LogLevel.Info:
                    return Microsoft.Extensions.Logging.LogLevel.Information;
                case Quartz.Logging.LogLevel.Trace:
                    return Microsoft.Extensions.Logging.LogLevel.Trace;
                default:
                    return Microsoft.Extensions.Logging.LogLevel.Debug;
            }
        }

        #region Hidden
        IDisposable ILogProvider.OpenNestedContext(string message) => this;

        IDisposable ILogProvider.OpenMappedContext(string key, string value) => this;

        void IDisposable.Dispose() { }
        #endregion
    }
}
