using System;
using FluentAssertions;
using Lib.extension;

namespace Microsoft.Extensions.Configuration
{
    public static class ConfigurationExtensions
    {
        public static string GetAppName(this IConfiguration config) => config["app_name"];

        public static bool InitDatabaseRequired(this IConfiguration config)
        {
            var res = config["init_db"]?.ToBool() ?? false;
            return res;
        }

        public static string GetIdentityServerAddressOrThrow(this IConfiguration config)
        {
            var identity_server = config["identity_server"];
            identity_server.Should().NotBeNullOrEmpty($"请配置{nameof(identity_server)}");

            identity_server.EndsWith("/").Should().BeFalse("identity server后面不要加斜杠");

            return identity_server;
        }

        public static string GetInternalApiGatewayAddressOrThrow(this IConfiguration config)
        {
            var res = config["internal_api_gateway"];
            res.Should().NotBeNullOrEmpty();
            res.StartsWith("http", StringComparison.OrdinalIgnoreCase).Should().BeTrue();
            return res;
        }

        public static string GetRedisConnectionString(this IConfiguration config)
        {
            var con_str = config["redis_server"];

            return con_str;
        }

        public static string GetSentryDsn(this IConfiguration config)
        {
            var dsn = config["sentry_dsn"];

            return dsn;
        }
    }
}
