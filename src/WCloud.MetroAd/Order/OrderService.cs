using Dapper;
using FluentAssertions;
using Lib.core;
using Lib.extension;
using Lib.helper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core.Helper;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.Framework.Database.EntityFrameworkCore.Service;
using WCloud.Member.DataAccess.EF;
using WCloud.Member.Domain.Admin;
using WCloud.Member.Domain.Login;
using WCloud.Member.Domain.User;
using WCloud.MetroAd.Metro;

namespace WCloud.MetroAd.Order
{
    public class UserOrderGroup
    {
        public string UserUID { get; set; }
        public int OrderCount { get; set; }
        public decimal PriceSum { get; set; }
    }
    public class OrderService : BasicService<OrderEntity>, IOrderService
    {
        private readonly IStringArraySerializer stringArraySerializer;
        private readonly IMetroAdDbFactory _dbFactory;
        private readonly IMSRepository<UserEntity> _userRepo;
        private readonly IConfiguration _config;

        private readonly int OrderNoLength = 9;

        public OrderService(
            IServiceProvider provider, IMetroAdRepository<OrderEntity> repo,
            IStringArraySerializer stringArraySerializer,
            IMetroAdDbFactory metroAdDbFactory,
            IMSRepository<UserEntity> _userRepo) : base(provider, repo)
        {
            this.stringArraySerializer = stringArraySerializer;
            this._dbFactory = metroAdDbFactory;
            this._userRepo = _userRepo;

            this._config = provider.ResolveConfig_();
            if (int.TryParse(this._config["order_no_len"] ?? "9", out var len))
            {
                len.Should().BeGreaterThan(5);
                this.OrderNoLength = len;
            }
        }

        public async Task<OrderEntity> GetOrderByNo(string order_no)
        {
            var res = await this._repo.GetFirstAsNoTrackAsync(x => x.OrderNo == order_no);
            return res;
        }

        public async Task SetPayed(string payment_handle_user_uid,
            string order_uid, int pay_method, string external_payment_no,
            string comment, string[] imgs)
        {
            order_uid.Should().NotBeNullOrEmpty();

            var db = this._repo.Database;

            var order = await db.Set<OrderEntity>().FirstOrDefaultAsync(x => x.UID == order_uid);
            order.Should().NotBeNull();

            if (order.Status != (int)OrderStatusEnum.待付款)
                throw new MsgException("当前状态无法付款");

            order.ManualPaymentOperatorUID = payment_handle_user_uid;
            order.PayMethod = pay_method;
            order.ExternalPaymentNo = external_payment_no;
            order.PaymentComment = comment;
            order.PaymentVoucherJson = this.stringArraySerializer.Serialize(imgs ?? new string[] { });
            order.PayTime = DateTime.UtcNow;

            order.Status = (int)OrderStatusEnum.待设计;

            await db.SaveChangesAsync();
        }

        public async Task SetAsPayedWhenFree(string order_uid)
        {
            order_uid.Should().NotBeNullOrEmpty();

            var db = this._repo.Database;

            var order = await db.Set<OrderEntity>().FirstOrDefaultAsync(x => x.UID == order_uid);
            order.Should().NotBeNull();

            if (order.Status != (int)OrderStatusEnum.待付款)
                throw new MsgException("当前状态无法付款");

            order.TotalPriceInCent.Should().BeLessOrEqualTo(0);

            order.PaymentComment = "免费订单自动付款";
            order.PayTime = DateTime.UtcNow;
            order.PayMethod = (int)PayMethodEnum.Free;

            order.Status = (int)OrderStatusEnum.待设计;

            await db.SaveChangesAsync();
        }

        public async Task ApproveOrReject(string approver_uid, string order_uid, bool approved, string comment)
        {
            approver_uid.Should().NotBeNullOrEmpty();
            order_uid.Should().NotBeNullOrEmpty();
            if (!approved && ValidateHelper.IsEmpty(comment))
                throw new MsgException("请输入拒绝理由");

            var db = this._repo.Database;

            var order = await db.Set<OrderEntity>().FirstOrDefaultAsync(x => x.UID == order_uid);
            order.Should().NotBeNull();

            if (order.Status != (int)OrderStatusEnum.待审核)
                throw new MsgException("当前状态无法审核");

            order.ApproverUID = approver_uid;
            order.ApproveComment = comment;
            order.ApproveTimeUtc = DateTime.UtcNow;

            order.Status = (int)(approved ? OrderStatusEnum.待付款 : OrderStatusEnum.驳回);

            await db.SaveChangesAsync();
        }

