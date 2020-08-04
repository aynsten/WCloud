using FluentAssertions;
using Lib.extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;
using System.Reflection;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Framework.Database.EntityFrameworkCore
{
    public class ConfigBaseEntityForMySQLConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.ConfigBaseEntityForMySQL();

            //这个算逆变/协变？
            /*
            if (builder is EntityTypeBuilder<IRemove> removeMarkBuilder)
            {
                removeMarkBuilder.ConfigIRemoveEntityForMySQL();
            }

            if (builder is EntityTypeBuilder<IRowVersion> rowVersionBuilder)
            {
                rowVersionBuilder.ConfigRowVersionForMySQL();
            }*/

            //不行老子就反射，妈的

            var entity_type = typeof(T);
            var all_methods = typeof(MySQLExtension).GetMethods();
            var builder_param = new object[] { builder };

            MethodInfo get_method(string name)
            {
                var list = all_methods.Where(x => x.Name == name).ToArray();
                list.Length.Should().Be(1);

                var m = list.First();

                m.IsGenericMethod.Should().BeTrue();

                var res = m.MakeGenericMethod(new Type[] { entity_type });

                return res;
            }

            if (entity_type.IsAssignableTo_<ILogicalDeletion>())
            {
                get_method(nameof(MySQLExtension.ConfigRemoveableEntityForMySQL)).Invoke(null, builder_param);
            }

            if (entity_type.IsAssignableTo_<IRowVersion>())
            {
                get_method(nameof(MySQLExtension.ConfigRowVersionEntityForMySQL)).Invoke(null, builder_param);
            }

            if (entity_type.IsAssignableTo<IUpdateTime>())
            {
                get_method(nameof(MySQLExtension.ConfigUpdateTimeEntityForMySQL)).Invoke(null, builder_param);
            }
        }
    }
}
