using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.EntityFrameworkCore;

namespace WCloud.MetroAd.Order
{
    public class OrderHistoryEntity : EntityBase, IMetroAdTable
    {
        public OrderHistoryEntity() { }

        public string OrderUID { get; set; }

        public int Status { get; set; }
    }

    public class OrderHistoryEntityMapper : EFMappingBase<OrderHistoryEntity>
    {
        public override void Configure(EntityTypeBuilder<OrderHistoryEntity> builder)
        {
            builder.ToTable("tb_order_history");

            builder.Property(x => x.OrderUID).IsRequired().HasMaxLength(100);
        }
    }
}
