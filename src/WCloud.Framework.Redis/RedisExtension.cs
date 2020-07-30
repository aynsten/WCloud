using StackExchange.Redis;

namespace WCloud.Framework.Redis
{
    public static class RedisExtension
    {
        public static IDatabase SelectDatabase(this IConnectionMultiplexer con, int? db)
        {
            var res = db == null ? con.GetDatabase() : con.GetDatabase(db.Value);

            return res;
        }
    }
}
