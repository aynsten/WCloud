using System;

namespace WCloud.Core.Cache
{
    [Serializable]
    public class CacheBundle
    {
        public virtual string[] TokenUID { get; set; }

        public virtual string[] UserUID { get; set; }
    }

}