        public async Task<DesignImageEntity> AddOrderDesign(DesignImageEntity model)
        {
            model.Should().NotBeNull();
            model.OrderUID.Should().NotBeNullOrEmpty();
            model.DesignImageJson.Should().NotBeNullOrEmpty();
            model.DesignerUID.Should().NotBeNullOrEmpty();

            var db = this._repo.Database;

            var order = await db.Set<OrderEntity>().Where(x => x.UID == model.OrderUID).FirstOrDefaultAsync();
            order.Should().NotBeNull();

            var top = await db.Set<DesignImageEntity>()
                .Where(x => x.OrderUID == model.OrderUID)
                .Select(x => x.Id).Take(3).ToArrayAsync();

            order.DesignCount = top.Length + 1;

            model.InitSelf();
            db.Set<DesignImageEntity>().Add(model);

            await db.SaveChangesAsync();

            return model;
        }

        public async Task<DeployEntity> AddOrderDeploymentUp(DeployEntity model)
        {
            model.Should().NotBeNull();
            model.OrderUID.Should().NotBeNullOrEmpty();
            model.ImageJson.Should().NotBeNullOrEmpty();
            model.DeployerUID.Should().NotBeNullOrEmpty();

            model.DeploymentType = (int)DeployTypeEnum.Up;

            model.InitSelf();

            var db = this._repo.Database;

            var order = await db.Set<OrderEntity>().FirstOrDefaultAsync(x => x.UID == model.OrderUID);
            order.Should().NotBeNull();

            if (order.Status != (int)OrderStatusEnum.设计确认)
                throw new MsgException("当前状态不能上刊");

            order.Status = (int)OrderStatusEnum.上刊;
            order.DeployUpTimeUtc = DateTime.UtcNow;
            if (ValidateHelper.IsEmpty(order.TFNo))
            {
                order.TFNo = $"TF{order.Id.ToString().PadLeft(this.OrderNoLength, '0')}";
            }

            db.Set<DeployEntity>().Add(model);
            await db.SaveChangesAsync();

            return model;
        }

        public async Task<DeployEntity> AddOrderDeploymentDown(DeployEntity model)
        {
            model.Should().NotBeNull();
            model.OrderUID.Should().NotBeNullOrEmpty();
            model.ImageJson.Should().NotBeNullOrEmpty();
            model.DeployerUID.Should().NotBeNullOrEmpty();

            model.DeploymentType = (int)DeployTypeEnum.Down;

            model.InitSelf();

            var db = this._repo.Database;

            var order = await db.Set<OrderEntity>().FirstOrDefaultAsync(x => x.UID == model.OrderUID);
            order.Should().NotBeNull();

            if (order.Status != (int)OrderStatusEnum.上刊)
                throw new MsgException("当前状态不能下刊");

            order.Status = (int)OrderStatusEnum.交易完成;
            order.FinishTimeUtc = DateTime.UtcNow;

            db.Set<DeployEntity>().Add(model);
            await db.SaveChangesAsync();

            return model;
        }

        public async Task<OrderEntity> UpdateOrderStatus(string order_uid, int status)
        {
            order_uid.Should().NotBeNullOrEmpty();

            var db = this._repo.Database;

            var order = await db.Set<OrderEntity>().FirstOrDefaultAsync(x => x.UID == order_uid);
            order.Should().NotBeNull();

            if (order.Status != status)
            {
                order.Status = status;
                await db.SaveChangesAsync();
            }

            return order;
        }

        public async Task<bool> IsMyOrder(string order_uid, string user_uid)
        {
            order_uid.Should().NotBeNullOrEmpty();
            user_uid.Should().NotBeNullOrEmpty();

            var res = await this._repo.ExistAsync(x => x.UID == order_uid && x.UserUID == user_uid);

            return res;
        }

