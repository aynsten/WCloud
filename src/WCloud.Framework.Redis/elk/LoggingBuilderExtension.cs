using FluentAssertions;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;

namespace WCloud.Framework.Redis.elk
{
    public static class LoggingBuilderExtension
    {
        public static ILoggingBuilder AddElkRedisPipline(this ILoggingBuilder builder,
            IConnectionMultiplexer con,
            Func<ElkRedisOption, ElkRedisOption> config = null)
        {
            con.Should().NotBeNull("elk pipline:redis connection");
            config.Should().NotBeNull("elk pipline:config");

            var option = new ElkRedisOption();
            if (config != null)
            {
                option = config.Invoke(option);
            }

            option.Valid();

            builder.AddProvider(provider: new ElkRedisProvider(connection: con, option: option));
            return builder;
        }

        [Obsolete("2.2后过期了")]
        public static ILoggerFactory AddElkRedis(this ILoggerFactory factory,
            IConnectionMultiplexer con,
            Func<ElkRedisOption, ElkRedisOption> config)
        {
            con.Should().NotBeNull("elk pipline:redis connection");
            config.Should().NotBeNull("elk pipline:config");

            var option = new ElkRedisOption();
            if (config != null)
            {
                option = config.Invoke(option);
            }

            option.Valid();

            factory.AddProvider(provider: new ElkRedisProvider(connection: con, option: option));
            return factory;
        }
    }
}
