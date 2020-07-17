# 消息总线

### 思路：
抽象出通用的consumer和publisher

masstransit/redis/rabbitmq的具体实现收到消息后
从ioc容器中找到符合条件的consumer，依次调用。