using System;
using Lib.data;
using MongoDB.Bson;

namespace WCloud.Framework.Database.MongoDB
{
    [Serializable]
    public abstract class MongoEntityBase : IDBTable
    {
        public virtual ObjectId _id { get; set; }

        public virtual string UID { get; set; }

        public virtual DateTime CreateTimeUtc { get; set; }
    }
}
