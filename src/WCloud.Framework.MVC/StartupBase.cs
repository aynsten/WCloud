using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.AspNetCore.Builder
{
    public abstract class AppStartupBase<T> : AppStartupBase
    {
        protected AppStartupBase(IWebHostEnvironment env, IConfiguration config) : base(env, config) { }

        public abstract void ConfigureContainer(T builder);
    }

    public abstract class AppStartupBase
    {
        protected readonly IWebHostEnvironment _env;
        protected readonly IConfiguration _config;

        protected AppStartupBase(IWebHostEnvironment env, IConfiguration config)
        {
            this._env = env;
            this._config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public abstract void ConfigureServices(IServiceCollection services);

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public abstract void Configure(IApplicationBuilder app, IHostApplicationLifetime lifetime);
    }
}
