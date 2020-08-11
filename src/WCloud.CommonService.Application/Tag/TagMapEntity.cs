using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.EntityFrameworkCore;

namespace WCloud.CommonService.Application.Tag
{
    public class TagMapEntityDto : TagMapEntity, IDtoBase { }

    [Table("tb_tag_map")]
    public class TagMapEntity : EntityBase, ICommonServiceEntity
    {
        public string TagUID { get; set; }

        public string EntityType { get; set; }

        public string SubjectID { get; set; }
    }

    public class TagMapEntityMapper : EFMappingBase<TagMapEntity>
    {
        public override void Configure(EntityTypeBuilder<TagMapEntity> model)
        {
            model.Property(x => x.TagUID).IsRequired();
            model.Property(x => x.EntityType).IsRequired();
            model.Property(x => x.SubjectID).IsRequired();
        }
    }
}
