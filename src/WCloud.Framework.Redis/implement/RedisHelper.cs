using System;
using System.Threading.Tasks;
using Lib.data;
using Lib.redis;
using StackExchange.Redis;

namespace Lib.redis
{
    /// <summary>
    /// Redis操作
    /// https://github.com/qq1206676756/RedisHelp
    /// 个人觉得对NoSql数据库的操作不要使用异步，因为本身响应就很快，不会阻塞
    /// </summary>
    public partial class RedisHelper : IRedisAll
    {
        private readonly IConnectionMultiplexer _conn;
        private readonly IDatabase _database;
        private readonly ISerializeProvider _serializer;

        public IConnectionMultiplexer Connection => this._conn;
        public IDatabase Database => this._database;

        public RedisHelper(IConnectionMultiplexer conn, int db, ISerializeProvider serializeProvider = null)
        {
            this._conn = conn;
            this._database = this._conn.SelectDatabase(db);
            this._serializer = serializeProvider ?? new DefaultSerializeProvider();
        }

        public const string DeleteKeyWithValueScript =
            @"if redis.call('get', KEYS[1]) == ARGV[1] then return redis.call('del',KEYS[1]) else return 0 end";

        /// <summary>
        /// key和value匹配的时候才删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public int DeleteKeyWithValue(string key, byte[] val) =>
            (int)this.Database.ScriptEvaluate(DeleteKeyWithValueScript, new RedisKey[] { key }, new RedisValue[] { val });

        /// <summary>
        /// key和value匹配的时候才删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public async Task<int> DeleteKeyWithValueAsync(string key, byte[] val) =>
            (int)(await this.Database.ScriptEvaluateAsync(DeleteKeyWithValueScript, new RedisKey[] { key }, new RedisValue[] { val }));

        /// <summary>
        /// 如果key不存在时写入
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expire"></param>
        /// <returns></returns>
        public async Task<bool> StringSetWhenNotExist(string key, byte[] value, TimeSpan expire) =>
             await this.Database.StringSetAsync(key, value, expire, When.NotExists);

        public ITransaction CreateTransaction() => this.Database.CreateTransaction();

        public IServer GetServer(string hostAndPort)
        {
            return _conn.GetServer(hostAndPort);
        }
    }
}
