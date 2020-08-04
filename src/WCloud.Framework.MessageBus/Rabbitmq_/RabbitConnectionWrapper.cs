using Lib.ioc;
using RabbitMQ.Client;
using System;

namespace WCloud.Framework.MessageBus.Rabbitmq_
{
    public class RabbitConnectionWrapper : ISingleInstanceService
    {
        private readonly IConnectionFactory _factory;
        private readonly IConnection _connection;

        public RabbitConnectionWrapper(IConnectionFactory factory)
        {
            this._factory = factory;
            this._connection = this._factory.CreateConnection();
        }

        public IConnectionFactory Factory => this._factory;
        public IConnection Connection => this._connection;

        public int DisposeOrder => int.MinValue;

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
