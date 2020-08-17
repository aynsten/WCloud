using MongoDB.Bson.Serialization;
using WCloud.Framework.Database.MongoDB.Mapping;
using WCloud.Member.Domain.Admin;

namespace WCloud.Member.DataAccess.MongoDB.Admin
{
    public class AdminEntityMapper : IMongoEntityMapping<AdminEntity>
    {
        public string CollectionName => "tb_admin";

        public void ConfigMap(BsonClassMap<AdminEntity> config)
        {
            config.BasicConfig();
        }
    }
}
