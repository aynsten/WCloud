using MongoDB.Bson.Serialization;

namespace WCloud.Framework.Database.MongoDB.Mapping
{
    public interface IMongoEntityMapping
    {
        public abstract string CollectionName { get; }
    }

    public interface IMongoEntityMapping<T> : IMongoEntityMapping
    {
        public abstract void ConfigMap(BsonClassMap<T> config);
    }
}
