using Lib.data;
using MongoDB.Bson;
using System;

namespace WCloud.Framework.Database.MongoDB
{
    [Serializable]
    public abstract class MongoEntityBase : IDBTable
    {
        public virtual ObjectId _id { get; set; }
    }
}