        public async Task CloseOrder(string order_uid, string reason)
        {
            order_uid.Should().NotBeNullOrEmpty();

            var db = this._repo.Database;

            var order = await db.Set<OrderEntity>().FirstOrDefaultAsync(x => x.UID == order_uid);
            order.Should().NotBeNull();

            var status = new[] { OrderStatusEnum.待审核, OrderStatusEnum.驳回, OrderStatusEnum.待付款 }.Select(x => (int)x).ToArray();

            if (!status.Contains(order.Status))
                throw new MsgException("当前状态不能关闭订单");

            order.Status = (int)OrderStatusEnum.交易关闭;
            order.CloseReason = reason;
            order.CloseTimeUtc = DateTime.UtcNow;

            await db.SaveChangesAsync();
        }

        public async Task<OrderEntity> ConfirmDesign(string user_uid, string order_uid)
        {
            user_uid.Should().NotBeNullOrEmpty();
            order_uid.Should().NotBeNullOrEmpty();

            var db = this._repo.Database;

            var order = await db.Set<OrderEntity>().FirstOrDefaultAsync(x => x.UID == order_uid);
            order.Should().NotBeNull();
            order.UserUID.Should().Be(user_uid);

            if (order.Status != (int)OrderStatusEnum.待设计)
                throw new MsgException("当前状态不能确认设计方案");

            var design = await db.Set<DesignImageEntity>().Where(x => x.OrderUID == order.UID).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            if (design == null)
                throw new MsgException("无法确认设计方案，设计师未提交设计方案");

            order.Status = (int)OrderStatusEnum.设计确认;
            order.ConfirmedDesignUID = design.UID;
            order.DesignConfirmTimeUtc = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return order;
        }

        async Task<OrderEntity> __load_ad_properties__(DbContext db, OrderEntity data)
        {
            var ad_window_uids = data.OrderItems.Select(x => x.AdWindowUID).ToArray();

            var all_ad_windows = await db.Set<AdWindowEntity>().AsNoTracking()
                .Where(x => ad_window_uids.Contains(x.UID)).ToArrayAsync();

            var line_uids = all_ad_windows.SelectNotEmptyAndDistinct(x => x.MetroLineUID).ToArray();
            var station_uids = all_ad_windows.SelectNotEmptyAndDistinct(x => x.MetroStationUID).ToArray();
            var media_uids = all_ad_windows.SelectNotEmptyAndDistinct(x => x.MediaTypeUID).ToArray();

            var all_lines = await db.Set<MetroLineEntity>().AsNoTracking()
                .Where(x => line_uids.Contains(x.UID)).ToArrayAsync();

            var all_stations = await db.Set<MetroStationEntity>().AsNoTracking()
                .Where(x => station_uids.Contains(x.UID)).ToArrayAsync();

            var all_medias = await db.Set<MediaTypeEntity>().AsNoTracking()
                .Where(x => media_uids.Contains(x.UID)).ToArrayAsync();

            foreach (var m in data.OrderItems)
            {
                var ad = all_ad_windows.FirstOrDefault(x => x.UID == m.AdWindowUID);
                if (ad == null)
                    continue;

                m.AdWindow = ad;

                m.MetroLineUID = ad.MetroLineUID;
                m.MetroStationUID = ad.MetroStationUID;
                m.PriceInCent = ad.PriceInCent;
                m.Height = ad.Height;
                m.Width = ad.Width;
                m.MediaTypeUID = ad.MediaTypeUID;
                m.AdWindowSnapshotJson = ad.ToJson();

                m.AdWindowName = ad.Name;
                m.MetroLineName = all_lines.FirstOrDefault(x => x.UID == m.MetroLineUID)?.Name;
                var station = all_stations.FirstOrDefault(x => x.UID == m.MetroStationUID);
                m.MetroStationName = station?.Name;
                m.MetroStationType = station.StationType;
                m.MediaTypeName = all_medias.FirstOrDefault(x => x.UID == m.MediaTypeUID)?.Name;
            }

            data.OrderItems = data.OrderItems.Where(x => x.AdWindow != null).ToArray();

            return data;
        }

        const string table = "`db-metro-ad`.`tb_order`";

