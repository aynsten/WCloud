using System;
using Volo.Abp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (var application = AbpApplicationFactory.Create<AppModule>(options =>
                {
                    options.Configuration.FileName = "appsettings.json";
                    options.Configuration.CommandLineArgs = args;

                    options.UseAutofac(); //Autofac integration
                    //options.Services.AddLogging(builder => { });
                    //options.Services.ReplaceConfiguration();
                }))
                {
                    application.Initialize();

                    Console.ReadLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
    }
}
