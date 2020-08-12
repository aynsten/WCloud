using System;

namespace WCloud.Framework.Database.Abstractions.Entity
{
    public abstract class TimeEntityBase : EntityBase
    {
        public virtual int TimeYear { get; set; }

        public virtual int TimeMonth { get; set; }

        public virtual int TimeDay { get; set; }

        public virtual int TimeHour { get; set; }
    }
}
