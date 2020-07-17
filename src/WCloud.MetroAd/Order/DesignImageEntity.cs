using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.Member.Domain.Admin;

namespace WCloud.MetroAd.Order
{
    public class DesignImageEntity : BaseEntity, IMetroAdTable
    {
        public virtual string OrderUID { get; set; }

        public virtual string DesignImageJson { get; set; }

        [NotMapped]
        public virtual string[] DesignImages { get; set; }

        public virtual string Comment { get; set; }

        public virtual string DesignerUID { get; set; }

        [NotMapped]
        public virtual AdminEntity Admin { get; set; }
    }

    public class DesignImageEntityMapper : EFMappingBase<DesignImageEntity>
    {
        public override void Configure(EntityTypeBuilder<DesignImageEntity> builder)
        {
            builder.ToTable("tb_design");

            builder.Property(x => x.OrderUID).IsRequired().HasMaxLength(100);
            builder.Property(x => x.DesignImageJson);
            builder.Property(x => x.DesignerUID).IsRequired().HasMaxLength(100);

            builder.Ignore(x => x.DesignImages);
        }
    }
}
