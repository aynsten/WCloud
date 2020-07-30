using Lib.extension;
using System.Threading;
using WCloud.Framework.Redis.implement;

namespace WCloud.Framework.Redis
{
    class MQTest
    {
        public MQTest()
        {
            var stop = false;
            {
                var redis = new RedisHelper(null, 1);
                var db = redis.Database;
                while (true)
                {
                    if (stop) { break; }

                    var data = db.ListRightPop("es-index");
                    if (!data.HasValue)
                    {
                        Thread.Sleep(50);
                        continue;
                    }
                    //handler data
                    string json = data;
                    var model = json.JsonToEntity<RedisHelper>();
                }
            }
        }
    }

}
