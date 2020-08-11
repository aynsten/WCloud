using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.EntityFrameworkCore;

namespace WCloud.CommonService.Application.FileUpload
{
    public class FileOwnerEntityDto : IDto
    {
        public string Id { get; set; }
    }

    [Table("tb_file_owner")]
    public class FileOwnerEntity : EntityBase, ICommonServiceEntity
    {
        public string UserUID { get; set; }
        public string FileUID { get; set; }
    }

    public class FileOwnerEntityMapper : EFMappingBase<FileOwnerEntity>
    {
        public override void Configure(EntityTypeBuilder<FileOwnerEntity> builder)
        {
            builder.Property(x => x.UserUID).IsRequired().HasMaxLength(100);
            builder.Property(x => x.FileUID).IsRequired().HasMaxLength(100);
        }
    }

    public class FileOwnerEntityProfile : Profile
    { }
}
