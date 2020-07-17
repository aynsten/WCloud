using RabbitMQ.Client;

namespace WCloud.Framework.MessageBus.Rabbitmq_
{
    class test
    {
        public test()
        {
            /*
             创建topic用于分发消息
             创建fanout用于广播消息
             部分需要广播的消息通过topic的routing key转发到fanout exchange里
             */

            var factory = new ConnectionFactory();
            var con = factory.CreateConnection();

            var channel = con.CreateModel();

            channel.ExchangeDeclare("shared-topic", "topic", durable: true, autoDelete: false);
            channel.ExchangeDeclare("user-group-change-broadcast", "fanout", durable: true, autoDelete: false);

            channel.ExchangeBind(destination: "user-group-change-broadcast", source: "shared-topic", routingKey: "user-group-change");



            var queue = channel.QueueDeclare(exclusive: true);
            channel.QueueBind(queue.QueueName, "user-group-change-broadcast", null);

            channel.BasicConsume(queue.QueueName, autoAck: true, null);

            channel.BasicPublish("shared-topic", "user-group-change", body: new byte[] { });
        }
    }
}
