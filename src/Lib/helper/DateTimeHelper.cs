using System;
using System.Globalization;
using System.Linq;

namespace Lib.helper
{
    public static class DateTimeHelper
    {
        public static readonly DateTime UTC1970 = new DateTime(
            year: 1970, month: 1, day: 1,
            hour: 0, minute: 0, second: 0,
            kind: DateTimeKind.Utc);

        public const string FormatHourMin = "HH:mm";
        public const string FormatHourMinSecond = "HH:mm:ss";
        public const string FormatHourMinSecondMillisecond = "HH:mm:ss.fff";
        public const string FormatDate = "yyyy-MM-dd";
        public const string FormatDateTime = "yyyy-MM-dd HH:mm";
        public const string FormatDateTimeWithSecond = "yyyy-MM-dd HH:mm:ss";
        public const string FormatDateChinese = "yyyy年MM月dd日";

        /// <summary>
        /// 时间戳生成规则
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp(DateTime? utc_time = null)
        {
            var time = utc_time ?? DateTime.UtcNow;
            var ts = time - UTC1970;
            return (long)ts.TotalSeconds;
        }

        public static string GetDateTimeStringFromCulture(DateTime time, CultureInfo cul) => time.ToString(cul.DateTimeFormat);


        /// <summary>
        /// 获取年龄
        /// </summary>
        /// <param name="birthday_utc"></param>
        /// <returns></returns>
        public static int GetAge(DateTime birthday_utc)
        {
            var utc_now = DateTime.UtcNow;

            int age = utc_now.Year - birthday_utc.Year;

            if (birthday_utc > utc_now.AddYears(-age))
            {
                --age;
            }

            return age;
        }

        /// <summary>
        /// 获取系统时区(不返回null)
        /// </summary>
        /// <returns></returns>
        public static TimeZoneInfo[] GetSystemTimeZone()
        {
            var collection = TimeZoneInfo.GetSystemTimeZones();
            return collection.ToArray();
        }

        public static string GetWeek(DateTime? time = null)
        {
            var week = string.Empty;

            switch ((time ?? DateTime.Now).DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    week = "星期日"; break;
                case DayOfWeek.Monday:
                    week = "星期一"; break;
                case DayOfWeek.Tuesday:
                    week = "星期二"; break;
                case DayOfWeek.Wednesday:
                    week = "星期三"; break;
                case DayOfWeek.Thursday:
                    week = "星期四"; break;
                case DayOfWeek.Friday:
                    week = "星期五"; break;
                case DayOfWeek.Saturday:
                    week = "星期六"; break;
                default: throw new ArgumentException("错误的星期");
            }

            return week;
        }

        /// <summary>
        /// 获取友好的时间格式
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string GetFriendlyDateTime(DateTime date)
        {
            if (date == null)
                throw new ArgumentNullException(nameof(date));

            var now = DateTime.Now;
            var cursor = now;
            if (date > cursor)
            {
                var span = date - now;
                if (span.TotalDays < 1)
                {
                    return $"明天{date.Hour}点{date.Minute}分";
                }
                if (span.TotalDays < 2)
                {
                    return $"后天{date.Hour}点{date.Minute}分";
                }
                return date.ToString();
            }
            else
            {
                //计算时间差
                var span = cursor - date;
                //如果是十分钟之内就显示几分钟之前
                if (span.TotalMinutes >= 0 && span.TotalMinutes < 10)
                {
                    if (((int)span.TotalMinutes) == 0) { return "刚刚"; }
                    return $"{(int)Math.Floor(span.TotalMinutes)}分钟前";
                }
                //今天
                cursor = now.Date;
                if (date >= cursor && date < cursor.AddDays(1))
                {
                    return $"今天 {date.Hour}:{date.Minute}";
                }
                //昨天
                cursor = now.Date.AddDays(-1);
                if (date >= cursor && date < cursor.AddDays(1))
                {
                    return $"昨天 {date.Hour}:{date.Minute}";
                }
                //前天
                cursor = now.Date.AddDays(-2);
                if (date >= cursor && date < cursor.AddDays(1))
                {
                    return $"前天 {date.Hour}:{date.Minute}";
                }
                //不是最近三天 看是不是今年
                if (date.Year != now.Year)
                {
                    return $"{date.Year}年{date.Month}月{date.Day}日 {date.Hour}:{date.Minute}";
                }
                else
                {
                    return $"今年{date.Month}月{date.Day}日 {date.Hour}:{date.Minute}";
                }
            }
        }

        /// <summary>
        /// 获取友好的时间格式
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string GetSimpleFriendlyDateTime(DateTime date)
        {
            if (date == null)
                throw new ArgumentNullException(nameof(date));

            var now = DateTime.Now;
            if (date > now)
            {
                var span = date - now;
                if (span.TotalDays < 1)
                {
                    return $"明天{date.Hour}点{date.Minute}分";
                }
                if (span.TotalDays < 2)
                {
                    return $"后天{date.Hour}点{date.Minute}分";
                }
                return date.ToString();
            }
            else
            {
                //计算时间差
                var span = now - date;

                if (span.TotalMinutes < 1)
                {
                    return "刚刚";
                }
                else if (span.TotalHours < 1)
                {
                    return $"{(int)span.TotalMinutes}分钟前";
                }
                else if (span.TotalDays < 1)
                {
                    return $"{(int)span.TotalHours}小时前";
                }
                else if (span.TotalDays < 31)
                {
                    return $"{(int)span.TotalDays}天前";
                }
                else if (span.TotalDays < 365)
                {
                    return $"{(int)(span.TotalDays / 31)}月前";
                }
                else
                {
                    return $"{(int)(span.TotalDays / 365)}年前";
                }
            }
        }
    }
}
