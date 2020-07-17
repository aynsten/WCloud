using System;
using System.Diagnostics;

namespace Lib.extension
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// 小时
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static TimeSpan Hours_(this int val) => TimeSpan.FromHours(val);

        /// <summary>
        /// 分钟
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static TimeSpan Minutes_(this int val) => TimeSpan.FromMinutes(val);

        /// <summary>
        /// 秒
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static TimeSpan Seconds_(this int val) => TimeSpan.FromSeconds(val);

        /// <summary>
        /// 毫秒
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static TimeSpan Milliseconds_(this int val) => TimeSpan.FromMilliseconds(val);

        /// <summary>
        /// 计算当月的总天数
        /// </summary>
        public static int DaysOfThisMonth(this DateTime time)
        {
            var border = time.GetMonthBorder();
            var days = (border.end - border.start).TotalDays;
            Debug.Assert((int)days == days, "每月的天数应该是整数");
            return (int)Math.Ceiling(days);
        }

        /// <summary>
        /// 获取每年开始结束
        /// </summary>
        public static (DateTime start, DateTime end) GetYearBorder(this DateTime time)
        {
            var start = new DateTime(time.Year, 1, 1).Date;

            return (start, start.AddYears(1));
        }

        /// <summary>
        /// 获取每月开始结束
        /// </summary>
        public static (DateTime start, DateTime end) GetMonthBorder(this DateTime time)
        {
            var start = new DateTime(time.Year, time.Month, 1).Date;

            return (start, start.AddMonths(1));
        }

        /// <summary>
        /// 获取当前周的开始结束
        /// </summary>
        public static (DateTime start, DateTime end) GetWeekBorder(this DateTime time, DayOfWeek weekStart = DayOfWeek.Monday)
        {
            var date = time.Date;
            while (true)
            {
                if (date.DayOfWeek == weekStart)
                {
                    break;
                }
                date = date.AddDays(-1);
            }
            return (date, date.AddDays(7));
        }

        /// <summary>
        /// 获取一天的开始和第二天的开始
        /// </summary>
        public static (DateTime start, DateTime end) GetDateBorder(this DateTime dateTime)
        {
            var date = dateTime.Date;
            return (date, date.AddDays(1));
        }

        /// <summary>
        /// 判断是否是同一天
        /// </summary>
        public static bool IsSameDay(this DateTime dateTime, DateTime time)
        {
            var border = dateTime.GetDateBorder();
            return border.start <= time && time < border.end;
        }
    }
}
