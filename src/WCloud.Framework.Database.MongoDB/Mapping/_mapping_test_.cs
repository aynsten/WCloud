#if DEBUG
using MongoDB.Bson.Serialization;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Framework.Database.MongoDB.Mapping
{
    class _mapping_test_
    {
        public _mapping_test_()
        {
            BsonClassMap.RegisterClassMap<EntityBase>(x =>
            {
                x.MapIdField(d => d.Id);
                x.MapField(x => x.Id).SetElementName("");
                x.SetDiscriminator("x");
            });
        }
    }
}
#endif