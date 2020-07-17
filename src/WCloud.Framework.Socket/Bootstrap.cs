using Lib.extension;
using Lib.ioc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using System.Threading;
using WCloud.Framework.Socket.Connection;
using WCloud.Framework.Socket.Handler;
using WCloud.Framework.Socket.Persistence;
using WCloud.Framework.Socket.RegistrationCenter;
using WCloud.Framework.Socket.Transport;
using WCloud.Framework.Socket.UserContext;

namespace WCloud.Framework.Socket
{
    public static class Bootstrap
    {
        public static IMBuilder AddIMServer(this IServiceCollection services)
        {
            services.AddTransient<IMessageSerializer, DefaultMessageSerializer>();
            //下面应该放到别的扩展方法里
            services.AddSingleton<IRedisDatabaseSelector, RedisDatabaseSelector>();
            services.AddSingleton<IRedisKeyManager, DefaultRedisKeyManager>();

            var handlers = new Assembly[] { typeof(IMBuilder).Assembly }.GetAllTypes()
                .Where(x => x.IsNormalClass())
                .Where(x => x.IsAssignableTo_<IMessageHandler>()).ToArray();
            foreach (var h in handlers)
            {
                services.AddSingleton(typeof(IMessageHandler), h);
            }

            return new IMBuilder(services);
        }

        public static IMBuilder AddDefaultHubProvider(this IMBuilder builder)
        {
            builder.Services.AddSingleton(provider => new WsServer(provider, $"{System.Net.Dns.GetHostName()}_"));
            builder.Services.AddSingleton<IWsServer>(provider => provider.Resolve_<WsServer>());
            return builder;
        }

        public static IMBuilder AddRedisRegistrationCenter(this IMBuilder builder)
        {
            builder.Services.AddSingleton<IRegistrationProvider, RedisRegistrationProvider>();
            return builder;
        }

        public static IMBuilder AddRedisTransportProvider(this IMBuilder builder)
        {
            builder.Services.AddSingleton<ITransportProvider, RedisTransportProvider>();
            return builder;
        }

        public static IMBuilder AddRedisPersistenceProvider(this IMBuilder builder)
        {
            builder.Services.AddSingleton<IPersistenceProvider, RedisPersistenceProvider>();
            return builder;
        }

        public static IMBuilder AddDefaultUserContextProvider(this IMBuilder builder)
        {
            builder.Services.AddSingleton<IUserGroups, TestUserGroups>();
            return builder;
        }

        public static IApplicationBuilder UseWebSocketEndpoint(this IApplicationBuilder app, string path)
        {
            return UseWebSocketEndpoint<IWsServer>(app, path);
        }

        public static IApplicationBuilder UseWebSocketEndpoint<T>(this IApplicationBuilder app, string path) where T : IWsServer
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == path)
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var me = context.Request.Query["me"];
                        var device = context.Request.Query["device"];
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();

                        var client = new WsClient(webSocket)
                        {
                            SubjectID = me,
                            DeviceType = device,
                            ConnectionID = context.Connection.Id
                        };

                        var server = context.RequestServices.Resolve_<T>();

                        var connection = new WsConnection(context.RequestServices, server, client);

                        await connection.StartReceiveMessageLoopAsync(CancellationToken.None);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }
            });
            app.ApplicationServices.Resolve_<T>().Start();
            return app;
        }
    }
}
