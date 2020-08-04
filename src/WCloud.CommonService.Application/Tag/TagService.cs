using FluentAssertions;
using Lib.extension;
using Lib.helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.Abstractions.Extension;

namespace WCloud.CommonService.Application.Tag
{
    public class TagService : ITagService
    {
        private readonly ICommonServiceRepository<TagEntity> _tagRepo;

        public TagService(ICommonServiceRepository<TagEntity> _tagRepo)
        {
            this._tagRepo = _tagRepo;
        }

        string EntityType<T>()
        {
            var tp = typeof(T);

            var custom_name = tp.GetCustomAttribute<TagTypeNameAttribute>()?.Name ??
                tp.GetCustomAttributes_<TableAttribute>().FirstOrDefault()?.Name;

            return (custom_name?.ToLower() ?? tp.FullName).Trim();
        }

        public async Task<IEnumerable<TagEntity>> QueryAll()
        {
            var data = await this._tagRepo.GetListAsync(x => x.Id >= 0, count: 5000);

            return data;
        }

        public async Task<_<TagEntity>> AddTag(TagEntity data)
        {
            data.Should().NotBeNull("add tag data");
            data.TagName.Should().NotBeNullOrEmpty("add tag tag name");

            var res = new _<TagEntity>();

            if (await this._tagRepo.ExistAsync(x => x.TagName == data.TagName))
                return res.SetErrorMsg("存在同名标签");

            data.InitSelf();

            await this._tagRepo.AddAsync(data);
            return res.SetSuccessData(data);
        }

        public async Task<_<TagEntity>> UpdateTag(TagEntity data)
        {
            data.Should().NotBeNull("update tag data");
            data.UID.Should().NotBeNullOrEmpty("update tag uid");
            data.TagName.Should().NotBeNullOrEmpty("update tag tag name");

            var model = await this._tagRepo.GetFirstAsync(x => x.UID == data.UID);
            model.Should().NotBeNull();

            model.SetField(new
            {
                data.TagName,
                data.Desc,
                data.Icon,
                data.Image
            });

            var res = new _<TagEntity>();

            if (await this._tagRepo.ExistAsync(x => x.Id != model.Id && x.TagName == model.TagName))
                return res.SetErrorMsg("存在同名标签");

            await this._tagRepo.UpdateAsync(model);
            return res.SetSuccessData(model);
        }

        public async Task DeleteTag(string uid)
        {
            uid.Should().NotBeNullOrEmpty("delete tag uid");

            await this._tagRepo.DeleteByIds(new string[] { uid });
        }

        public async Task MigrateTag(string from_uid, string to_uid)
        {
            from_uid.Should().NotBeNullOrEmpty("migrate tag from uid");
            to_uid.Should().NotBeNullOrEmpty("migrate tag to uid");
            from_uid.Should().NotBe(to_uid,"migrate tag from and to uid");

            var db = this._tagRepo.Database;
            var set = db.Set<TagMapEntity>();

            while (true)
            {
                var data = await set.Where(x => x.TagUID == from_uid).Take(100).ToArrayAsync();
                if (!data.Any())
                {
                    break;
                }
                foreach (var m in data)
                    m.TagUID = to_uid;

                await db.SaveChangesAsync();
            }
        }

        public async Task SaveTags<T>(T model, IEnumerable<string> tags_uid) where T : BaseEntity
        {
            model.Should().NotBeNull("save tags model");

            await this.SaveTags<T>(model.UID, tags_uid);
        }

        public async Task SaveTags<T>(string subject_id, IEnumerable<string> tags_uid)
        {
            subject_id.Should().NotBeNullOrEmpty("save tag subject id");

            var entity_type = this.EntityType<T>();

            var db = this._tagRepo.Database;

            var set = db.Set<TagMapEntity>();

            set.RemoveRange(set.Where(x => x.EntityType == entity_type && x.SubjectID == subject_id));
            await db.SaveChangesAsync();

            if (ValidateHelper.IsNotEmpty(tags_uid))
            {
                set.AddRange(tags_uid.Select(x => new TagMapEntity()
                {
                    EntityType = entity_type,
                    SubjectID = subject_id,
                    TagUID = x
                }.InitSelf()));
                await db.SaveChangesAsync();
            }
        }
    }
}
