using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Net;
using System.Threading.Tasks;

namespace WCloud.Framework.Orleans_
{
    public interface IHello : Orleans.IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);
    }

    public class HelloGrain : Orleans.Grain, IHello
    {
        public Task<string> SayHello(string greeting)
        {
            return Task.FromResult($"You said: '{greeting}', I say: Hello!");
        }
    }

    internal class Test
    {
        /// <summary>
        /// http://dotnet.github.io/orleans/Documentation/tutorials_and_samples/tutorial_1.html
        /// </summary>
        /// <returns></returns>
        private static async Task DoClientWork(IClusterClient client)
        {
            // example of calling grains from the initialized client
            var friend = client.GetGrain<IHello>(0);
            var response = await friend.SayHello("Good morning, my friend!");
            Console.WriteLine("\n\n{0}\n\n", response);
        }

        private static async Task<ISiloHost> StartSilo()
        {
            // define the cluster configuration
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "HelloWorldApp";
                })
                .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(HelloGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => { });

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }

        private static async Task<IClusterClient> StartClientWithRetries()
        {
            var client = new ClientBuilder()
                   .UseLocalhostClustering()
                   .Configure<ClusterOptions>(options =>
                   {
                       options.ClusterId = "dev";
                       options.ServiceId = "HelloWorldApp";
                   })
                   .ConfigureLogging(logging => { })
                   .Build();

            await client.Connect();
            Console.WriteLine("Client successfully connect to silo host");
            return client;
        }
    }
}
