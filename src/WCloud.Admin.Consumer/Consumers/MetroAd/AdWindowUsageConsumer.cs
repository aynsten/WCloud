using FluentAssertions;
using Lib.extension;
using Lib.ioc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Framework.MessageBus;
using WCloud.MetroAd;
using WCloud.MetroAd.Order;
using WCloud.MetroAd.Shared.Message;
using WCloud.MetroAd.Statistic;

namespace WCloud.Admin.Consumer.Consumers
{
    /// <summary>
    /// 广告位使用率统计数据准备
    /// </summary>
    public class AdWindowUsageConsumer : IMessageConsumer<AdWindowUsageLogMessage>, Lib.core.IFinder
    {
        private readonly IServiceProvider provider;
        public AdWindowUsageConsumer(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public async Task Consume(IMessageConsumeContext<AdWindowUsageLogMessage> context)
        {
            try
            {
                var order_uid = context.Message.OrderUID;
                order_uid.Should().NotBeNullOrEmpty();

                var order_service = provider.Resolve_<IOrderService>();
                var repo = provider.Resolve_<IMetroAdRepository<OrderEntity>>();

                var db = repo.Database;
                var set = db.Set<AdWindowUsageEntity>();

                set.RemoveRange(set.Where(x => x.OrderUID == order_uid));

                await db.SaveChangesAsync();

                var order = await order_service.GetByUID(order_uid);
                order.Should().NotBeNull();
                order = (await order_service.LoadItems(new[] { order })).First();
                order.OrderItems.Should().NotBeNullOrEmpty();

                var datas = this.__flat_data__(order);

                if (datas.Any())
                {
                    set.AddRange(datas);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                provider.Resolve_<ILogger<AdWindowUsageConsumer>>().AddErrorLog(nameof(AdWindowUsageConsumer), e);
            }
        }

        IEnumerable<AdWindowUsageEntity> __flat_data__(OrderEntity data)
        {
            var start = data.AdStartTimeUtc;
            for (var i = 1; i <= data.TotalDays; ++i)
            {
                var offset = i - 1;
                var date = offset > 0 ? start.AddDays(offset) : start;
                foreach (var m in data.OrderItems)
                {
                    var model = new AdWindowUsageEntity()
                    {
                        DateUtc = date,

                        OrderUID = data.UID,
                        LineUID = m.MetroLineUID,
                        StationUID = m.MetroStationUID,
                        StationType = m.MetroStationType,
                        AdWindowUID = m.AdWindowUID,
                        MediaTypeUID = m.MetroStationUID,
                        PriceInCent = m.PriceInCent,
                    };

                    model.InitSelf();

                    yield return model;
                }
            }
        }
    }
}
