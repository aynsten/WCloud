using AutoMapper;
using Lib.extension;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.EntityFrameworkCore;

namespace WCloud.CommonService.Application.FileUpload
{
    public class FileUploadEntityDto : DtoBase
    {

    }

    [Table("tb_file_upload")]
    public class FileUploadEntity : EntityBase, ICommonServiceEntity
    {
        public string Url { get; set; } = string.Empty;

        public string Extension { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public string FileHash { get; set; } = string.Empty;

        public string OriginFileName { get; set; } = string.Empty;

        public string ExtraData { get; set; } = string.Empty;

        public string Catalog { get; set; } = string.Empty;

        public string StorageProvider { get; set; } = string.Empty;

        public int ReferenceCount { get; set; }

        [NotMapped]
        public string QiniuKey
        {
            get
            {
                var res = $"{this.FileHash}{this.Extension}";

                var catalog_path = this.Catalog?.Split('/', '\\').WhereNotEmpty() ?? new string[] { };
                if (catalog_path.Any())
                {
                    res = string.Join("/", new List<string>().AddList_(catalog_path).AddItem_(res));
                }
                return res;
            }
        }
    }

    public class FileUploadEntityMapper : EFMappingBase<FileUploadEntity>
    {
        public override void Configure(EntityTypeBuilder<FileUploadEntity> builder)
        {
            builder.Property(x => x.StorageProvider).HasMaxLength(100);
            builder.Property(x => x.Url).HasMaxLength(500);
            builder.Property(x => x.Extension).HasMaxLength(30);
            builder.Property(x => x.FileHash).HasMaxLength(100);
            builder.Property(x => x.OriginFileName).HasMaxLength(300);
            builder.Property(x => x.Catalog).HasMaxLength(50);
            builder.Property(x => x.ExtraData).HasMaxLength(5000);

            //不同catalog里可以重复
            //builder.HasIndex(x => x.FileHash).IsUnique();
        }
    }

    public class FileUploadEntityProfile : Profile
    { }
}
