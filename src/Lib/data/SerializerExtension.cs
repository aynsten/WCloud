using Lib.data;
using Lib.ioc;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SerializerExtension
    {
        public static ISerializeProvider ResolveSerializer(this IServiceProvider provider)
        {
            var serializer = provider.Resolve_<ISerializeProvider>();
            return serializer;
        }
    }
}
