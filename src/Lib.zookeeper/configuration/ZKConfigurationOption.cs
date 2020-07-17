using System;
namespace Lib.zookeeper.configuration
{
    public class ZKConfigurationOption:Lib.core.OptionBase
    {
        public string BasePath { get; set; }

        public int? MaxDeep { get; set; }

        public override void Valid()
        {
            base.Valid();
        }
    }
}
