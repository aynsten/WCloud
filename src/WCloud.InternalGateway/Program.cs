using Autofac.Extensions.DependencyInjection;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace WCloud.InternalGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildAbpWebHost(args).Build().Run();
        }
        static IHostBuilder BuildAbpWebHost(string[] args) =>
           Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, builder) =>
            {
                var env = context.HostingEnvironment;

                var ocelot_config = "ocelot.Development.json";
                if (env.IsStaging())
                {
                    ocelot_config = "ocelot.Staging.json";
                }
                if (env.IsProduction())
                {
                    ocelot_config = "ocelot.Production.json";
                }

                var path = Path.Combine(env.ContentRootPath, ocelot_config);
                File.Exists(path).Should().BeTrue($"{path} not exist");

                builder.AddJsonFile(path);
                builder.AddEnvironmentVariables().AddCommandLine(args);
            })
            .ConfigureWebHostDefaults(builder =>
            {
                builder.UseStartup<Startup>().UseUrls("http://*:18888");
            })
            .UseAutofac()
            .UseServiceProviderFactory(new AutofacServiceProviderFactory());
    }
}
