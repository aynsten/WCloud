using System;

namespace WCloud.Framework.Database.Abstractions.Entity
{
    public abstract class TimeEntityBase : EntityBase
    {
        public virtual int TimeYear { get; set; }

        public virtual int TimeMonth { get; set; }

        public virtual int TimeDay { get; set; }

        public virtual int TimeHour { get; set; }

        public override void Init(string flag = null, DateTime? utc_time = null)
        {
            base.Init(flag, utc_time);

            this.TimeYear = this.CreateTimeUtc.Year;
            this.TimeMonth = this.CreateTimeUtc.Month;
            this.TimeDay = this.CreateTimeUtc.Day;
            this.TimeHour = this.CreateTimeUtc.Hour;
        }
    }
}
