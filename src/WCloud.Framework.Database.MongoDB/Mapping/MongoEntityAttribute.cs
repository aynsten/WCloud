using System;
using FluentAssertions;

namespace WCloud.Framework.Database.MongoDB.Mapping
{
    public class MongoEntityAttribute : Attribute
    {
        public string CollectionName { get; }
        public MongoEntityAttribute(string collection_name)
        {
            this.CollectionName = collection_name;
            this.CollectionName.Should().NotBeNullOrEmpty();
        }
    }
}
