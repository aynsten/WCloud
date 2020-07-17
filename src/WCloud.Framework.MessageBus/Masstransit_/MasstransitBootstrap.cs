using FluentAssertions;
using Lib.extension;
using Lib.ioc;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WCloud.Framework.MessageBus.Masstransit_
{
    public static class MasstransitBootstrap
    {
        static IBusControl __create_rabbitmq_bus__(IConfiguration config, Action<IRabbitMqBusFactoryConfigurator> option = null)
        {
            var rabbit = config.GetRabbitmqOrThrow();
            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri($"rabbitmq://{rabbit.ServerAndPort}/"), h =>
                {
                    h.Username(rabbit.User);
                    h.Password(rabbit.Password);
                });

                option?.Invoke(cfg);
            });
            return bus;
        }

        static IServiceCollection __add_di__(IServiceCollection collection)
        {
            collection.AddSingleton<IBus>(provider => provider.Resolve_<IBusControl>());
            collection.AddSingleton<IPublishEndpoint>(provider => provider.Resolve_<IBus>());
            collection.AddSingleton<ISendEndpointProvider>(provider => provider.Resolve_<IBus>());

            //message publisher & sender
            collection.AddScoped<IMessagePublisher, MyMasstransitMessagePublisher>();
            collection.AddSingleton<IConsumerStartor, MasstransitConsumerStartor>();

            return collection;
        }

        internal static IServiceCollection AddMasstransitMessageBus(this IServiceCollection collection, IConfiguration config, Type[] consumer_types = null)
        {
            consumer_types ??= new Type[] { };

            var descriptors = consumer_types.Select(x => new
            {
                MessageType = x.__get_message_type__(),
                QueueConfig = x.__get_config__()
            }).ToArray();

            var group = descriptors.GroupBy(x => x.QueueConfig.QueueName).Select(x => new
            {
                QueueName = x.Key,
                MasstransitConsumers = x.Select(c => new
                {
                    c.MessageType,
                    Consumer = typeof(BasicMasstransitConsumer<>).MakeGenericType(c.MessageType)
                }).ToArray()
            }).ToArray();

            var all_masstransit_consumers = group.SelectMany(x => x.MasstransitConsumers).Select(x => x.Consumer).ToArray();

            collection.AddMassTransit(x =>
            {
                foreach (var c in all_masstransit_consumers)
                {
                    //加入ioc
                    x.AddConsumer(c);
                }

                x.AddBus(provider =>
                {
                    var config = provider.ResolveConfig_();
                    var bus = __create_rabbitmq_bus__(config, cfg =>
                    {
                        foreach (var g in group)
                        {
                            cfg.ReceiveEndpoint(g.QueueName, endpoint =>
                            {
                                //endpoint.ExchangeType = queue_config.ExchangeType;
                                //endpoint.AutoDelete = queue_config.AutoDelete;
                                //endpoint.Exclusive = queue_config.Exclusive;
                                //endpoint.Durable = queue_config.Durable;
                                endpoint.PrefetchCount = 3;
                                var consumers = g.MasstransitConsumers.Select(x => x.Consumer).ToArray();
                                foreach (var c in consumers)
                                {
                                    endpoint.ConfigureConsumer(provider, consumerType: c);
                                }
                            });
                        }
                    });
                    return bus;
                });
            });

            __add_di__(collection);

            return collection;
        }

        [Obsolete]
        internal static IServiceCollection AddMasstransitMessageBus_(this IServiceCollection collection, IConfiguration config, Action<IRabbitMqBusFactoryConfigurator> option = null)
        {
            var bus = __create_rabbitmq_bus__(config, cfg =>
            {
                cfg.ReceiveEndpoint("masstransit_test_queue", endpoint =>
                {
                    endpoint.Handler<MyMessage>(async context =>
                    {
                        await Console.Out.WriteLineAsync($"Received: {context.Message.Value}");
                    });
                });

                option?.Invoke(cfg);
            });

            //bus
            collection.AddDisposableSingleInstanceService(new MasstransitBusControlWrapper(bus));
            collection.AddSingleton<IBusControl>(provider => provider.Resolve_<MasstransitBusControlWrapper>().Instance);

            __add_di__(collection);

            return collection;
        }

        [Obsolete]
        class MasstransitBusControlWrapper : ISingleInstanceService<IBusControl>
        {
            private readonly IBusControl _bus;

            public MasstransitBusControlWrapper(IBusControl bus) =>
                this._bus = bus ?? throw new ArgumentNullException(nameof(bus));

            public int DisposeOrder => int.MaxValue;

            public IBusControl Instance => this._bus;

            public void Dispose()
            {
                async Task Stop()
                {
                    await this._bus.StopAsync();
                }

                Task.Run(async () => await Stop()).Wait();
            }
        }

        /// <summary>
        /// 1.查找consumer实现，
        /// 2.注册消费到masstransit
        /// 3.masstransit的消费收到消息后从ioc中找到通用consumer后调用通用consumer
        /// </summary>
        /// <param name="config"></param>
        /// <param name="ass"></param>
        [Obsolete]
        static IRabbitMqBusFactoryConfigurator RegMasstransitBasicConsumer(this IRabbitMqBusFactoryConfigurator config, Type[] all_types)
        {
            all_types.Should().NotBeNull();

            var descriptors = all_types.Select(x => new
            {
                MessageType = x.__get_message_type__(),
                QueueConfig = x.__get_config__()
            }).ToArray();

            var group = descriptors.GroupBy(x => x.QueueConfig.QueueName).Select(x => new
            {
                QueueName = x.Key,
                MessageTypes = x.Select(d => d.MessageType).ToArray()
            }).ToArray();

            foreach (var g in group)
            {
                config.ReceiveEndpoint(g.QueueName, endpoint =>
                {
                    var consumers = g.MessageTypes.Select(x => typeof(BasicMasstransitConsumer<>).MakeGenericType(x)).ToArray();
                    endpoint.AddMasstransitConsumer(consumers);
                });
            }
            return config;
        }

        /// <summary>
        /// 避免一个消息被多次执行
        /// </summary>
        /// <param name="types"></param>
        [Obsolete]
        static void CheckRepeatConsumerOrThrow(Type[] types)
        {
            IEnumerable<(Type ConsumerType, Type MessageType)> flatten(Type t)
            {
                var @interfaces = t.GetInterfaces()
                    .Where(x => x.IsGenericType_(typeof(MassTransit.IConsumer<>))).ToArray();

                var res = @interfaces
                    .Select(x => (ConsumerType: t, MessageType: x.GetGenericArguments().FirstOrDefault()))
                    .Where(x => x.MessageType != null).ToArray();

                return res;
            }

            var maps = types.SelectMany(x => flatten(x)).ToArray();

            var errors = maps.GroupBy(x => x.MessageType)
                .Select(x => new
                {
                    MessageType = x.Key.FullName,
                    ConsumerTypes = x.Select(m => m.ConsumerType.FullName).ToArray()
                })
                .Where(x => x.ConsumerTypes.Count() > 1)
                .ToDictionary(x => x.MessageType, x => x.ConsumerTypes);

            if (errors.Any())
            {
                throw new RepeatConsumeException("存在一个消息对应多个消费的情况")
                {
                    Errors = errors
                };
            }

        }

    }
}
