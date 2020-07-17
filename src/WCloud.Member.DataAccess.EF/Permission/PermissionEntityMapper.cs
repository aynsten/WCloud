using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.Member.Domain.Permission;

namespace WCloud.Member.DataAccess.EF.Permission
{
    public class PermissionEntityMapper : EFMappingBase<PermissionEntity>
    {
        public override void Configure(EntityTypeBuilder<PermissionEntity> builder)
        {
            builder.ToTable("tb_permission");
            builder.Property(x => x.PermissionName).HasMaxLength(50);
            builder.Property(x => x.PermissionKey).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Desc).HasMaxLength(200);
        }
    }
}
