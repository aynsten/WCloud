using Quartz;
using System;

namespace WCloud.Framework.Jobs.Quartz_
{
    public static class TriggerBuilderExtension
    {
        public static TriggerBuilder TriggerWithCron_(this TriggerBuilder builder, string cron, string name, string group = null)
        {
            builder = builder.WithIdentity_(name, group).WithSchedule(CronScheduleBuilder.CronSchedule(cron));
            return builder;
        }

        public static TriggerBuilder TriggerDaily_(this TriggerBuilder builder, int hour, int minute, string name, string group = null)
        {
            builder = builder.WithIdentity_(name, group).WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(hour, minute));
            return builder;
        }

        public static TriggerBuilder TriggerMonthly_(this TriggerBuilder builder, int dayOfMonth, int hour, int minute, string name, string group = null)
        {
            builder = builder.WithIdentity_(name, group).WithSchedule(CronScheduleBuilder.MonthlyOnDayAndHourAndMinute(dayOfMonth, hour, minute));
            return builder;
        }

        public static TriggerBuilder TriggerWeekly_(this TriggerBuilder builder, DayOfWeek dayOfWeek, int hour, int minute, string name, string group = null)
        {
            builder = builder.WithIdentity_(name, group).WithSchedule(CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(dayOfWeek, hour, minute));
            return builder;
        }

        public static TriggerBuilder TriggerInterval_(this TriggerBuilder builder,
            TimeSpan interval, string name, string group = null, DateTimeOffset? start = null, int? repeat_count = null)
        {
            builder = builder.WithIdentity_(name, group);
            builder = builder.WithSimpleSchedule(x =>
            {
                x = x.WithIntervalInSeconds((int)interval.TotalSeconds);
                if (repeat_count == null)
                    x = x.RepeatForever();
                else
                    x = x.WithRepeatCount(repeat_count.Value);
            });
            builder = builder.Start_(start);
            return builder;
        }

    }
}
