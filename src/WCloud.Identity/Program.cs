using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WCloud.Identity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildAbpWebHost(args).Build().Run();
        }

        static IHostBuilder BuildAbpWebHost(string[] args) =>
           Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(builder =>
            {
                builder.UseStartup<Startup>().UseUrls("http://*:5001");
            })
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddEnvironmentVariables().AddCommandLine(args);
            })
            .ConfigureLogging(builder =>
            {
                builder.ClearProviders();
            })
            .UseAutofac()
            .UseServiceProviderFactory(new AutofacServiceProviderFactory());
    }
}
