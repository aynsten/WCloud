using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.EntityFrameworkCore;

namespace WCloud.CommonService.Application.KVStore
{
    public class KVStoreEntity : EntityBase, ICommonServiceEntity
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class KVStoreEntityMapper : EFMappingBase<KVStoreEntity>
    {
        public override void Configure(EntityTypeBuilder<KVStoreEntity> builder)
        {
            builder.ToTable("tb_kv_store");
            builder.Property(x => x.Key).IsRequired().HasMaxLength(300);
        }
    }
}
