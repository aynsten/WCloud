using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.Member.Domain.Admin;

namespace WCloud.Member.DataAccess.EF.Admin
{

    public class AdminEntityMapper : EFMappingBase<AdminEntity>
    {
        public override void Configure(EntityTypeBuilder<AdminEntity> builder)
        {
            builder.Property(x => x.UserName).IsRequired().HasMaxLength(20);

            builder.Property(x => x.NickName).HasMaxLength(10);
            builder.Property(x => x.PassWord).HasMaxLength(100);
            builder.Property(x => x.UserImg).HasMaxLength(500);
            builder.Property(x => x.ContactPhone).HasMaxLength(30);
            builder.Property(x => x.ContactEmail).HasMaxLength(30);

            builder.HasIndex(x => x.UserName).IsUnique();
        }
    }
}
