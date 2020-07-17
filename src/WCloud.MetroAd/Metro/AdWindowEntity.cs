using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.EntityFrameworkCore;

namespace WCloud.MetroAd.Metro
{
    public class AdWindowEntityDto : IDto
    {
        public int Id { get; set; }
    }

    public class AdWindowEntity : BaseEntity, IMetroAdTable
    {
        public string Name { get; set; }

        public string Desc { get; set; }

        public string MetroLineUID { get; set; }

        public string MetroStationUID { get; set; }

        public string MediaTypeUID { get; set; } = "-1";

        public string ImageListJson { get; set; } = "[]";

        public int IsActive { get; set; } = (int)YesOrNoEnum.YES;

        public int Height { get; set; }

        public int Width { get; set; }

        public int PriceInCent { get; set; }

        [NotMapped]
        public decimal Price
        {
            get
            {
                decimal res = (decimal)this.PriceInCent / 100;
                return res;
            }
            set
            {
                //do nothing
            }
        }

        [NotMapped]
        public string[] ImageList { get; set; }

        [NotMapped]
        public MediaTypeEntity MediaType { get; set; }
    }

    public class AdWindowEntityProfile : Profile
    { }

    public class AdWindowEntityMapper : EFMappingBase<AdWindowEntity>
    {
        public override void Configure(EntityTypeBuilder<AdWindowEntity> builder)
        {
            builder.ToTable("tb_ad_windows");

            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Desc).HasMaxLength(200);

            builder.Property(x => x.MetroLineUID).IsRequired().HasMaxLength(100);
            builder.Property(x => x.MetroStationUID).IsRequired().HasMaxLength(100);
            builder.Property(x => x.MediaTypeUID).HasMaxLength(100);

            builder.Ignore(x => x.ImageList);
            builder.Ignore(x => x.Price);
            builder.Ignore(x => x.MediaType);
        }
    }
}