        async Task<string> __create_order_no__(string order_uid)
        {
            var sql = @$"update {table} set
`OrderNo` = CONCAT('OD', LPAD(`Id`, {this.OrderNoLength}, '0')),
`TFNo` = CONCAT('TF', LPAD(`Id`, {this.OrderNoLength}, '0'))
where UID=@uid";

            using (var db = this._dbFactory.GetMetroAdDatabase())
            {
                var rows = await db.ExecuteAsync(sql, new { uid = order_uid });
                rows.Should().BeGreaterThan(0);

                sql = $"select `OrderNo` from {table} where UID=@uid limit 1";
                var res = await db.ExecuteScalarAsync<string>(sql, new { uid = order_uid });
                return res;
            }
        }

        public async Task UpdatePayCounter(string order_uid)
        {
            var sql = @$"update {table} set
`PayCounter`=`PayCounter`+1
where UID=@uid";

            using (var db = this._dbFactory.GetMetroAdDatabase())
            {
                var rows = await db.ExecuteAsync(sql, new { uid = order_uid });
                rows.Should().BeGreaterThan(0);
            }
        }

        public async Task<int> SetAsPayed(string order_no, string transaction_id, PayMethodEnum method)
        {
            var pay_time = DateTime.UtcNow;

            var sql = @$"
update {table} set

Status=@payed_status,
PayTime=@pay_time,
ExternalPaymentNo=@transaction_id,
PayMethod=@pay_method

where OrderNo=@order_no and Status=@where_status
";

            var p = new
            {
                payed_status = (int)OrderStatusEnum.待设计,
                where_status = (int)OrderStatusEnum.待付款,
                pay_method = (int)method,

                order_no = order_no,
                pay_time = pay_time,
                transaction_id = transaction_id,
            };

            using (var con = this._dbFactory.GetMetroAdDatabase())
            {
                var count = await con.ExecuteAsync(sql, p);

                return count;
            }
        }

        public async Task<_<OrderEntity>> UpdateOrder(OrderEntity data)
        {
            this.__ensure_order_input__(data);
            data.UID.Should().NotBeNullOrEmpty();

            var res = new _<OrderEntity>();

            try
            {
                var db = this._repo.Database;
                var order = await db.Set<OrderEntity>().FirstOrDefaultAsync(x => x.UID == data.UID);
                order.Should().NotBeNull();
                if (order.Status != (int)OrderStatusEnum.驳回)
                {
                    return res.SetErrorMsg("只有驳回状态才能修改订单");
                }

                data = await this.__init_order_data__(db, data);

                foreach (var m in data.OrderItems)
                {
                    m.InitSelf();
                    m.OrderUID = order.UID;
                }

                var old_item_uids = await db.Set<OrderItemEntity>().AsNoTracking()
                    .Where(x => x.OrderUID == order.UID)
                    .Select(x => x.UID).ToArrayAsync();

                if (old_item_uids.Any())
                {
                    //删除老的item
                    db.Set<OrderItemEntity>().RemoveRange(db.Set<OrderItemEntity>().Where(x => old_item_uids.Contains(x.UID)));
                }
                //添加新的item
                db.Set<OrderItemEntity>().AddRange(data.OrderItems);
                //修改主表信息
                order.SetField(new
                {
                    data.AdStartTimeUtc,
                    data.AdEndTimeUtc,
                    data.TotalDays,
                    data.CustomerImageJson,
                    data.CustomerDemand,
                    data.Status,
                    data.TotalPriceInCent,
                    //下面好多参数是为了确保清空无用信息
                    data.IsDeleted,
                    data.ApproverUID,
                    data.ApproveComment,
                    data.ApproveTimeUtc,
                    data.DesignCount,
                    data.ConfirmedDesignUID,
                    data.PayMethod,
                    data.ExternalPaymentNo,
                    data.ManualPaymentOperatorUID,
                    data.PaymentComment,
                    data.PaymentVoucherJson,
                    data.PayTime,
                    data.PaymentPending,
                    data.PayCounter,
                    data.TFNo,
                    data.CloseTimeUtc,
                    data.DeployUpTimeUtc,
                    data.DesignConfirmTimeUtc,
                    data.FinishTimeUtc,
                });

                await db.SaveChangesAsync();

                return res.SetSuccessData(order);
            }
            catch (MsgException e)
            {
                return res.SetErrorMsg(e.Message);
            }
        }

