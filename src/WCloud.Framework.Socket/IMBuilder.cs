using Microsoft.Extensions.DependencyInjection;

namespace WCloud.Framework.Socket
{
    public class IMBuilder
    {
        public IServiceCollection Services { get; }

        public IMBuilder(IServiceCollection services)
        {
            this.Services = services;
        }
    }
}
