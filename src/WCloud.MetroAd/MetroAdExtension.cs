using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace WCloud.MetroAd
{
    public static class MetroAdExtension
    {
        public static string GetMetroAdConnectionStringOrThrow(this IConfiguration config)
        {
            var cstr = config.GetConnectionString("db_metro_ad");
            cstr.Should().NotBeNullOrEmpty("请配置metro db连接字符串");
            return cstr;
        }
    }
}
