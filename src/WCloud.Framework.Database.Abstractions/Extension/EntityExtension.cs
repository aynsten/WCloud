using FluentAssertions;
using Lib.helper;
using System;
using System.Linq;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Framework.Database.Abstractions.Extension
{
    public static class EntityExtension
    {
        /// <summary>
        /// 初始化后返回自己
        /// </summary>
        public static T InitEntity<T>(this T model, DateTime? utc_time = null)
            where T : EntityBase
        {
            var time = utc_time ?? DateTime.UtcNow;

            var uid = Com.GetUUID();
            model.SetId(uid);

            model.CreateTimeUtc = time;
            return model;
        }

        public static T InitTimeEntity<T>(this T model) where T : TimeEntityBase
        {
            model.InitEntity();
            model.TimeYear = model.CreateTimeUtc.Year;
            model.TimeMonth = model.CreateTimeUtc.Month;
            model.TimeDay = model.CreateTimeUtc.Day;
            model.TimeHour = model.CreateTimeUtc.Hour;
            return model;
        }

        /// <summary>
        /// 中国是东八区
        /// </summary>
        const int BeijingTimezoneOffset = +8;

        public static DateTime? UpdateTimeBeijingTimezone(this IUpdateTime model)
        {
            var res = model.UpdateTimeUtc?.AddHours(BeijingTimezoneOffset);
            return res;
        }

        public static DateTime CreateTimeBeijingTimezone(this EntityBase model)
        {
            var res = model.CreateTimeUtc.AddHours(BeijingTimezoneOffset);
            return res;
        }

        /// <summary>
        /// 更新时间等信息
        /// </summary>
        public static T SetUpdateTime<T>(this T model, DateTime? utc_time = null)
            where T : IUpdateTime
        {
            var time = utc_time ?? DateTime.UtcNow;
            model.UpdateTimeUtc = time;
            return model;
        }

        public static T SetField<T>(this T model, object data) where T : EntityBase
        {
            model.Should().NotBeNull();
            data.Should().NotBeNull();

            var props = model.GetType().GetProperties();

            foreach (var m in data.GetType().GetProperties())
            {
                var val = m.GetValue(data);
                var prop = props.FirstOrDefault(x => x.Name == m.Name);
                prop.Should().NotBeNull();
                prop.CanWrite.Should().BeTrue();

                prop.SetValue(model, val);
            }

            return model;
        }
    }
}
