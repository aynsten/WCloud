using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.Member.Domain.Login;

namespace WCloud.Member.DataAccess.EF.Login
{

    public class UserPhoneEntityMapper : EFMappingBase<UserPhoneEntity>
    {
        public override void Configure(EntityTypeBuilder<UserPhoneEntity> builder)
        {
            builder.Property(x => x.UserUID).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Phone).IsRequired().HasMaxLength(20);

            builder.HasIndex(x => x.Phone).IsUnique();
        }
    }
}