        void __ensure_order_input__(OrderEntity data)
        {
            data.Should().NotBeNull();
            data.UserUID.Should().NotBeNullOrEmpty();
            data.TotalDays.Should().BeGreaterOrEqualTo(1);
            data.OrderItems.Should().NotBeNullOrEmpty();
            data.CustomerDemand.Should().NotBeNullOrEmpty();
            data.ImageList.Should().NotBeNullOrEmpty();

            foreach (var m in data.OrderItems)
            {
                m.AdWindowUID.Should().NotBeNullOrEmpty();
            }
            data.OrderItems.Select(x => x.AdWindowUID).Distinct().Count()
                .Should().Be(data.OrderItems.Length, "橱窗id存在重复");
        }

        async Task<OrderEntity> __init_order_data__(DbContext db, OrderEntity data)
        {
            var now = DateTime.UtcNow;

            //开始结束时间
            //这里时间不取日期，utc取日期没有意义
            //data.AdStartTimeUtc = data.AdStartTimeUtc.Date;
            data.AdEndTimeUtc = data.AdStartTimeUtc.AddDays(data.TotalDays);
            data.CustomerImageJson = this.stringArraySerializer.Serialize(data.ImageList);
            data.Status = (int)OrderStatusEnum.待审核;
            if (data.AdStartTimeUtc <= now)
            {
                throw new MsgException("广告起始日期必须晚于今天");
            }
            data.ApproverUID = null;
            data.ApproveComment = null;
            data.ApproveTimeUtc = null;

            data.IsDeleted = 0;

            data.DesignCount = 0;
            data.ConfirmedDesignUID = null;
            data.ExternalPaymentNo = null;

            data.ManualPaymentOperatorUID = null;

            data.PayMethod = (int)PayMethodEnum.None;
            data.PaymentComment = null;
            data.PaymentVoucherJson = null;
            data.PayTime = null;
            data.PaymentPending = 0;

            data.TFNo = null;
            data.FinishTimeUtc = null;
            data.DesignConfirmTimeUtc = null;
            data.CloseTimeUtc = null;
            data.DeployUpTimeUtc = null;

            data = await this.__load_ad_properties__(db, data);
            //总价
            data.TotalPriceInCent = data.OrderItems.Sum(x => x.PriceInCent * data.TotalDays);

            return data;
        }

        public async Task<_<OrderEntity>> PlaceOrder(OrderEntity data)
        {
            this.__ensure_order_input__(data);

            var res = new _<OrderEntity>();

            try
            {
                var db = this._repo.Database;
                data = await this.__init_order_data__(db, data);

                data.InitSelf();

                foreach (var m in data.OrderItems)
                {
                    m.InitSelf();
                    m.OrderUID = data.UID;
                }

                db.Set<OrderEntity>().Add(data);
                db.Set<OrderItemEntity>().AddRange(data.OrderItems);

                await db.SaveChangesAsync();

                data.OrderNo = await this.__create_order_no__(data.UID);

                return res.SetSuccessData(data);
            }
            catch (MsgException e)
            {
                return res.SetErrorMsg(e.Message);
            }
        }

        /// <summary>
        /// 处理用户名查找条件的时候对用户表limit 1，尽量避免全表扫描
        /// </summary>
        /// <param name="user_uid"></param>
        /// <param name="status"></param>
        /// <param name="expired"></param>
        /// <returns></returns>
        public async Task<PagerData<OrderEntity>> QueryByConditions(
            string order_no, string tf_no, string[] user_uids,
            int[] status, bool? expired, bool? has_design,
            DateTime? start_time_utc, DateTime? end_time_utc,
            int page, int page_count)
        {
            var db = this._repo.Database;
            var query = db.Set<OrderEntity>().AsNoTracking();

            query = query.WhereIf(ValidateHelper.IsNotEmpty(order_no), x => x.OrderNo == order_no);
            query = query.WhereIf(ValidateHelper.IsNotEmpty(tf_no), x => x.TFNo == tf_no);
            query = query.WhereIf(ValidateHelper.IsNotEmpty(user_uids), x => user_uids.Contains(x.UserUID));
            query = query.WhereIf(ValidateHelper.IsNotEmpty(status), x => status.Contains(x.Status));

            if (has_design != null)
            {
                if (has_design.Value)
                    query = query.Where(x => x.DesignCount > 0);
                else
                    query = query.Where(x => x.DesignCount <= 0);
            }
            if (expired != null)
            {
                var now = DateTime.UtcNow;
                if (expired.Value)
                    query = query.Where(x => x.AdEndTimeUtc <= now);
                else
                    query = query.Where(x => x.AdEndTimeUtc > now);
            }
            if (start_time_utc != null)
            {
                start_time_utc = start_time_utc.Value;
                query = query.Where(x => x.CreateTimeUtc >= start_time_utc.Value);
            }
            if (end_time_utc != null)
            {
                end_time_utc = end_time_utc.Value;
                query = query.Where(x => x.CreateTimeUtc < end_time_utc.Value);
            }

            var res = await query.ToPagedListAsync(page, page_count, x => x.Id);

            return res;
        }

