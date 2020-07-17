using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.EntityFrameworkCore;

namespace WCloud.MetroAd.Order
{
    public class PaymentNotificationEntityDto : BaseDto { }

    public class PaymentNotificationEntity : BaseEntity, IMetroAdTable
    {
        public virtual int PayMethod { get; set; }

        public virtual string OrderNo { get; set; } = string.Empty;

        public virtual string OutTradeNo { get; set; } = string.Empty;

        public virtual string OrderUID { get; set; } = string.Empty;

        /// <summary>
        /// 支付订单号
        /// </summary>
        public virtual string ExternalPaymentNo { get; set; } = string.Empty;

        public virtual string NotificationData { get; set; } = string.Empty;
    }

    public class PaymentNotificationEntityProfile : Profile { }

    public class PaymentNotificationEntityMapper : EFMappingBase<PaymentNotificationEntity>
    {
        public override void Configure(EntityTypeBuilder<PaymentNotificationEntity> builder)
        {
            builder.ToTable("tb_payment_notification");
        }
    }
}
