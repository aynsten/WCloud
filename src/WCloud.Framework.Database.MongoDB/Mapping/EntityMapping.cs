using System;
using Lib.extension;
using FluentAssertions;
using System.Linq;

namespace WCloud.Framework.Database.MongoDB.Mapping
{
    public class EntityMapping
    {
        public Type EntityType { get; }
        public string CollectionName { get; }

        public EntityMapping(Type entity_type)
        {
            entity_type.Should().NotBeNull();
            this.EntityType = entity_type;

            var config = this.EntityType.GetCustomAttributes_<MongoEntityAttribute>().FirstOrDefault();
            config.Should().NotBeNull();
            this.CollectionName = config.CollectionName;
        }

    }
}
