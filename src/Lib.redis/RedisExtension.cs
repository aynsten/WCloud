using StackExchange.Redis;
using System.Threading.Tasks;

namespace Lib.redis
{
    public static class RedisExtension
    {
        public static IDatabase SelectDatabase(this IConnectionMultiplexer con, int? db)
        {
            var res = db == null ? con.GetDatabase() : con.GetDatabase(db.Value);

            return res;
        }

        [System.Obsolete]
        public static async Task ConsumeList(this IDatabase db, string key)
        {
            while (true)
            {

            }
        }
    }
}
