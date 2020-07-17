using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.Member.Domain.Tenant;

namespace WCloud.Member.DataAccess.EF.Tenant
{

    public class OrgEntityMapper : EFMappingBase<OrgEntity>
    {
        public override void Configure(EntityTypeBuilder<OrgEntity> builder)
        {
            builder.HasIndex(x => x.OrgName).IsUnique();
        }
    }
}
