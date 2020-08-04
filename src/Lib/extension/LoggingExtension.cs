using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lib.extension
{
    public static class LoggerExtension
    {
        public static ILoggerFactory ResolveLoggerFactory(this IServiceProvider provider)
        {
            var res = provider.Resolve_<ILoggerFactory>();
            return res;
        }

        public static ILogger<T> ResolveLogger<T>(this IServiceProvider provider)
        {
            var logger = provider.Resolve_<ILogger<T>>();
            return logger;
        }

        public static void AddErrorLog(this ILogger logger, string msg, Exception e = null)
        {
            var message = msg ?? e?.Message ?? string.Empty;

            if (e != null)
            {
                logger.LogError(message: message, exception: e);
            }
            else
            {
                logger.LogError(message: message);
            }
        }

        public static void AddWarningLog(this ILogger logger, string msg, Exception e = null)
        {
            var message = msg ?? e?.Message ?? string.Empty;

            if (e != null)
            {
                logger.LogWarning(message: message, exception: e);
            }
            else
            {
                logger.LogWarning(message: message);
            }
        }

        public static void AddInfoLog(this ILogger logger, string msg, Exception e = null)
        {
            var message = msg ?? e?.Message ?? string.Empty;

            if (e != null)
            {
                logger.LogInformation(message: message, exception: e);
            }
            else
            {
                logger.LogInformation(message: message);
            }
        }

        public static void DebugInfo(this string log)
        {
            System.Diagnostics.Debug.WriteLine(log);
        }

        public static void DebugInfo(this Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.GetInnerExceptionAsJson());
        }

        public static string LogLevelAsString(this LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Critical:
                    return nameof(LogLevel.Critical);
                case LogLevel.Debug:
                    return nameof(LogLevel.Debug);
                case LogLevel.Error:
                    return nameof(LogLevel.Error);
                case LogLevel.Information:
                    return nameof(LogLevel.Information);
                case LogLevel.None:
                    return nameof(LogLevel.None);
                case LogLevel.Trace:
                    return nameof(LogLevel.Trace);
                case LogLevel.Warning:
                    return nameof(LogLevel.Warning);
                default:
                    return string.Empty;
            }
        }
    }

    public class LoggerName
    {
        private readonly string _name;
        public LoggerName(string name)
        {
            this._name = name ??
                throw new ArgumentNullException(nameof(LoggerName));
        }

        public string Logger => this._name;


        public static implicit operator LoggerName(string logger) =>
            new LoggerName(logger);

        public static implicit operator LoggerName(Type t) =>
            new LoggerName(t.FullName);

        public static implicit operator string(LoggerName logger) =>
            logger.Logger;
    }

    public class LoggerName<T> : LoggerName
    {
        public LoggerName() : base(typeof(T).FullName) { }
    }
}