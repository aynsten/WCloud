using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace WCloud.Migration
{
    class Program
    {
        public static void Main(string[] args)
        {
            BuildAbpWebHost(args).Build().Run();
        }

        static IHostBuilder BuildAbpWebHost(string[] args) =>
           Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(builder =>
            {
                builder.UseStartup<Startup>();
            })
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddEnvironmentVariables().AddCommandLine(args);
            })
            .UseAutofac()
            .UseServiceProviderFactory(new AutofacServiceProviderFactory());
    }
}
