using FluentAssertions;
using Lib.cache;
using Lib.core;
using Lib.extension;
using Lib.helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Admin.Message;
using WCloud.Core.Cache;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Framework.MessageBus;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Framework.Wechat.Login;
using WCloud.Framework.Wechat.Pay;
using WCloud.Member.Authentication.ControllerExtensions;
using WCloud.MetroAd.Message;
using WCloud.MetroAd.Order;

namespace WCloud.MetroAd.Api.Controller
{
    [MetroAdServiceRoute]
    public class WechatController : WCloudBaseController, IUserController
    {
        private readonly ILogger _logger;
        private readonly IOrderService orderService;
        private readonly WxPayApi wxPayApi;
        private readonly IUserWxLoginService userWxLoginService;
        private readonly IConfiguration configuration;
        private readonly ICacheKeyManager cacheKeyManager;
        private readonly ICacheProvider cacheProvider;

        public WechatController(ILogger<WechatController> logger,
            IOrderService orderService, IConfiguration configuration,
            WxPayApi wxPayApi, IUserWxLoginService userWxLoginService,
            ICacheProvider cacheProvider,ICacheKeyManager cacheKeyManager)
        {
            this._logger = logger;
            this.orderService = orderService;
            this.configuration = configuration;
            this.wxPayApi = wxPayApi;
            this.userWxLoginService = userWxLoginService;
            this.cacheKeyManager = cacheKeyManager;
            this.cacheProvider = cacheProvider;
        }

        /// <summary>
        /// 一个订单号在一个商户下是唯一的，
        /// 为了解决多次点击支付但是不付款，在订单号后面加一个计数器
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        async Task<string> __create_out_trade_no__(OrderEntity order)
        {
            order.OrderNo.Should().NotBeNullOrEmpty();

            await this.orderService.UpdatePayCounter(order.UID);

            var counter = order.PayCounter.ToString().PadLeft(3, '0');

            var res = $"{order.OrderNo}_{counter}";
            return res;
        }

        async Task<string> __get_openid__(string user_uid, string code)
        {
            var openid_response = await this.userWxLoginService.__get_wx_openid__(code);

            return openid_response.openid;
        }

        /// <summary>
        /// 调用微信服务器创建预支付订单
        /// </summary>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> CreatePrepayOrder(string order_uid, string code)
        {
            var log = new Dictionary<string, object>
            {
                ["input"] = new { order_uid, code }
            };
            try
            {
                order_uid.Should().NotBeNullOrEmpty();
                code.Should().NotBeNullOrEmpty();

                var loginuser = await this.GetLoginUserAsync();

                log["user_uid"] = loginuser.UserID;

                var openid = await this.__get_openid__(loginuser.UserID, code);

                log["openid"] = openid;

                var order = await this.orderService.GetByUID(order_uid);
                order.Should().NotBeNull();
                order.UserUID.Should().Be(loginuser.UserID);

                if (order.TotalPriceInCent <= 0)
                    throw new MsgException("订单金额必须大于0");

                if (order.Status != (int)OrderStatusEnum.待付款)
                    throw new MsgException("订单状态无法发起支付");

                if (order.PaymentPending > 0)
                    throw new MsgException("订单已支付，等待支付结果中");

                log["order"] = order;

                var out_trade_no = await this.__create_out_trade_no__(order);

                log["out_trade_no"] = out_trade_no;

                var dict = new Dictionary<string, object>()
                {
                    ["out_trade_no"] = out_trade_no,
                    ["total_fee"] = order.TotalPriceInCent,
                    ["body"] = $"商品-订单号:{order.OrderNo}",
                    ["trade_type"] = "JSAPI",
                    ["openid"] = openid,
                };

                var wx_res = await this.wxPayApi.UnifiedOrder(dict);
                wx_res.Should().NotBeNull();

                log["wx_res"] = wx_res;

                if (!wx_res.IsSuccess())
                {
                    throw new WxPayException("创建支付订单号失败");
                }

                var olog = new OperationLogMessage(loginuser).UpdateOrAdd("支付", false).WithMessage("发起支付").WithExtraData(new
                {
                    order_uid,
                    openid,
                    pay_info = dict
                });
                await this.AddOperationLog(olog);

                var res = this.__prepay_data__(wx_res);
                return SuccessJson(res);
            }
            catch (WxPayException e)
            {
                log["out_exception"] = e.Message;
                log["out_exception_info"] = e.Info;

                var info = log.ToJson();

                this._logger.AddErrorLog(info, e);
                return GetJsonRes("创建支付订单号失败");
            }
        }

        /*
          timeStamp: '',
  nonceStr: '',
  package: '',
  signType: 'MD5',
  paySign: '',
  success (res) { },
  fail (res) { }
})
             */

        object __prepay_data__(WxPayUnifiedOrderResponse wx_res)
        {
            var appIdKey = "appId";

