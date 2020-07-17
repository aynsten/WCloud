using FluentAssertions;
using Lib.cache;
using Lib.extension;
using Lib.helper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Admin.Message;
using WCloud.Core.Cache;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Authentication.ControllerExtensions;
using WCloud.MetroAd.Order;

namespace WCloud.MetroAd.Api.Controller
{
    [MetroAdServiceRoute]
    public class OrderController : WCloudBaseController, IUserController
    {
        private readonly IOrderService _service;
        private readonly ICacheProvider cacheProvider;
        private readonly ICacheKeyManager cacheKeyManager;

        public OrderController(IOrderService _service, ICacheProvider cacheProvider, ICacheKeyManager cacheKeyManager)
        {
            this._service = _service;
            this.cacheKeyManager = cacheKeyManager;
            this.cacheProvider = cacheProvider;
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> PlaceOrder([FromForm]string data)
        {
            var model = this.JsonToEntity_<OrderEntity>(data);
            if (ValidateHelper.IsEmpty(model.CustomerDemand))
            {
                return GetJsonRes("用户需求不能为空");
            }
            if (ValidateHelper.IsEmpty(model.ImageList))
            {
                return GetJsonRes("请上传用户需求图");
            }
            if (ValidateHelper.IsEmpty(model.OrderItems))
            {
                return GetJsonRes("请至少选择一个广告位");
            }

            var loginuser = await this.GetLoginUserAsync();

            var olog = new OperationLogMessage(loginuser).UpdateOrAdd("订单", false).WithMessage("创建订单").WithExtraData(model);
            await this.AddOperationLog(olog);

            model.UserUID = loginuser.UserID;

            var order_res = await this._service.PlaceOrder(model);

            order_res.ThrowIfNotSuccess();

            await this.AddOrderStatusHistoryLog(order_res.Data.UID);

            var res = new
            {
                order_res.Data.UID,
                order_res.Data.OrderNo,
            };

            return SuccessJson(res);
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> UpdateOrder([FromForm]string data)
        {
            var model = this.JsonToEntity_<OrderEntity>(data);
            model.UID.Should().NotBeNullOrEmpty();
            if (ValidateHelper.IsEmpty(model.CustomerDemand))
            {
                return GetJsonRes("用户需求不能为空");
            }
            if (ValidateHelper.IsEmpty(model.ImageList))
            {
                return GetJsonRes("请上传用户需求图");
            }
            if (ValidateHelper.IsEmpty(model.OrderItems))
            {
                return GetJsonRes("请至少选择一个广告位");
            }

            var loginuser = await this.GetLoginUserAsync();

            var olog = new OperationLogMessage(loginuser).UpdateOrAdd("订单", true).WithMessage("修改订单").WithExtraData(model);
            await this.AddOperationLog(olog);

            model.UserUID = loginuser.UserID;

            var is_mine = await this._service.IsMyOrder(model.UID, loginuser.UserID);
            is_mine.Should().BeTrue();

            var order_res = await this._service.UpdateOrder(model);

            order_res.ThrowIfNotSuccess();

            await this.AddOrderStatusHistoryLog(order_res.Data.UID);

            var res = new
            {
                order_res.Data.UID,
                order_res.Data.OrderNo,
            };

            return SuccessJson(res);
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> SetPaymentPending([FromForm]string order_uid)
        {
            var loginuser = await this.GetLoginUserAsync();

            var olog = new OperationLogMessage(loginuser).UpdateOrAdd("订单", true).WithMessage("支付成功，冻结订单支付").WithExtraData(new
            {
                order_uid
            });
            await this.AddOperationLog(olog);

            var is_mine = await this._service.IsMyOrder(order_uid, loginuser.UserID);

            is_mine.Should().BeTrue();

            await this._service.SetPaymentPending(order_uid);

            return SuccessJson();
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> SetAsPayedWhenFree([FromForm]string order_uid)
        {
            order_uid.Should().NotBeNullOrEmpty();

            var loginuser = await this.GetLoginUserAsync();

            var olog = new OperationLogMessage(loginuser).UpdateOrAdd("订单", true).WithMessage("免费订单自动付款").WithExtraData(new
            {
                order_uid
            });
            await this.AddOperationLog(olog);

            var is_mine = await this._service.IsMyOrder(order_uid, loginuser.UserID);
            is_mine.Should().BeTrue();

            await this._service.SetAsPayedWhenFree(order_uid);

            await this.AddOrderStatusHistoryLog(order_uid);

            return SuccessJson();
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> MyOrder(
            [FromForm]int? min_id,
            [FromForm]string multi_status)
        {
            var loginuser = await this.GetLoginUserAsync();

            var s = new List<int>();
            if (ValidateHelper.IsNotEmpty(multi_status))
            {
                s.AddList_(this.JsonToEntity_<int[]>(multi_status));
            }
            s = s.Distinct().ToList();

            var data = await this._service.QueryByMyOrders(loginuser.UserID, s.ToArray(), min_id, this.PageSize);

            data = await this._service.LoadItems(data);

            var res = data.Select(x => x);

            return SuccessJson(res);
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> MyOrderDetail([FromForm]string order_uid)
        {
            order_uid.Should().NotBeNullOrEmpty();

            var loginuser = await this.GetLoginUserAsync();

            var order = await this._service.GetByUID(order_uid);

            order.Should().NotBeNull();
            order.UserUID.Should().Be(loginuser.UserID);

            IEnumerable<OrderEntity> data = new[] { order };

            data = await this._service.PrepareOrder(data);
            data = await this._service.LoadItems(data);
            //data = await this._service.LoadItemsStationInfo(data, load_line: false, load_station: false, load_adwindow: true, load_media_type: false);
            data = await this._service.LoadDesigns(data);
            data = await this._service.LoadDeployments(data);

            var m = data.First();

            var res = m;

            return SuccessJson(res);
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> OrderBasicInfo([FromForm]string order_uid)
        {
            order_uid.Should().NotBeNullOrEmpty();

            var loginuser = await this.GetLoginUserAsync();

            var order = await this._service.GetByUID(order_uid);

            order.Should().NotBeNull();
            order.UserUID.Should().Be(loginuser.UserID);

            IEnumerable<OrderEntity> data = new[] { order };

            data = await this._service.PrepareOrder(data);
            data = await this._service.LoadItems(data);

            var m = data.First();

            var res = m;

            return SuccessJson(res);
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> MyOrderCount([FromForm]string data)
        {
            var status = this.JsonToEntity_<int[]>(data);

            var loginuser = await this.GetLoginUserAsync();

            var key = this.cacheKeyManager.OrderCount(loginuser.UserID);

            var count_data = await this.cacheProvider.GetOrSetAsync_(key,
                () => this._service.QueryOrderCountGroupByStatus(loginuser.UserID, status),
                TimeSpan.FromMinutes(10));

            count_data ??= new Dictionary<int, int>();

            var res = count_data.Select(x => new
            {
                Status = x.Key,
                Count = x.Value
            }).ToArray();

            return SuccessJson(res);
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> ConfirmDesign([FromForm]string order_uid)
        {
            order_uid.Should().NotBeNullOrEmpty();
            var loginuser = await this.GetLoginUserAsync();

            var olog = new OperationLogMessage(loginuser).UpdateOrAdd("订单", true).WithMessage("确认设计").WithExtraData(new
            {
                order_uid
            });
            await this.AddOperationLog(olog);

            await this._service.ConfirmDesign(loginuser.UserID, order_uid);

            await this.AddOrderStatusHistoryLog(order_uid);

            return SuccessJson();
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> CloseOrder([FromForm]string order_uid)
        {
            order_uid.Should().NotBeNullOrEmpty();
            var loginuser = await this.GetLoginUserAsync();

            var olog = new OperationLogMessage(loginuser).UpdateOrAdd("订单", true).WithMessage("关闭订单").WithExtraData(new
            {
                order_uid
            });
            await this.AddOperationLog(olog);

            var mine = await this._service.IsMyOrder(order_uid, loginuser.UserID);
            mine.Should().BeTrue();

            await this._service.CloseOrder(order_uid, "用户主动关闭，不想要了");

            await this.AddOrderStatusHistoryLog(order_uid);

            return SuccessJson();
        }
    }
}
