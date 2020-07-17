using Lib.ioc;
using MongoDB.Driver;
using System;

namespace WCloud.Framework.Database.MongoDB
{
    public class MongoConnectionWrapper : ISingleInstanceService
    {
        private readonly IMongoClient _client;
        private readonly string _collection;

        public MongoConnectionWrapper(IMongoClient client, string collection_name)
        {
            this._client = client;
            this._collection = collection_name ?? throw new ArgumentNullException(nameof(collection_name));
        }

        public IMongoClient Client => this._client;

        public string CollectionName => this._collection;

        public int DisposeOrder => int.MinValue;

        public void Dispose()
        {
            //
        }
    }
}
