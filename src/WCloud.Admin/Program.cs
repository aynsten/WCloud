using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace WCloud.Admin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildAbpWebHost(args).Build().Run();
            //CreateWebHostBuilder(args).Build().Run();
        }

        static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, builder) =>
            {
                //builder.AddSharedAppSetting(context.HostingEnvironment);
                builder.AddEnvironmentVariables().AddCommandLine(args);
            })
            .UseStartup<Startup>()
            .UseUrls("http://*:5000");

        static IHostBuilder BuildAbpWebHost(string[] args) =>
           Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(builder =>
            {
                builder.UseStartup<Startup>().UseUrls("http://*:5000");
            })
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddEnvironmentVariables().AddCommandLine(args);
            })
            .UseAutofac()
            .UseServiceProviderFactory(new AutofacServiceProviderFactory());
    }
}
