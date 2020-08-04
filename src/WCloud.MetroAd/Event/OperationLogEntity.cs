using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.Member.Shared;

namespace WCloud.MetroAd.Event
{
    public class OperationLogEntityDto : IDto
    {
        public int Id { get; set; }
    }

    [Table("tb_operation_log")]
    public class OperationLogEntity : BaseEntity, IMetroAdTable
    {
        public virtual int AccountType { get; set; } = (int)AccountTypeEnum.Admin;

        public virtual string UserUID { get; set; }

        public virtual string UserName { get; set; }

        public virtual string Page { get; set; }

        public virtual string Action { get; set; }

        public virtual string Message { get; set; }

        public virtual string ExtraDataJson { get; set; }

        public virtual string Platform { get; set; } = "admin";
    }

    public class OperationLogEntityProfile : Profile
    { }

    public class OperationLogEntityMapper : EFMappingBase<OperationLogEntity>
    {
        public override void Configure(EntityTypeBuilder<OperationLogEntity> builder)
        {
            builder.ToTable("tb_operation_log");
        }
    }
}
