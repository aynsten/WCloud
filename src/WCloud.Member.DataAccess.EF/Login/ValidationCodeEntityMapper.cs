using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.Member.Domain.Login;

namespace WCloud.Member.DataAccess.EF.Login
{

    public class ValidationCodeEntityMapper : EFMappingBase<ValidationCodeEntity>
    {
        public override void Configure(EntityTypeBuilder<ValidationCodeEntity> builder)
        {
            builder.ToTable("tb_login_validation_code");

            builder.Property(x => x.Code).HasMaxLength(20);
            builder.Property(x => x.UserUID).HasMaxLength(50);
            builder.Property(x => x.UserName).HasMaxLength(20);
            builder.Property(x => x.Email).HasMaxLength(50);
            builder.Property(x => x.Phone).HasMaxLength(20);
            builder.Property(x => x.CodeType).HasMaxLength(20);
        }
    }
}
