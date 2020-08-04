using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.Member.Domain.Admin;

namespace WCloud.MetroAd.Order
{
    public class DeployEntity : BaseEntity, IMetroAdTable
    {
        public virtual string OrderUID { get; set; }

        public virtual string ImageJson { get; set; }

        [NotMapped]
        public virtual string[] ImageList { get; set; }

        public virtual string Comment { get; set; }

        public virtual int DeploymentType { get; set; } = (int)DeployTypeEnum.Up;

        public virtual int OnSiteDeploy { get; set; } = (int)YesOrNoEnum.YES;

        public virtual string DeployerUID { get; set; }

        public virtual string DeployerName { get; set; }

        [NotMapped]
        public virtual AdminEntity Admin { get; set; }
    }

    public class DeployEntityMapper : EFMappingBase<DeployEntity>
    {
        public override void Configure(EntityTypeBuilder<DeployEntity> builder)
        {
            builder.ToTable("tb_deployment");

            builder.Ignore(x => x.ImageList);
        }
    }
}
