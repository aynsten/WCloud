using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.Member.Domain.User;

namespace WCloud.Member.DataAccess.EF.User
{

    public class UserEntityMapping : EFMappingBase<UserEntity>
    {
        public override void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.Property(x => x.UserName).HasMaxLength(20);
            builder.Property(x => x.CurrentLoginPhone).HasMaxLength(20);

            builder.Property(x => x.NickName).HasMaxLength(10);
            builder.Property(x => x.PassWord).HasMaxLength(100);
            builder.Property(x => x.UserImg).HasMaxLength(500);

            builder.Property(x => x.IdCard).HasMaxLength(100);
            builder.Property(x => x.RealName).HasMaxLength(100);

            builder.HasIndex(x => x.UserName).IsUnique();

            builder.Ignore(x => x.UserPhone);
        }
    }
}
