using FluentAssertions;
using Lib.data;
using Lib.extension;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Framework.Database.Abstractions.Extension
{
    public static class EntityExtension
    {
        /// <summary>
        /// 初始化后返回自己
        /// </summary>
        public static T InitSelf<T>(this T model, string flag = null)
            where T : BaseEntity
        {
            model.Init(flag);
            return model;
        }

        /// <summary>
        /// 更新信息后返回自己
        /// </summary>
        public static T UpdateSelf<T>(this T model)
            where T : IUpdateTime
        {
            model.Update();
            return model;
        }

        public static T SetField<T>(this T model, object data) where T : BaseEntity
        {
            model.Should().NotBeNull();
            data.Should().NotBeNull();

            var props = model.GetType().GetProperties();

            foreach (var m in data.GetType().GetProperties())
            {
                var val = m.GetValue(data);
                var prop = props.FirstOrDefault(x => x.Name == m.Name);
                prop.Should().NotBeNull();

                prop.SetValue(model, val);
            }

            return model;
        }

        /// <summary>
        /// check input=>delete by uids
        /// </summary>
        public static async Task DeleteByIds<T>(this IRepository<T> repo, string[] uids) where T : BaseEntity
        {
            uids.Should().NotBeNullOrEmpty();
            foreach (var uid in uids)
            {
                uid.Should().NotBeNullOrEmpty("批量删除数据：uid为空");
            }

            await repo.DeleteWhereAsync(x => uids.Contains(x.UID));
        }

        /// <summary>
        /// init=>check=>save
        /// </summary>
        public static async Task<_<T>> AddEntity_<T>(this IRepository<T> repo, T model) where T : BaseEntity
        {
            var res = new _<T>();

            model.InitSelf();

            if (!model.IsValid(out var msg))
            {
                return res.SetErrorMsg(msg);
            }

            await repo.AddAsync(model);

            return res.SetSuccessData(model);
        }
    }
}
