using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace WCloud.IM.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildAbpWebHost(args).Build().Run();
            //CreateWebHostBuilder(args).Build().Run();
        }

        static IHostBuilder BuildAbpWebHost(string[] args) =>
           Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(builder =>
            {
                builder.UseStartup<Startup>().UseUrls("http://*:4001");
            })
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddEnvironmentVariables().AddCommandLine(args);
            })
            .UseAutofac()
            .UseServiceProviderFactory(new AutofacServiceProviderFactory());
    }
}
