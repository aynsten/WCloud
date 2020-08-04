using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.MetroAd.Metro;

namespace WCloud.MetroAd.Showcase
{
    public class CaseEntityDto : BaseDto { }

    public class CaseEntity : BaseEntity, IMetroAdTable
    {
        public virtual string Name { get; set; }

        public virtual string Desc { get; set; }

        public virtual string AdWindowUIDJson { get; set; } = "[]";

        public virtual string ImageJson { get; set; } = "[]";

        public virtual int IsActive { get; set; } = (int)YesOrNoEnum.YES;

        [NotMapped]
        public virtual string[] AdWindowUIDList { get; set; }

        [NotMapped]
        public virtual AdWindowEntity[] AdWindows { get; set; }

        [NotMapped]
        public virtual string[] ImageList { get; set; }
    }

    public class CaseEntityProfile : Profile { }

    public class CaseEntityMapper : EFMappingBase<CaseEntity>
    {
        public override void Configure(EntityTypeBuilder<CaseEntity> builder)
        {
            builder.ToTable("tb_showcase");

            builder.Ignore(x => x.AdWindowUIDList).Ignore(x => x.AdWindows).Ignore(x => x.ImageList);
        }
    }
}
