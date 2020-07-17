using Lib.extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Data;
using System.Linq;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Framework.Database.EntityFrameworkCore
{
    public static class MySQLExtension
    {
        public static EntityTypeBuilder<T> ConfigBaseEntityForMySQL<T>(this EntityTypeBuilder<T> builder) where T : BaseEntity
        {
            //--
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd().UseMySqlIdentityColumn();

            //--
            builder.HasIndex(x => x.UID).IsUnique();
            builder.Property(x => x.UID).IsRequired().HasMaxLength(100).ValueGeneratedNever();

            //--
            //builder.Property(x => x.CreateTimeUtc).HasDefaultValueSql("CURRENT_TIMESTAMP");

            return builder;
        }

        public static EntityTypeBuilder<T> ConfigRemoveableEntityForMySQL<T>(this EntityTypeBuilder<T> builder) where T : class, ILogicalDeletion
        {
            builder.HasIndex(x => x.IsDeleted);

            builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue((int)YesOrNoEnum.No);

            builder.HasQueryFilter(x => x.IsDeleted == 0);

            return builder;
        }

        public static EntityTypeBuilder<T> ConfigRowVersionEntityForMySQL<T>(this EntityTypeBuilder<T> builder) where T : class, IRowVersion
        {
            builder.Property(x => x.RowVersion).IsRequired().IsRowVersion().ValueGeneratedOnAddOrUpdate();

            return builder;
        }

        public static EntityTypeBuilder<T> ConfigUpdateTimeEntityForMySQL<T>(this EntityTypeBuilder<T> builder) where T : class, IUpdateTime
        {
            builder.Property(x => x.UpdateTimeUtc);

            return builder;
        }

        public static Type[] FindEntityTypes<DbContextType>(this DbContextType db) where DbContextType : DbContext
        {
            var res = db
                .GetType()
                .GetProperties()
                .Select(x => x.PropertyType)
                .Where(x => x.IsGenericType_(typeof(DbSet<>)))
                .Select(x => x.GenericTypeArguments.FirstOrDefault())
                .WhereNotNull()
                .ToArray();

            return res;
        }

        public static void AutoApplyEntityConfigurationForMySQL<DbContextType>(this DbContextType db, ModelBuilder modelBuilder)
            where DbContextType : DbContext
        {
            AutoApplyEntityConfigurationForMySQL<DbContextType, Lib.data.IDBTable>(db, modelBuilder);
        }

        /// <summary>
        /// 等待优化
        /// </summary>
        /// <typeparam name="DbContextType"></typeparam>
        /// <param name="db"></param>
        /// <param name="modelBuilder"></param>
        public static void AutoApplyEntityConfigurationForMySQL<DbContextType, ITable>(this DbContextType db, ModelBuilder modelBuilder)
            where DbContextType : DbContext
        {
            //注册base类型
            var method = modelBuilder
                .GetType()
                .GetMethods()
                .Where(x => x.Name == nameof(modelBuilder.ApplyConfiguration))
                .Where(x => x.IsGenericMethod && x.GetGenericArguments().Length == 1)
                .FirstOrDefault() ??
                throw new ArgumentException("找不到方法");

            var entitys = FindEntityTypes(db).Where(x => x.IsAssignableTo<ITable>()).ToArray();

            foreach (var m in entitys)
            {
                var config_type = typeof(ConfigBaseEntityForMySQLConfiguration<>).MakeGenericType(m);
                var config_ins = Activator.CreateInstance(config_type);

                method.MakeGenericMethod(m).Invoke(modelBuilder, new object[] { config_ins });

                $"EF:初始化base字段{m.FullName}".DebugInfo();
            }

            //注册其他类型----------------------------------------------------------------------------
            Type __get_generic_arg__(Type x)
            {
                var type = x.GetInterfaces().FirstOrDefault(d => d.IsGenericType_(typeof(IEntityTypeConfiguration<>)));
                var res = type?.GenericTypeArguments.FirstOrDefault();
                return res;
            }

            var all_mapping_types = db.GetType().Assembly.GetTypes()
                .Where(x => x.IsPublic)
                .Where(x => !x.IsGenericType)
                .Where(x => x.IsNormalClass())
                .Select(x => new
                {
                    Type = x,
                    GenericParam = __get_generic_arg__(x)
                })
                .Where(x => x.GenericParam != null)
                .Where(x => x.GenericParam.IsAssignableTo<ITable>())
                .ToArray();

            foreach (var m in all_mapping_types)
            {
                var mapper_instance = Activator.CreateInstance(m.Type);

                method.MakeGenericMethod(m.GenericParam).Invoke(modelBuilder, new object[] { mapper_instance });

                $"EF:初始化其他字段{m.GenericParam.FullName}".DebugInfo();
            }
        }
    }
}
