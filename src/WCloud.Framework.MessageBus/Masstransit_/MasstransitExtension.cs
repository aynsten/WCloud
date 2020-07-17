using MassTransit;
using MassTransit.RabbitMqTransport;
using System;
using System.Collections.Generic;

namespace WCloud.Framework.MessageBus.Masstransit_
{
    public static class MasstransitExtension
    {
        [Obsolete]
        public static void AddMasstransitConsumer(this IRabbitMqReceiveEndpointConfigurator endpoint, IEnumerable<Type> consumers)
        {
            foreach (var c in consumers)
            {
                /*
                 * http://masstransit-project.com/MassTransit/usage/message-consumers.html#connecting-an-existing-consumer-instance
                 While using a consumer instance per message is highly suggested, 
                 it is possible to connect an existing consumer instance which will be called for every message. 
                 The consumer must be thread-safe, as the Consume method will be called from multiple threads simultaneously.
                 To connect an existing instance, see the example below.
                 */

                object __factory__(Type t)
                {
                    var res = Activator.CreateInstance(t);
                    return res;
                }

                //https://github.com/MassTransit/MassTransit/issues/1502
                //这里的consumerFactory只有来消息了才会调用
                endpoint.Consumer(consumerType: c, consumerFactory: __factory__);

                //mass是通过范型类型来配置mq的，如果范型是个接口那么就注册失败了。
                //但是很没道理的是注册失败为啥不抛异常呢？
                //endpoint.Consumer<IConsumer>(consumerFactoryMethod: () => null);
            }
        }
    }
}
