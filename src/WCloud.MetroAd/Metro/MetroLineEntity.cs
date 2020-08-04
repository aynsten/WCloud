using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.EntityFrameworkCore;

namespace WCloud.MetroAd.Metro
{
    public class MetroLineEntityDto : BaseDto { }

    public class MetroLineEntity : BaseEntity, IMetroAdTable
    {
        public virtual string Name { get; set; }

        public virtual string Desc { get; set; }

        public virtual int AdWindowCount { get; set; }

        public virtual MetroStationEntity[] Stations { get; set; }
    }

    public class MetroLineEntityPofile : Profile { }

    public class MetroLineEntityMapper : EFMappingBase<MetroLineEntity>
    {
        public override void Configure(EntityTypeBuilder<MetroLineEntity> builder)
        {
            builder.ToTable("tb_metro_line");

            builder.Ignore(x => x.Stations);
        }
    }
}
