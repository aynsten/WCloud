using System;
using Lib.data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WCloud.Framework.Database.MongoDB
{
    public abstract class MongoEntityBase : IDBTable
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public virtual string Id { get; set; }

        [BsonElement(nameof(UID))]
        public virtual string UID { get; set; }

        public virtual DateTime CreateTimeUtc { get; set; }
    }
}
