using MongoDB.Bson.Serialization;

namespace WCloud.Framework.Database.MongoDB.Mapping
{
    public interface IMongoEntityMapping
    {
        string CollectionName { get; }
    }

    public interface IMongoEntityMapping<T> : IMongoEntityMapping
    {
        void ConfigMap(BsonClassMap<T> config);
    }
}
