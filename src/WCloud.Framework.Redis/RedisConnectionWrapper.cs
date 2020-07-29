using FluentAssertions;
using Lib.ioc;
using StackExchange.Redis;
using System;

namespace Lib.redis
{
    public interface IRedisConnection : IDisposable
    {
        IConnectionMultiplexer Connection { get; }
    }

    public class RedisConnectionWrapper : IRedisConnection, ISingleInstanceService
    {
        private readonly IConnectionMultiplexer _con;

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
            // 
        }

        public IConnectionMultiplexer Connection => this._con;

        public int DisposeOrder => int.MinValue;

        public void Dispose()
        {
            this._con?.Dispose();
        }
    }
}
