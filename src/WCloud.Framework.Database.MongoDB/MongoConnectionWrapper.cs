using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace WCloud.Framework.Database.MongoDB
{
    public class MongoConnectionWrapper : ISingleInstanceService
    {
        private readonly IMongoClient _client;
        private readonly string _database_name;

        public MongoConnectionWrapper(IMongoClient client, string database_name)
        {
            client.Should().NotBeNull();
            database_name.Should().NotBeNullOrEmpty();

            this._client = client;
            this._database_name = database_name;
        }

        public MongoConnectionWrapper(string connection_string, string database_name) : this(new MongoClient(connection_string), database_name)
        { }

        public IMongoClient Client => this._client;

        public string DatabaseName => this._database_name;

        public int DisposeOrder => int.MinValue;

        public void Dispose()
        {
            //
        }
    }
}
