using System;
using WCloud.Core.DataSerializer;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SerializerExtension
    {
        public static IDataSerializer ResolveSerializer(this IServiceProvider provider)
        {
            var serializer = provider.Resolve_<IDataSerializer>();
            return serializer;
        }
    }
}
