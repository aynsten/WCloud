using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WCloud.MetroAd.Order
{
    public enum PayMethodEnum : int
    {
        [Description("0元订单")]
        Free = -2,

        [Description("无支付方式")]
        None = -1,

        [Description("微信")]
        Wechat = 0,

        [Description("支付宝")]
        Alipay = 1,

        [Description("银联")]
        UnionPay = 3,

        [Description("转账")]
        Transfer = 4
    }
}
