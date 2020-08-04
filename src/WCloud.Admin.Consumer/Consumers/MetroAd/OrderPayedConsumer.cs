using FluentAssertions;
using Lib.extension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Framework.MessageBus;
using WCloud.MetroAd;
using WCloud.MetroAd.Finance;
using WCloud.MetroAd.Order;
using WCloud.MetroAd.Shared.Message;

namespace WCloud.Admin.Consumer.Consumers
{
    public class OrderPayedConsumer : IMessageConsumer<OrderPayedMessage>, Lib.core.IFinder
    {
        private readonly IServiceProvider provider;
        public OrderPayedConsumer(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public async Task Consume(IMessageConsumeContext<OrderPayedMessage> context)
        {
            try
            {
                var order_uid = context.Message.OrderUID;

                var repo = provider.Resolve_<IMetroAdRepository<OrderEntity>>();
                var db = repo.Database;
                var set = db.Set<FinanceFlowEntity>();

                if (await set.AsNoTracking().AnyAsync(x => x.OrderUID == order_uid))
                {
                    return;
                }

                var order = await db.Set<OrderEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.UID == order_uid);
                order.Should().NotBeNull();
                order.PayTime.Should().NotBeNull();

                order = (await provider.Resolve_<IOrderService>().LoadUsers(new[] { order })).First();

                var flow = new FinanceFlowEntity()
                {
                    OrderUID = order.UID,
                    OrderNo = order.OrderNo,
                    PriceInCent = order.TotalPriceInCent,
                    FlowDirection = (int)FlowDirectionEnum.In,
                    PayMethod = order.PayMethod,
                    ConsumerName = order.User?.UserName,
                    ConsumerUID = order.UserUID,
                    OrderCreateTimeUtc = order.CreateTimeUtc,
                };

                flow.InitSelf();

                set.Add(flow);

                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                provider.Resolve_<ILogger<OrderPayedConsumer>>().AddErrorLog(nameof(OrderPayedConsumer), e);
            }
        }
    }
}
