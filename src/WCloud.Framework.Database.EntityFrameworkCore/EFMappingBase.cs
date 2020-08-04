using Lib.data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Framework.Database.EntityFrameworkCore
{
    /// <summary>
    /// fluent map base class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EFMappingBase<T> : IEntityTypeConfiguration<T> where T : EntityBase, IDBTable
    {
        public abstract void Configure(EntityTypeBuilder<T> builder);
    }
}
