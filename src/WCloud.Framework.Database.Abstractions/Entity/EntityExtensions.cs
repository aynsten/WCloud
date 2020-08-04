using System;

namespace WCloud.Framework.Database.Abstractions.Entity
{
    public static class EntityExtensions
    {
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
        public static void Update(this IUpdateTime model, DateTime? utc_time = null)
        {
            var time = utc_time ?? DateTime.UtcNow;
            model.UpdateTimeUtc = time;
        }
    }
}