        public async Task<IEnumerable<OrderEntity>> QueryByMyOrders(string user_uid, int[] status, int? min_id, int count)
        {
            user_uid.Should().NotBeNullOrEmpty();

            var db = this._repo.Database;

            var query = db.Set<OrderEntity>().AsNoTracking().Where(x => x.UserUID == user_uid);
            if (ValidateHelper.IsNotEmpty(status))
            {
                query = query.Where(x => status.Contains(x.Status));
            }
            if (min_id != null)
            {
                query = query.Where(x => x.Id < min_id.Value);
            }

            var res = await query.OrderByDescending(x => x.Id).Take(count).ToArrayAsync();

            return res;
        }

        public async Task<IEnumerable<OrderEntity>> PrepareOrder(IEnumerable<OrderEntity> list)
        {
            list.Should().NotBeNull();
            if (list.Any())
            {
                foreach (var m in list)
                {
                    m.ImageList = this.stringArraySerializer.Deserialize(m.CustomerImageJson);
                    m.PaymentVoucherList = this.stringArraySerializer.Deserialize(m.PaymentVoucherJson);

                    m.CustomerImageJson = null;
                    m.PaymentVoucherJson = null;
                }
            }
            await Task.CompletedTask;
            return list;
        }

        public async Task<IEnumerable<OrderEntity>> LoadUsers(IEnumerable<OrderEntity> list)
        {
            list.Should().NotBeNull();
            if (list.Any())
            {
                var uids = list.Select(x => x.UserUID).Distinct().ToArray();
                var data = await this._userRepo.GetListAsync(x => uids.Contains(x.UID));

                var phones = await this._userRepo.Database.Set<UserPhoneEntity>().AsNoTracking()
                    .Where(x => uids.Contains(x.UserUID)).ToArrayAsync();

                foreach (var m in data)
                {
                    m.UserPhone = phones.FirstOrDefault(x => x.UserUID == m.UID);
                }

                foreach (var m in list)
                {
                    m.User = data.FirstOrDefault(x => x.UID == m.UserUID);
                }
            }
            return list;
        }

        public async Task<IEnumerable<OrderEntity>> LoadAdmins(IEnumerable<OrderEntity> list)
        {
            list.Should().NotBeNull();
            if (list.Any())
            {
                foreach (var m in list)
                {
                    m.OrderDeploymentUp ??= new DeployEntity[] { };
                    m.OrderDeploymentDown ??= new DeployEntity[] { };
                    m.OrderDesigns ??= new DesignImageEntity[] { };
                }
                var admin_uids = new List<string>();
                admin_uids.AddList_(list.SelectMany(x => x.OrderDeploymentUp).Select(x => x.DeployerUID).ToArray());
                admin_uids.AddList_(list.SelectMany(x => x.OrderDeploymentDown).Select(x => x.DeployerUID).ToArray());
                admin_uids.AddList_(list.SelectMany(x => x.OrderDesigns).Select(x => x.DesignerUID));

                admin_uids = admin_uids.Distinct().ToList();
                if (admin_uids.Any())
                {
                    var db = this._userRepo.Database;
                    var admins = await db.Set<AdminEntity>().AsNoTracking().Where(x => admin_uids.Contains(x.UID)).ToArrayAsync();

                    foreach (var order in list)
                    {
                        foreach (var up in order.OrderDeploymentUp)
                        {
                            up.Admin = admins.FirstOrDefault(x => x.UID == up.DeployerUID);
                        }
                        foreach (var up in order.OrderDeploymentDown)
                        {
                            up.Admin = admins.FirstOrDefault(x => x.UID == up.DeployerUID);
                        }
                        foreach (var up in order.OrderDesigns)
                        {
                            up.Admin = admins.FirstOrDefault(x => x.UID == up.DesignerUID);
                        }
                    }

                }
            }
            return list;
        }

