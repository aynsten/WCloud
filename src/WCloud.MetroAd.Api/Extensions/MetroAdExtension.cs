using FluentAssertions;
using Lib.ioc;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WCloud.Admin.Message;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Framework.MessageBus;
using WCloud.MetroAd.Message;

namespace WCloud.MetroAd
{
    public static class MetroAdExtension
    {
        public static async Task AddOperationLog(this ControllerBase controller, OperationLogMessage log)
        {
            log.Should().NotBeNull();
            //防止消费延迟导致乱序,事实上查询哪里要改，需要按照时间排序
            log.InitSelf();

            var publisher = controller.HttpContext.RequestServices.Resolve_<IMessagePublisher>();
            await publisher.PublishAsync(log);
        }

        public static async Task AddOrderStatusHistoryLog(this ControllerBase controller, string order_uid)
        {
            order_uid.Should().NotBeNullOrEmpty();

            var log = new OrderStatusChangedMessage()
            {
                OrderUID = order_uid,
            };
            log.InitSelf();

            var publisher = controller.HttpContext.RequestServices.Resolve_<IMessagePublisher>();
            await publisher.PublishAsync(log);
        }
    }
}
