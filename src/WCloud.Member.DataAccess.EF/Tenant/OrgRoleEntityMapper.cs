using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.Member.Domain.Tenant;

namespace WCloud.Member.DataAccess.EF.Tenant
{

    public class OrgRoleEntityMapper : EFMappingBase<OrgRoleEntity>
    {
        public override void Configure(EntityTypeBuilder<OrgRoleEntity> builder)
        {
            builder.Property(x => x.RoleName).IsRequired();
            builder.Property(x => x.OrgUID).IsRequired();
            builder.Property(x => x.PermissionJson).IsRequired();
        }
    }
}
