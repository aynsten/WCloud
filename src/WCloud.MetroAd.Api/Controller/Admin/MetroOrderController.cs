using FluentAssertions;
using Lib.cache;
using Lib.core;
using Lib.extension;
using Lib.helper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Admin.Message;
using WCloud.Core.Cache;
using WCloud.Core.Helper;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Application.Service;
using WCloud.Member.Authentication.ControllerExtensions;
using WCloud.MetroAd.Order;

namespace WCloud.MetroAd.Api.Controller
{
    [MetroAdServiceRoute("admin")]
    public class AdminMetroOrderController : BasicServiceController<IOrderService, OrderEntity>, IAdminController
    {
        private readonly IStringArraySerializer stringArraySerializer;
        private readonly ICacheKeyManager cacheKeyManager;
        private readonly ICacheProvider cacheProvider;

        public AdminMetroOrderController(IStringArraySerializer stringArraySerializer, ICacheKeyManager cacheKeyManager, ICacheProvider cacheProvider)
        {
            this.stringArraySerializer = stringArraySerializer;
            this.cacheProvider = cacheProvider;
            this.cacheKeyManager = cacheKeyManager;
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> UserOrderCount([FromForm]string data)
        {
            var user_uids = this.JsonToEntity_<string[]>(data);

            var count_res = await this._service.UserOrderCount(user_uids);

            return SuccessJson(count_res);
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> QueryOrderByConditions(
            [FromServices]IUserService userService,
            [FromForm]string order_no,
            [FromForm]string tf_no,
            [FromForm]string user_name,
            [FromForm]string multi_status,
            [FromForm]bool? is_expired,
            [FromForm]bool? has_design,
            [FromForm]DateTime? start_time_utc,
            [FromForm]DateTime? end_time_utc,
            [FromForm]int? page)
        {
            page = this.CheckPage(page);

            var user_uids = new List<string>() { };
            if (ValidateHelper.IsNotEmpty(user_name))
            {
                var matched_users = await userService.GetTopMatchedUsers(user_name, 10);

                if (!matched_users.Any())
                {
                    return GetJsonRes("未找到用户");
                }
                user_uids.AddRange(matched_users.Select(x => x.UID));
            }

            var s = new List<int>();
            if (ValidateHelper.IsNotEmpty(multi_status))
            {
                s.AddList_(this.JsonToEntity_<int[]>(multi_status));
            }
            s = s.Distinct().ToList();

            var data = await this._service.QueryByConditions(
                order_no, tf_no, user_uids.ToArray(), s.ToArray(), is_expired, has_design,
                start_time_utc, end_time_utc, page.Value, this.PageSize);

            data.DataList = await this._service.LoadUsers(data.DataList);
            data.DataList = await this._service.LoadItems(data.DataList);

            var res = data.PagerDataMapper_(x => x);

            return SuccessJson(res);
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> QueryOrderDetail([FromForm]string order_uid)
        {
            var loginadmin = await this.GetLoginAdminAsync();

            order_uid.Should().NotBeNullOrEmpty();

            var order = await this._service.GetByUID(order_uid);

            order.Should().NotBeNull();

            IEnumerable<OrderEntity> data = new[] { order };

            data = await this._service.PrepareOrder(data);
            data = await this._service.LoadUsers(data);
            data = await this._service.LoadApprovers(data);
            data = await this._service.LoadItems(data);
            //data = await this._service.LoadItemsStationInfo(data);
            data = await this._service.LoadDesigns(data);
            data = await this._service.LoadDeployments(data);
            data = await this._service.LoadHisotries(data);
            data = await this._service.LoadAdmins(data);

            var m = data.First();

            var res = m;

            return SuccessJson(res);
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> UpdateOrderStatus([FromForm]string order_uid, [FromForm]int status)
        {
            var loginadmin = await this.GetLoginAdminAsync();

            var olog = new OperationLogMessage(loginadmin).UpdateOrAdd("订单管理", true).WithMessage("修改订单状态")
                .WithExtraData(new { order_uid, status });
            await this.AddOperationLog(olog);

            var data = await this._service.UpdateOrderStatus(order_uid, status);

            await this.AddOrderStatusHistoryLog(data.UID);

            return SuccessJson();
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> AddOrderDesign([FromForm]string data)
        {
            var model = this.JsonToEntity_<DesignImageEntity>(data);

            model.DesignImages ??= new string[] { };
            if (!model.DesignImages.Any())
            {
                return GetJsonRes("至少上传一张图片");
            }

            var loginadmin = await this.GetLoginAdminAsync();

            var olog = new OperationLogMessage(loginadmin).UpdateOrAdd("订单管理", true).WithMessage("添加设计稿").WithExtraData(model);
            await this.AddOperationLog(olog);

            model.DesignImageJson = this.stringArraySerializer.Serialize(model.DesignImages);
            model.DesignerUID = loginadmin.UserID;

            await this._service.AddOrderDesign(model);

            return SuccessJson();
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> AddOrderDeploymentUp([FromForm]string data)
        {
            var model = this.JsonToEntity_<DeployEntity>(data);

            var loginadmin = await this.GetLoginAdminAsync();
            var olog = new OperationLogMessage(loginadmin).UpdateOrAdd("订单管理", true).WithMessage("上刊").WithExtraData(model);
            await this.AddOperationLog(olog);

            model.ImageList ??= new string[] { };

            model.ImageJson = this.stringArraySerializer.Serialize(model.ImageList);
            model.DeployerUID = loginadmin.UserID;
            model.DeployerName = loginadmin.UserName;

            await this._service.AddOrderDeploymentUp(model);

            await this.AddOrderStatusHistoryLog(model.OrderUID);

            return SuccessJson();
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> AddOrderDeploymentDown([FromForm]string data)
        {
            var model = this.JsonToEntity_<DeployEntity>(data);

            var loginadmin = await this.GetLoginAdminAsync();
            var olog = new OperationLogMessage(loginadmin).UpdateOrAdd("订单管理", true).WithMessage("下刊").WithExtraData(model);
            await this.AddOperationLog(olog);

            model.ImageList ??= new string[] { };

            model.ImageJson = this.stringArraySerializer.Serialize(model.ImageList);
            model.DeployerUID = loginadmin.UserID;
            model.DeployerName = loginadmin.UserName;

            var res = await this._service.AddOrderDeploymentDown(model);

            await this.AddOrderStatusHistoryLog(model.OrderUID);

            return SuccessJson();
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> ApproveOrder([FromForm]string order_uid, [FromForm]bool? approved, [FromForm]string comment)
        {
            order_uid.Should().NotBeNullOrEmpty();
            approved.Should().NotBeNull();
            comment ??= string.Empty;
            if (!approved.Value && ValidateHelper.IsEmpty(comment))
            {
                throw new MsgException("请输入拒绝理由");
            }

            var loginadmin = await this.GetLoginAdminAsync();
            var olog = new OperationLogMessage(loginadmin).UpdateOrAdd("订单管理", true).WithMessage("审核订单")
                .WithExtraData(new
                {
                    order_uid,
                    approved,
                    comment
                });
            await this.AddOperationLog(olog);

            await this._service.ApproveOrReject(loginadmin.UserID, order_uid, approved.Value, comment);

            await this.AddOrderStatusHistoryLog(order_uid);

            return SuccessJson();
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> SetPayed([FromForm]string order_uid, [FromForm]int? pay_method,
            [FromForm]string external_payment_no,
            [FromForm]string img, [FromForm]string comment)
        {
            order_uid.Should().NotBeNullOrEmpty();
            pay_method.Should().NotBeNull();
            var imgs = this.JsonToEntity_<string[]>(img);

            var loginadmin = await this.GetLoginAdminAsync();
            var olog = new OperationLogMessage(loginadmin).UpdateOrAdd("订单管理", true).WithMessage("设为已支付").WithExtraData(new
            {
                order_uid,
                pay_method,
                external_payment_no,
                img,
                comment
            });
            await this.AddOperationLog(olog);

            await this._service.SetPayed(loginadmin.UserID, order_uid, pay_method.Value, external_payment_no, comment, imgs);

            await this.AddOrderStatusHistoryLog(order_uid);

            return SuccessJson();
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> CloseOrder([FromForm]string order_uid)
        {
            order_uid.Should().NotBeNullOrEmpty();

            var loginadmin = await this.GetLoginAdminAsync();
            var olog = new OperationLogMessage(loginadmin).UpdateOrAdd("订单管理", true).WithMessage("关闭订单").WithExtraData(new
            {
                order_uid
            });
            await this.AddOperationLog(olog);

            await this._service.CloseOrder(order_uid, "不想要了");

            await this.AddOrderStatusHistoryLog(order_uid);

            return SuccessJson();
        }
    }
}
