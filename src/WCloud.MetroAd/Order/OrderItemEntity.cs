using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.MetroAd.Metro;

namespace WCloud.MetroAd.Order
{
    public class OrderItemEntityDto : BaseDto { }

    public class OrderItemEntity : BaseEntity, IMetroAdTable
    {
        public virtual string OrderUID { get; set; }

        public virtual string MetroLineUID { get; set; }

        public virtual string MetroStationUID { get; set; }

        public virtual int MetroStationType { get; set; }

        public virtual string AdWindowUID { get; set; }

        public virtual string MetroLineName { get; set; }

        public virtual string MetroStationName { get; set; }

        public virtual string AdWindowName { get; set; }

        public virtual string MediaTypeName { get; set; }

        public virtual int PriceInCent { get; set; }

        public virtual decimal Price
        {
            get
            {
                decimal res = (decimal)this.PriceInCent / 100;
                return res;
            }
            set { }
        }

        public virtual int Height { get; set; }

        public virtual int Width { get; set; }

        public virtual string MediaTypeUID { get; set; }

        /// <summary>
        /// 下单时产生的广告位快照
        /// </summary>
        public virtual string AdWindowSnapshotJson { get; set; }

        [NotMapped]
        public MetroLineEntity MetroLine { get; set; }

        [NotMapped]
        public MetroStationEntity MetroStation { get; set; }

        [NotMapped]
        public AdWindowEntity AdWindow { get; set; }

        [NotMapped]
        public MediaTypeEntity MediaType { get; set; }
    }

    public class OrderItemEntityProfile : Profile { }

    public class OrderItemEntityMapper : EFMappingBase<OrderItemEntity>
    {
        public override void Configure(EntityTypeBuilder<OrderItemEntity> builder)
        {
            builder.ToTable("tb_order_item");
            builder.Property(x => x.OrderUID).IsRequired().HasMaxLength(100);
            builder.Property(x => x.MetroLineUID).IsRequired().HasMaxLength(100);
            builder.Property(x => x.MetroStationUID).IsRequired().HasMaxLength(100);
            builder.Property(x => x.AdWindowUID).IsRequired().HasMaxLength(100);

            builder.Ignore(x => x.Price);
            builder.Ignore(x => x.MetroLine).Ignore(x => x.MetroStation).Ignore(x => x.AdWindow).Ignore(x => x.MediaType);
        }
    }
}
