using Lib.helper;

namespace WCloud.Core.Cache
{
    public static class CacheExtension
    {
        /// <summary>
        /// 缓存key加前缀
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string WithPrefix(this string key, string prefix = null)
        {
            var res = ValidateHelper.IsNotEmpty(prefix) ? $"{prefix}.{key}" : key;
            return res;
        }

        public static CacheResult<T> OK<T>(this CacheResult<T> data)
        {
            data.Success = true;
            return data;
        }
    }
}
