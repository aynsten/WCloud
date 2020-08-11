using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.EntityFrameworkCore;

namespace WCloud.CommonService.Application.Message
{
    public class UserMessageEntityDto : DtoBase
    {
        public virtual string FromUID { get; set; } = "system";

        public virtual string UserUID { get; set; }

        public virtual string Message { get; set; }

        public virtual int AlreadyRead { get; set; }

        public virtual DateTime? ReadTimeUtc { get; set; }
    }

    [Table("tb_user_message")]
    public class UserMessageEntity : EntityBase, ICommonServiceEntity
    {
        public virtual string FromUID { get; set; } = "system";

        public virtual string UserUID { get; set; }

        public virtual string Message { get; set; }

        public virtual int AlreadyRead { get; set; }

        public virtual DateTime? ReadTimeUtc { get; set; }
    }

    public class UserMessageEntityMapper : EFMappingBase<UserMessageEntity>
    {
        public override void Configure(EntityTypeBuilder<UserMessageEntity> builder)
        {
            builder.Property(x => x.FromUID).IsRequired().HasMaxLength(100);
            builder.Property(x => x.UserUID).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Message).IsRequired().HasMaxLength(1000);
        }
    }
}
