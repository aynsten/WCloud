using Microsoft.Extensions.DependencyInjection;

namespace WCloud.Framework.Elasticsearch
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
