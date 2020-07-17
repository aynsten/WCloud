using System;

namespace WCloud.MetroAd.Order
{
    public enum OrderStatusEnum : int
    {
        驳回 = -1,
        交易关闭 = -2,

        待审核 = 0,
        待付款 = 1,
        待设计 = 2,
        //设计待确认 = 3,
        设计确认 = 4,
        上刊 = 5,
        交易完成 = 6,
    }
}
