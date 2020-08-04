using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.CommonService.Application.Tag
{
    public interface ITagService : Lib.ioc.IAutoRegistered
    {
        Task<IEnumerable<TagEntity>> QueryAll();
        Task<_<TagEntity>> AddTag(TagEntity data);
        Task<_<TagEntity>> UpdateTag(TagEntity data);
        Task DeleteTag(string uid);
        Task MigrateTag(string from_uid, string to_uid);
        Task SaveTags<T>(T model, IEnumerable<string> tags_uid) where T : BaseEntity;
        Task SaveTags<T>(string subject_id, IEnumerable<string> tags_uid);
    }
}
