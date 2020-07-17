using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.Member.Domain.Admin;

namespace WCloud.Member.DataAccess.EF.Admin
{

    public class RoleEntityMapper : EFMappingBase<RoleEntity>
    {
        public override void Configure(EntityTypeBuilder<RoleEntity> builder)
        {
            builder.HasIndex(x => x.NodeName).IsUnique();
            builder.Property(x => x.RoleDescription).HasMaxLength(100);
            builder.Property(x => x.PermissionJson).IsRequired();
        }
    }
}
