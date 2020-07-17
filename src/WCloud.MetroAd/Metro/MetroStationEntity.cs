using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.EntityFrameworkCore;

namespace WCloud.MetroAd.Metro
{
    public class MetroStationEntityDto : BaseDto { }

    public class MetroStationEntity : BaseEntity, IMetroAdTable
    {
        public virtual string Name { get; set; }

        public virtual string Desc { get; set; } = string.Empty;

        public virtual string MetroLineUID { get; set; }

        public virtual int StationType { get; set; }

        public virtual AdWindowEntity[] AdWindows { get; set; }
    }

    public class MetroStationEntityProfile : Profile { }

    public class MetroStationEntityMapper : EFMappingBase<MetroStationEntity>
    {
        public override void Configure(EntityTypeBuilder<MetroStationEntity> builder)
        {
            builder.ToTable("tb_metro_station");
            builder.Property(x => x.Name).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Desc).HasMaxLength(100);
            builder.Property(x => x.MetroLineUID).IsRequired().HasMaxLength(100);

            builder.Ignore(x => x.AdWindows);
        }
    }
}
