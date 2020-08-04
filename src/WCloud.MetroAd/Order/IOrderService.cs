using Lib.helper;
using Lib.ioc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Service;

namespace WCloud.MetroAd.Order
{
    public interface IOrderService : IBasicService<OrderEntity>, IAutoRegistered
    {
        Task<UserOrderGroup[]> UserOrderCount(string[] user_uids);
        Task<IEnumerable<OrderEntity>> LoadAdmins(IEnumerable<OrderEntity> list);

        Task<OrderEntity> ConfirmDesign(string user_uid, string order_uid);
        Task ApproveOrReject(string approver_uid, string order_uid, bool approved, string comment);

        Task<OrderEntity> GetOrderByNo(string order_no);
        Task UpdatePayCounter(string order_uid);

        Task SetPaymentPending(string order_uid);
        Task SetAsPayedWhenFree(string order_uid);
        Task<int> SetAsPayed(string order_no, string transaction_id, PayMethodEnum method);
        Task SetPayed(string payment_handle_user_uid,
             string order_uid, int pay_method, string external_payment_no,
             string comment, string[] imgs);

        Task<bool> IsMyOrder(string order_uid, string user_uid);
        Task CloseOrder(string order_uid, string reason);

        Task<DesignImageEntity> AddOrderDesign(DesignImageEntity model);
        Task<DeployEntity> AddOrderDeploymentUp(DeployEntity model);
        Task<DeployEntity> AddOrderDeploymentDown(DeployEntity model);
        Task<OrderEntity> UpdateOrderStatus(string order_uid, int status);
        Task<_<OrderEntity>> UpdateOrder(OrderEntity data);
        Task<_<OrderEntity>> PlaceOrder(OrderEntity data);
        Task<IEnumerable<OrderEntity>> QueryByMyOrders(string user_uid, int[] status, int? min_id, int count);
        Task<Dictionary<int, int>> QueryOrderCountGroupByStatus(string user_uid, int[] status);
        Task<IEnumerable<OrderEntity>> PrepareOrder(IEnumerable<OrderEntity> list);

        Task<IEnumerable<OrderEntity>> LoadItemsStationInfo(IEnumerable<OrderEntity> list,
            bool load_line, bool load_station, bool load_adwindow, bool load_media_type);

        Task<IEnumerable<OrderEntity>> LoadUsers(IEnumerable<OrderEntity> list);
        Task<IEnumerable<OrderEntity>> LoadApprovers(IEnumerable<OrderEntity> list);
        Task<IEnumerable<OrderEntity>> LoadItems(IEnumerable<OrderEntity> list);
        Task<IEnumerable<OrderEntity>> LoadDesigns(IEnumerable<OrderEntity> list);
        Task<IEnumerable<OrderEntity>> LoadDeployments(IEnumerable<OrderEntity> list);
        Task<IEnumerable<OrderEntity>> LoadHisotries(IEnumerable<OrderEntity> list);
        Task<PagerData<OrderEntity>> QueryByConditions(
            string order_no, string tf_no, string[] user_uids,
            int[] status, bool? expired, bool? has_design,
            DateTime? start_time_utc, DateTime? end_time_utc,
            int page, int page_count);
    }
}
