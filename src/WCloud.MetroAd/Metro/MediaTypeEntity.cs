using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.EntityFrameworkCore;

namespace WCloud.MetroAd.Metro
{
    public class MediaTypeEntityDto : DtoBase { }

    public class MediaTypeEntity : EntityBase, IMetroAdTable
    {
        public string Name { get; set; }
    }

    public class MediaTypeEntityProfile : Profile
    { }

    public class MediaTypeEntityMapper : EFMappingBase<MediaTypeEntity>
    {
        public override void Configure(EntityTypeBuilder<MediaTypeEntity> builder)
        {
            builder.ToTable("tb_media_type");
            builder.Property(x => x.Name).IsRequired().HasMaxLength(20);
        }
    }
}
