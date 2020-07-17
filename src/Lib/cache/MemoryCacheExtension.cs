using Microsoft.Extensions.Caching.Memory;

namespace Lib.cache
{
    public static class MemoryCacheExtension
    {
        public static void NotRecommend(this IMemoryCache cache) { }
    }
}
