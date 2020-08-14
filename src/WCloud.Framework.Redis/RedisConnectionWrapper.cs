using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;

namespace WCloud.Framework.Redis
{
    public class RedisConnectionWrapper : ISingleInstanceService
    {
        private readonly IConnectionMultiplexer _con;
        public string ConnectionString { get; }

        public RedisConnectionWrapper(IConnectionMultiplexer con)
        {
            this._con = con ?? throw new ArgumentNullException(nameof(IConnectionMultiplexer));
        }

        static IConnectionMultiplexer __connect__(string con_str)
        {
            con_str.Should().NotBeNullOrEmpty();

            var res = ConnectionMultiplexer.Connect(con_str);
            var i = 0;
            while (!res.IsConnected)
            {
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                (++i).Should().BeLessThan(10, "无法链接redis，等待时间过长");
            }

            return res;
        }

        public RedisConnectionWrapper(string connectionString) : this(__connect__(connectionString))
        {
            this.ConnectionString = connectionString;
            this.ConnectionString.Should().NotBeNullOrEmpty();
        }

        public IConnectionMultiplexer Connection => this._con;

        public int DisposeOrder => int.MinValue;

        public void Dispose()
        {
            this._con?.Dispose();
        }
    }
}