            var dict = new Dictionary<string, string>()
            {
                [appIdKey] = this.wxPayApi.Config.AppID,
                ["timeStamp"] = this.wxPayApi.GenerateTimeStamp(),
                ["nonceStr"] = wx_res.nonce_str,
                ["signType"] = "MD5",
                ["package"] = $"prepay_id={wx_res.prepay_id}"
            };

            var sign = this.wxPayApi.MD5Sign(dict, out var sign_str);
            dict["paySign"] = sign;

            dict.Remove(appIdKey);

            return dict;
        }


        IActionResult __response__(bool success, string message = null)
        {
            var code = success ? "SUCCESS" : "FAIL";
            var dict = new Dictionary<string, object>()
            {
                ["return_code"] = code,
                ["return_msg"] = message ?? code
            };

            var res_xml = this.wxPayApi.ToXml(dict);
            var res = Content(res_xml, contentType: MediaTypeHeaderValue.Parse("application/xml"));

            return res;
        }

        async Task<bool> __return_success__(string out_trade_no)
        {
            out_trade_no.Should().NotBeNullOrEmpty();

            var order = await this.orderService.GetOrderByNo(out_trade_no);
            if (order != null)
            {
                var status = new OrderStatusEnum[]
                {
                    OrderStatusEnum.待付款
                }.Select(x => (int)x).ToArray();

                //还是待付款就提示失败，让微信再次回调
                if (status.Contains(order.Status))
                {
                    return false;
                }
            }
            else
            {
                this._logger.AddErrorLog($"回调订单号不存在：{out_trade_no}");
            }
            return true;
        }

        PayNotifyLogMessage __get_notify_log__(WxPayNotifyData data, string order_no, string post_data)
        {
            var res = new PayNotifyLogMessage()
            {
                PayMethod = (int)PayMethodEnum.Wechat,
                OrderNo = order_no ?? data.out_trade_no,
                OutTradeNo = data.out_trade_no,
                ExternalPaymentNo = data.transaction_id,
                NotificationData = post_data
            };
            res.InitSelf();
            return res;
        }

        string __parse_order_no__(string out_trade_no)
        {
            var res = out_trade_no?.Split('_').FirstOrDefault();
            return res;
        }

        /// <summary>
        /// 支付结果回调
        /// </summary>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> Notify(
            [FromServices]IMessagePublisher messagePublisher)
        {
            var log = new Dictionary<string, object>();

            string out_trade_no = null;

            try
            {
                using var ms = new System.IO.MemoryStream();

                await this.Request.Body.CopyToAsync(ms);

                var xml_data = System.Text.Encoding.UTF8.GetString(ms.ToArray()); //IOHelper.StreamToString(ms);

                log["notify_data"] = xml_data;

                var notify_data = this.wxPayApi.ParseNotifyData(xml_data);
                log["notify_data_model"] = notify_data;
                out_trade_no = this.__parse_order_no__(notify_data.out_trade_no);
                log["out_trade_no"] = out_trade_no;

                {
                    //收到回调不管怎么样都添加记录
                    var notify_data_log = this.__get_notify_log__(notify_data, out_trade_no, xml_data);
                    log["notify_data_log"] = notify_data_log;
                    await messagePublisher.PublishAsync(notify_data_log);
                }

                if (ValidateHelper.IsEmpty(out_trade_no))
                    throw new WxPayException("回调信息中未找到订单号");

                log["publish"] = true;

                if (!notify_data.IsSuccess())
                    throw new WxPayException("通知状态为不成功");

                var my_sign = this.wxPayApi.MD5Sign(notify_data.Parameters, out var sign_str);
                log["notify_sign_str"] = sign_str;

                if (notify_data.sign != my_sign)
                    throw new WxPayException("签名校验失败");

                var count = await this.orderService.SetAsPayed(out_trade_no, notify_data.transaction_id, PayMethodEnum.Wechat);
                if (count <= 0)
                    throw new WxPayException("更新订单失败");

                {
                    var order = await this.orderService.GetOrderByNo(out_trade_no);
                    if (order != null)
                    {
                        await this.AddOrderStatusHistoryLog(order.UID);
                    }
                    else
                        this._logger.AddErrorLog("____pls_notice____");
                }

                return this.__response__(true);
            }
            catch (WxPayException e)
            {
                log["out_exception"] = e.Message;
                log["out_exception_info"] = e.Info;
                //更新状态失败，检查订单状态是否时已支付，如果是就直接返回成功

                if (ValidateHelper.IsNotEmpty(out_trade_no))
                {
                    try
                    {
                        var success = await this.__return_success__(out_trade_no);
                        if (success)
                        {
                            return this.__response__(true);
                        }
                    }
                    catch (Exception err)
                    {
                        log["retry_in_ex_block"] = new
                        {
                            err.Message,
                            err.GetType().FullName,
                        };
                    }
                }

                var info = log.ToJson();

                this._logger.AddErrorLog($"处理微信支付回调失败:{info}", e);

                return this.__response__(false, e.Message.Take(50).JoinAsString(string.Empty));
            }
        }
    }
}
