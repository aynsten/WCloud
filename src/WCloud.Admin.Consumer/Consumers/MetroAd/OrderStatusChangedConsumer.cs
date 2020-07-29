using FluentAssertions;
using Lib.cache;
using Lib.extension;
using Lib.ioc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core.Cache;
using WCloud.Framework.MessageBus;
using WCloud.MetroAd;
using WCloud.MetroAd.Message;
using WCloud.MetroAd.Order;
using WCloud.MetroAd.Shared.Message;

namespace WCloud.Admin.Consumer.Consumers
{
    public class OrderStatusChangedConsumer : IMessageConsumer<OrderStatusChangedMessage>, Lib.core.IFinder
    {
        private readonly IServiceProvider provider;
        public OrderStatusChangedConsumer(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public async Task Consume(IMessageConsumeContext<OrderStatusChangedMessage> context)
        {
            try
            {
                var data = context.Message;
                data.Should().NotBeNull();
                data.UID.Should().NotBeNullOrEmpty("先init");
                data.OrderUID.Should().NotBeNullOrEmpty("订单号不存在");

                var repo = provider.Resolve_<IMetroAdRepository<OrderHistoryEntity>>();
                var db = repo.Database;

                var order = await db.Set<OrderEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.UID == data.OrderUID);
                order.Should().NotBeNull();

                //删除订单数量统计的缓存
                var cacheKeyManager = provider.Resolve_<ICacheKeyManager>();
                var key = cacheKeyManager.OrderCount(order.UserUID);
                await provider.Resolve_<ICacheProvider>().RemoveAsync(key);

                data.Status = order.Status;

                var set = db.Set<OrderHistoryEntity>();

                var previous_data = await set.AsNoTracking()
                    .Where(x => x.OrderUID == data.OrderUID)
                    .OrderByDescending(x => x.CreateTimeUtc)
                    .FirstOrDefaultAsync();

                if (previous_data == null || previous_data.Status != data.Status)
                {
                    var publisher = provider.Resolve_<IMessagePublisher>();
                    if (data.Status == (int)OrderStatusEnum.待设计)
                    {
                        //添加财务流水
                        await publisher.PublishAsync(new OrderPayedMessage() { OrderUID = data.OrderUID });
                    }
                    if (data.Status == (int)OrderStatusEnum.交易完成)
                    {
                        //完成
                        await publisher.PublishAsync(new OrderFinishedMessage() { OrderUID = data.OrderUID });
                        //发消息通知创建广告位用量统计数据
                        await publisher.PublishAsync(new AdWindowUsageLogMessage() { OrderUID = data.OrderUID });
                    }

                    set.Add(data);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                provider.Resolve_<ILogger<OrderStatusChangedConsumer>>().AddErrorLog(nameof(OrderStatusChangedConsumer), e);
            }
        }
    }
}
