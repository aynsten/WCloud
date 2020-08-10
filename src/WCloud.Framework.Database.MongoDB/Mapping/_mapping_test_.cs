#if DEBUG
using MongoDB.Bson.Serialization;

namespace WCloud.Framework.Database.MongoDB.Mapping
{
    class _mapping_test_
    {
        public _mapping_test_()
        {
            BsonClassMap.RegisterClassMap<MongoEntityBase>(x =>
            {
                x.MapIdField(d => d.Id);
                x.MapField(x => x.UID).SetElementName("");
            });
        }
    }
}
#endif