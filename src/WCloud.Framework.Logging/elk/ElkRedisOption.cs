using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace WCloud.Framework.Logging.elk
{
    public class ElkRedisOption : Lib.core.OptionBase
    {
        public string Key { get; set; }
        public int Database { get; set; }
        public LogLevel? MinLevel { get; set; }
        public LogLevel? MaxLevel { get; set; }

        public override void Valid()
        {
            this.Key.Should().NotBeNullOrEmpty("elk pipline redis key");
            this.Database.Should().BeGreaterOrEqualTo(0, "elk pipline redis database");
        }
    }
}
