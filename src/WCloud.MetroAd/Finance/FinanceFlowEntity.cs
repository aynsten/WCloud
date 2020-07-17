using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.EntityFrameworkCore;

namespace WCloud.MetroAd.Finance
{
    public class FinanceFlowEntityDto : IDto
    {
        public int Id { get; set; }
    }

    public class FinanceFlowEntity : TimeEntityBase, IMetroAdTable
    {
        public virtual string OrderUID { get; set; }

        public virtual string OrderNo { get; set; }

        public virtual DateTime? OrderCreateTimeUtc { get; set; }

        public virtual int PriceInCent { get; set; }

        public virtual int FlowDirection { get; set; } = (int)FlowDirectionEnum.In;

        public virtual int PayMethod { get; set; }

        public virtual string ConsumerName { get; set; }

        public virtual string ConsumerUID { get; set; }

        [NotMapped]
        public virtual int Status { get; set; }

        [NotMapped]
        public virtual decimal Price
        {
            get
            {
                decimal res = (decimal)this.PriceInCent / 100;
                return res;
            }
            set { }
        }
    }

    public class FinanceFlowEntityProfile : Profile
    { }

    public class FinanceFlowEntityMapper : EFMappingBase<FinanceFlowEntity>
    {
        public override void Configure(EntityTypeBuilder<FinanceFlowEntity> builder)
        {
            builder.ToTable("tb_finance_flow");

            builder.Ignore(x => x.Status).Ignore(x => x.Price);
        }
    }
}
