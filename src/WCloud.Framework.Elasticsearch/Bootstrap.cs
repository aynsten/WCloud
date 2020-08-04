using Microsoft.Extensions.DependencyInjection;

namespace Lib.elasticsearch
{
    public static class ESBootstrap
    {
        public static IServiceCollection UseElasticsearch(this IServiceCollection collection, 
            string servers, bool debug = false)
        {
            return collection;
        }
    }
}
