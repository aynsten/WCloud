using System;
using System.Collections.Generic;
using System.Linq;

namespace WCloud.Framework.Jobs.Quartz_
{
    public class JobTriggerModel
    {
        public virtual string TriggerName { get; set; }

        public virtual string TriggerGroup { get; set; }

        public virtual string JobStatus { get; set; }

        public virtual DateTime StartTime { get; set; }

        public virtual DateTime? PreTriggerTime { get; set; }

        public virtual DateTime? NextTriggerTime { get; set; }
    }

    public class JobModel
    {
        public virtual string JobName { get; set; }

        public virtual string JobGroup { get; set; }

        public virtual bool IsRunning { get; set; }

        public virtual List<JobTriggerModel> Triggers { get; set; }

        public virtual DateTime? PreTriggerTime
        {
            get
            {
                return this.Triggers?.Select(x => x.PreTriggerTime).Where(x => x != null).Min();
            }
        }

        public virtual DateTime? NextTriggerTime
        {
            get
            {
                return this.Triggers?.Select(x => x.NextTriggerTime).Where(x => x != null).Min();
            }
        }
    }
}
