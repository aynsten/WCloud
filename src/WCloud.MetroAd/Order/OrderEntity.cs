using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.Member.Domain.Admin;
using WCloud.Member.Domain.User;

namespace WCloud.MetroAd.Order
{
    public class OrderEntityDto : BaseDto { }

    public class OrderEntity : BaseEntity, ILogicalDeletion, IMetroAdTable
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public virtual string OrderNo { get; set; } = string.Empty;

        /// <summary>
        /// 顾客uid
        /// </summary>
        public virtual string UserUID { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public virtual int TotalPriceInCent { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public virtual DateTime AdStartTimeUtc { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public virtual DateTime AdEndTimeUtc { get; set; }

        /// <summary>
        /// 一共几天
        /// </summary>
        public virtual int TotalDays { get; set; }

        /// <summary>
        /// 用户需求
        /// </summary>
        public virtual string CustomerDemand { get; set; }

        /// <summary>
        /// 用户需求的图片json
        /// </summary>
        public virtual string CustomerImageJson { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public virtual int Status { get; set; } = (int)OrderStatusEnum.待审核;

        /// <summary>
        /// 是否删除
        /// </summary>
        public virtual int IsDeleted { get; set; } = (int)YesOrNoEnum.No;
        //----------------------------------------------------------------------------

        /// <summary>
        /// 审核人
        /// </summary>
        public virtual string ApproverUID { get; set; }

        /// <summary>
        /// 审核备注
        /// </summary>
        public virtual string ApproveComment { get; set; } = string.Empty;

        /// <summary>
        /// 审核时间
        /// </summary>
        public virtual DateTime? ApproveTimeUtc { get; set; }

        //----------------------------------------------------------------------------

        /// <summary>
        /// 设计稿数量
        /// </summary>
        public virtual int DesignCount { get; set; }

        /// <summary>
        /// 顾客确认的设计方案
        /// </summary>
        public virtual string ConfirmedDesignUID { get; set; } = string.Empty;

        public virtual DateTime? DesignConfirmTimeUtc { get; set; }

        //----------------------------------------------------------------------------

        /// <summary>
        /// 支付方式
        /// </summary>
        public virtual int PayMethod { get; set; } = (int)PayMethodEnum.None;

        /// <summary>
        /// 支付订单号
        /// </summary>
        public virtual string ExternalPaymentNo { get; set; }

        /// <summary>
        /// 手动支付操作员
        /// </summary>
        public virtual string ManualPaymentOperatorUID { get; set; }

        /// <summary>
        /// 支付备注
        /// </summary>
        public virtual string PaymentComment { get; set; }

        /// <summary>
        /// 支付凭证
        /// </summary>
        public virtual string PaymentVoucherJson { get; set; } = "[]";

        /// <summary>
        /// 支付时间
        /// </summary>
        public virtual DateTime? PayTime { get; set; }

        public virtual int PaymentPending { get; set; } = 0;

        public virtual int PayCounter { get; set; } = 0;

        //----------------------------------------------------------------------------

        /// <summary>
        /// 投放号
        /// </summary>
        public virtual string TFNo { get; set; } = string.Empty;

        //----------------------------------------------------------------------------
        public virtual string CloseReason { get; set; }
        public virtual DateTime? CloseTimeUtc { get; set; }
        public virtual DateTime? DeployUpTimeUtc { get; set; }
        public virtual DateTime? FinishTimeUtc { get; set; }
        //----------------------------------------------------------------------------

        /// <summary>
        /// 订单总金额
        /// </summary>
        [NotMapped]
        public virtual decimal TotalPrice
        {
            get
            {
                decimal res = (decimal)this.TotalPriceInCent / 100;
                return res;
            }
            set { }
        }

        [NotMapped]
        public virtual string[] ImageList { get; set; }

        [NotMapped]
        public virtual string[] PaymentVoucherList { get; set; }

        [NotMapped]
        public virtual OrderItemEntity[] OrderItems { get; set; }

        [NotMapped]
        public virtual OrderHistoryEntity[] OrderHistories { get; set; }

        [NotMapped]
        public virtual DesignImageEntity[] OrderDesigns { get; set; }

        [NotMapped]
        public virtual DeployEntity[] OrderDeploymentUp { get; set; }

        [NotMapped]
        public virtual DeployEntity[] OrderDeploymentDown { get; set; }

        [NotMapped]
        public virtual UserEntity User { get; set; }

        [NotMapped]
        public virtual AdminEntity Approver { get; set; }
    }

    public class OrderEntityProfile : Profile { }

    public class OrderEntityMapper : EFMappingBase<OrderEntity>
    {
        public override void Configure(EntityTypeBuilder<OrderEntity> builder)
        {
            builder.ToTable("tb_order");

            builder.Property(x => x.OrderNo).HasMaxLength(100);
            builder.Property(x => x.TFNo).HasMaxLength(100);
            builder.Property(x => x.CustomerDemand).HasMaxLength(3000);
            builder.Property(x => x.ApproverUID).HasMaxLength(100);

            builder.Property(x => x.CloseReason).HasMaxLength(300);

            builder.Ignore(x => x.TotalPrice);
            builder.Ignore(x => x.OrderItems);
            builder.Ignore(x => x.OrderDesigns);
            builder.Ignore(x => x.OrderDeploymentUp).Ignore(x => x.OrderDeploymentDown);
            builder.Ignore(x => x.OrderHistories);
            builder.Ignore(x => x.User);
            builder.Ignore(x => x.Approver);
            builder.Ignore(x => x.ImageList);
        }
    }
}
