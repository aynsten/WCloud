using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.EntityFrameworkCore;

namespace WCloud.MetroAd.Statistic
{
    public class AdWindowUsageEntity : EntityBase, IMetroAdTable
    {
        public string OrderUID { get; set; }

        public DateTime DateUtc { get; set; }
        public string LineUID { get; set; }
        public string StationUID { get; set; }
        public int StationType { get; set; }
        public string AdWindowUID { get; set; }
        public string MediaTypeUID { get; set; }
        public int PriceInCent { get; set; }
    }

    public class AdWindowUsageEntityMapper : EFMappingBase<AdWindowUsageEntity>
    {
        public override void Configure(EntityTypeBuilder<AdWindowUsageEntity> builder)
        {
            builder.ToTable("tb_adwindow_usage");

            builder.Property(x => x.OrderUID).HasMaxLength(100);
            builder.Property(x => x.LineUID).HasMaxLength(100);
            builder.Property(x => x.StationUID).HasMaxLength(100);
            builder.Property(x => x.AdWindowUID).HasMaxLength(100);
            builder.Property(x => x.MediaTypeUID).HasMaxLength(100);
        }
    }
}
