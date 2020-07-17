using Lib.extension;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace WCloud.Framework.MessageBus.Rabbitmq_
{
    public abstract class RabbitMqConfiguationBuilder
    {
        public abstract void Build();

        void test()
        {
            var builder = new RabbitMqExchangeBuilder(null);

            builder = builder.ExchangeName("public-topic").ExchangeType(ExchangeTypeEnum.topic)
                .RouteToExchange(x => x.ExchangeName("order-ex").RouteToQueue(c => c.QueueName("x")))
                .RouteToQueue(x => x.QueueName("order"));

            builder.Build();
        }
    }

    public class RabbitMqQueueBuilder : RabbitMqConfiguationBuilder
    {
        private readonly IModel _model;

        public RabbitMqQueueBuilder(IModel model)
        {
            this._model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public string Name { get; private set; }

        public QueueOption Option { get; private set; }

        public RabbitMqQueueBuilder QueueName(string name)
        {
            this.Name = name;
            return this;
        }

        public RabbitMqQueueBuilder QueueOption(QueueOption option)
        {
            this.Option = option;
            return this;
        }

        public override void Build()
        {
            throw new NotImplementedException();
        }
    }

    public class RabbitMqExchangeBuilder : RabbitMqConfiguationBuilder
    {
        private readonly IModel _model;
        private readonly List<RabbitMqExchangeBuilder> _routeToExchanges = new List<RabbitMqExchangeBuilder>();
        private readonly List<RabbitMqQueueBuilder> _routeToQueues = new List<RabbitMqQueueBuilder>();

        public RabbitMqExchangeBuilder(IModel model)
        {
            this._model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public override void Build()
        {
            var list = new List<RabbitMqExchangeBuilder>();

            void _build_exchange(RabbitMqExchangeBuilder _exchange)
            {
                var _this = _exchange;
                list.AddOnceOrThrow(_this);

                //create this exchange
                this._model.CreateExchange(null, default, null);

                foreach (var child_queue in this._routeToQueues)
                {
                    child_queue.Build();

                    //bind _this to child_queue
                    this._model.RouteFromExchangeToQueue("", "", "", null);
                }

                foreach (var child_exchange in this._routeToExchanges)
                {
                    _build_exchange(child_exchange);

                    //bind _this to child_exchange
                    this._model.RouteFromExchangeToExchange(null, null, null, null);
                }
            }
            //构建exchange之间的绑定
            _build_exchange(this);
        }

        public RabbitMqExchangeBuilder ExchangeName(string name)
        {
            return this;
        }

        public RabbitMqExchangeBuilder ExchangeType(ExchangeTypeEnum type)
        {
            return this;
        }

        public RabbitMqExchangeBuilder ExchangeOption(ExchangeOption option)
        {
            return this;
        }

        public RabbitMqExchangeBuilder RouteToExchange(Func<RabbitMqExchangeBuilder, RabbitMqExchangeBuilder> exchange_config)
        {
            if (exchange_config == null)
                throw new ArgumentNullException(nameof(exchange_config));

            var exchange = new RabbitMqExchangeBuilder(this._model);
            exchange = exchange_config.Invoke(exchange);

            this._routeToExchanges.Add(exchange);

            return this;
        }

        public RabbitMqExchangeBuilder RouteToQueue(Func<RabbitMqQueueBuilder, RabbitMqQueueBuilder> queue_config)
        {
            if (queue_config == null)
                throw new ArgumentNullException(nameof(queue_config));

            var queue = new RabbitMqQueueBuilder(this._model);
            queue = queue_config.Invoke(queue);

            this._routeToQueues.Add(queue);

            return this;
        }
    }
}
