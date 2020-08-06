using Microsoft.Extensions.DependencyInjection;
using WCloud.Core.MessageBus;

namespace WCloud.Framework.MessageBus.Memory
{
    public static class MemoryExtension
    {
        public static IServiceCollection AddMemoryMeesageBus(this IServiceCollection services)
        {
            services.AddTransient<IMessagePublisher, MemoryPublisher>();
            return services;
        }
    }
}
