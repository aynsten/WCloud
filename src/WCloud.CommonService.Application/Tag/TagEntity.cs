using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.EntityFrameworkCore;

namespace WCloud.CommonService.Application.Tag
{
    public class TagEntityDto : DtoBase
    {
        public string TagName { get; set; }

        public string Desc { get; set; } = string.Empty;

        public string Icon { get; set; }

        public string Image { get; set; }

        public int ReferenceCount { get; set; }

        public string Group { get; set; }
    }

    [Table("tb_tag")]
    public class TagEntity : EntityBase, ICommonServiceEntity
    {
        public string TagName { get; set; }

        public string Desc { get; set; } = string.Empty;

        public string Icon { get; set; }

        public string Image { get; set; }

        public int ReferenceCount { get; set; }

        public string Group { get; set; }
    }

    public class TagEntityMapper : EFMappingBase<TagEntity>
    {
        public override void Configure(EntityTypeBuilder<TagEntity> builder)
        {
            builder.HasIndex(x => x.TagName).IsUnique();
            builder.Property(x => x.TagName).IsRequired().HasMaxLength(20);
        }
    }
}