        public async Task<IEnumerable<OrderEntity>> LoadApprovers(IEnumerable<OrderEntity> list)
        {
            list.Should().NotBeNull();
            if (list.Any())
            {
                var uids = list.SelectNotEmptyAndDistinct(x => x.ApproverUID).ToArray();
                var db = this._userRepo.Database;
                var all_items = uids.Any() ?
                    await db.Set<AdminEntity>().AsNoTracking().Where(x => uids.Contains(x.UID)).ToArrayAsync() :
                    new AdminEntity[] { };

                foreach (var m in list)
                {
                    m.Approver = all_items.FirstOrDefault(x => x.UID == m.ApproverUID);
                }
            }
            return list;
        }

        public async Task<IEnumerable<OrderEntity>> LoadItems(IEnumerable<OrderEntity> list)
        {
            list.Should().NotBeNull();
            if (list.Any())
            {
                var uids = list.Select(x => x.UID).Distinct().ToArray();
                var db = this._repo.Database;
                var all_items = await db.Set<OrderItemEntity>().AsNoTracking().Where(x => uids.Contains(x.OrderUID)).ToArrayAsync();

                foreach (var m in list)
                {
                    m.OrderItems = all_items.Where(x => x.OrderUID == m.UID).ToArray();
                }
            }
            return list;
        }

        public async Task<IEnumerable<OrderEntity>> LoadItemsStationInfo(IEnumerable<OrderEntity> list,
            bool load_line, bool load_station, bool load_adwindow, bool load_media_type)
        {
            list.Should().NotBeNull();

            var items = list.SelectMany(x => x.OrderItems).ToArray();

            if (items.Any())
            {
                var line_uids = load_line ?
                    items.SelectNotEmptyAndDistinct(x => x.MetroLineUID).ToArray() : new string[] { };

                var station_uids = load_station ?
                    items.SelectNotEmptyAndDistinct(x => x.MetroStationUID).ToArray() : new string[] { };

                var ad_window_uids = load_adwindow ?
                    items.SelectNotEmptyAndDistinct(x => x.AdWindowUID).ToArray() : new string[] { };

                var media_uids = load_media_type ?
                    items.SelectNotEmptyAndDistinct(x => x.MediaTypeUID).ToArray() : new string[] { };

                var db = this._repo.Database;

                var all_lines = line_uids.Any() ?
                    await db.Set<MetroLineEntity>().AsNoTracking().Where(x => line_uids.Contains(x.UID)).ToArrayAsync() :
                    new MetroLineEntity[] { };

                var all_stations = station_uids.Any() ?
                    await db.Set<MetroStationEntity>().AsNoTracking().Where(x => station_uids.Contains(x.UID)).ToArrayAsync() :
                    new MetroStationEntity[] { };

                var all_ad_windows = ad_window_uids.Any() ?
                    await db.Set<AdWindowEntity>().AsNoTracking().Where(x => ad_window_uids.Contains(x.UID)).ToArrayAsync() :
                    new AdWindowEntity[] { };

                var all_medias = media_uids.Any() ?
                    await db.Set<MediaTypeEntity>().AsNoTracking().Where(x => media_uids.Contains(x.UID)).ToArrayAsync() :
                    new MediaTypeEntity[] { };

                foreach (var m in items)
                {
                    m.MetroLine = all_lines.FirstOrDefault(x => x.UID == m.MetroLineUID);
                    m.MetroStation = all_stations.FirstOrDefault(x => x.UID == m.MetroStationUID);
                    m.AdWindow = all_ad_windows.FirstOrDefault(x => x.UID == m.AdWindowUID);
                    m.MediaType = all_medias.FirstOrDefault(x => x.UID == m.MediaTypeUID);
                }
            }
            return list;
        }

