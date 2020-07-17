using FluentAssertions;
using System;

namespace WCloud.Framework.Jobs
{
    [AttributeUsage(AttributeTargets.Class)]
    public class JobConfigurationAttribute : Attribute
    {
        public string JobId { get; set; }
        public string CronExpression { get; set; }

        public void ValidateOrThrow()
        {
            this.CronExpression.Should().NotBeNullOrEmpty();
        }
    }
}
