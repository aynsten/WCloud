using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.EntityFrameworkCore;

namespace WCloud.CommonService.Application.Infrastructure
{
    public class QueueJobEntityDto : DtoBase
    {
        public string JobKey { get; set; }

        public string Desc { get; set; }

        public string Exchange { get; set; }

        public string RoutingKey { get; set; }

        public string Queue { get; set; }

        public string ExceptionMessage { get; set; }

        public string ExtraData { get; set; }

        public DateTime? StartTimeUtc { get; set; }

        public DateTime? EndTimeUtc { get; set; }

        public int Status { get; set; }
    }

    [Table("tb_queue_job")]
    public class QueueJobEntity : EntityBase, ICommonServiceEntity
    {
        public string JobKey { get; set; }

        public string Desc { get; set; }

        public string Exchange { get; set; }

        public string RoutingKey { get; set; }

        public string Queue { get; set; }

        public string ExceptionMessage { get; set; }

        public string ExtraData { get; set; }

        public DateTime? StartTimeUtc { get; set; }

        public DateTime? EndTimeUtc { get; set; }

        public int Status { get; set; }
    }

    public class QueueJobEntityMapper : EFMappingBase<QueueJobEntity>
    {
        public override void Configure(EntityTypeBuilder<QueueJobEntity> builder)
        {
            builder.Property(x => x.JobKey).IsRequired().HasMaxLength(200);

            builder.Property(x => x.JobKey).HasMaxLength(200);
            builder.Property(x => x.Desc).HasMaxLength(200);
            builder.Property(x => x.Exchange).HasMaxLength(200);
            builder.Property(x => x.RoutingKey).HasMaxLength(200);
            builder.Property(x => x.Queue).HasMaxLength(200);
            builder.Property(x => x.ExceptionMessage).HasMaxLength(1000);
            builder.Property(x => x.ExtraData).HasMaxLength(4000);
        }
    }
}