        public async Task<IEnumerable<OrderEntity>> LoadHisotries(IEnumerable<OrderEntity> list)
        {
            list.Should().NotBeNull();
            if (list.Any())
            {
                var uids = list.Select(x => x.UID).Distinct().ToArray();
                var db = this._repo.Database;

                var all_items = await db.Set<OrderHistoryEntity>()
                    .AsNoTracking()
                    .Where(x => uids.Contains(x.OrderUID))
                    .ToArrayAsync();

                foreach (var m in list)
                {
                    m.OrderHistories = all_items.Where(x => x.OrderUID == m.UID).OrderBy(x => x.CreateTimeUtc).ToArray();
                }
            }
            return list;
        }

        public async Task<IEnumerable<OrderEntity>> LoadDesigns(IEnumerable<OrderEntity> list)
        {
            if (list.Any())
            {
                var uids = list.Select(x => x.UID).Distinct().ToArray();
                var db = this._repo.Database;
                var all_items = await db.Set<DesignImageEntity>().AsNoTracking().Where(x => uids.Contains(x.OrderUID)).ToArrayAsync();

                foreach (var m in all_items)
                {
                    m.DesignImageJson ??= "[]";
                    m.DesignImages = this.stringArraySerializer.Deserialize(m.DesignImageJson);
                }

                foreach (var m in list)
                {
                    //只显示最后一个
                    m.OrderDesigns = all_items.Where(x => x.OrderUID == m.UID).OrderByDescending(x => x.Id).Take(1).ToArray();
                }
            }
            return list;
        }

        public async Task<IEnumerable<OrderEntity>> LoadDeployments(IEnumerable<OrderEntity> list)
        {
            if (list.Any())
            {
                var uids = list.Select(x => x.UID).Distinct().ToArray();
                var db = this._repo.Database;
                var all_items = await db.Set<DeployEntity>().AsNoTracking().Where(x => uids.Contains(x.OrderUID)).ToArrayAsync();

                foreach (var m in all_items)
                {
                    m.ImageJson ??= "[]";
                    m.ImageList = this.stringArraySerializer.Deserialize(m.ImageJson);
                }

                foreach (var m in list)
                {
                    var query = all_items.Where(x => x.OrderUID == m.UID);
                    m.OrderDeploymentUp = query.Where(x => x.DeploymentType == (int)DeployTypeEnum.Up).ToArray();
                    m.OrderDeploymentDown = query.Where(x => x.DeploymentType == (int)DeployTypeEnum.Down).ToArray();
                }
            }
            return list;
        }

        public async Task<Dictionary<int, int>> QueryOrderCountGroupByStatus(string user_uid, int[] status)
        {
            user_uid.Should().NotBeNullOrEmpty();
            status.Should().NotBeNullOrEmpty();

            var db = this._repo.Database;

            var query = db.Set<OrderEntity>().AsNoTrackingQueryable().Where(x => x.UserUID == user_uid && status.Contains(x.Status));

            var data = await query.GroupBy(x => x.Status).Select(x => new { x.Key, Count = x.Count() }).ToArrayAsync();

            var res = data.ToDictionary_(x => x.Key, x => x.Count);

            return res;
        }

        public async Task<UserOrderGroup[]> UserOrderCount(string[] user_uids)
        {
            user_uids.Should().NotBeNullOrEmpty();
            user_uids = user_uids.Distinct().ToArray();

            var query = this._repo.Table.AsNoTracking().Where(x => user_uids.Contains(x.UserUID));

            var data = await query
                .GroupBy(x => x.UserUID)
                .Select(x => new
                {
                    x.Key,
                    Count = x.Count(),
                    PriceSum = x.Sum(m => m.TotalPriceInCent)
                }).ToArrayAsync();

            var res = data.Select(x => new UserOrderGroup()
            {
                UserUID = x.Key,
                OrderCount = x.Count,
                PriceSum = (decimal)((double)x.PriceSum / 100.0)
            }).ToArray();

            return res;
        }

        public async Task SetPaymentPending(string order_uid)
        {
            order_uid.Should().NotBeNullOrEmpty();

            var db = this._repo.Database;

            var order = await db.Set<OrderEntity>().FirstOrDefaultAsync(x => x.UID == order_uid);
            order.Should().NotBeNull();

            order.PaymentPending = 1;

            await db.SaveChangesAsync();
        }
    }
}
