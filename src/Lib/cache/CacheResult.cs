using Newtonsoft.Json;
using System;

namespace Lib.cache
{
    /// <summary>
    /// 带是否成功标志位的结果
    /// </summary>
    [Serializable]
    public class CacheResult<T> : IDataContainer<T>
    {
        public CacheResult() { }

        public CacheResult(T data)
        {
            this.Data = data;
            this.Success = true;
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public virtual T Data { get; set; }

        [JsonIgnore]
        public virtual bool Success { get; set; } = false;
    }
}
