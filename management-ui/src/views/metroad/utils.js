var status_dict = [
    {
        key: '0',
        name: '待审批'
    },
    {
        key: '1',
        name: '待付款'
    },
    {
        key: '2',
        name: '待设计'
    },
    {
        key: '4',
        name: '待上刊'
    },
    {
        key: '5',
        name: '投放中'
    },
    {
        key: '6',
        name: '已完成'
    },
    {
        key: '-1',
        name: '已驳回'
    },
    {
        key: '-2',
        name: '已取消'
    },
    {
        key: 'xx',
        name: '全部'
    }
];

var station_types = [
    {
        key: 0,
        name: '普通站点'
    },
    {
        key: 1,
        name: '特色站点'
    },
    {
        key: 2,
        name: '核心站点'
    },
]

function get_status_str(s) {
    if (s === null || s === undefined) {
        s = '0';
    }
    var data = status_dict.filter(x => x.key === s.toString());
    if (data && data.length > 0) {
        return data[0].name;
    } else {
        return '未知状态';
    }
}

var all_paymethods = [
    {
        key: '-2',
        name: '0元订单'
    },
    {
        key: '-1',
        name: '无支付方式'
    },
    {
        key: '0',
        name: '微信'
    },
    {
        key: '1',
        name: '支付宝'
    },
    {
        key: '3',
        name: '银联'
    },
    {
        key: '4',
        name: '转账'
    }
]

function get_pay_method_str(s) {
    if (s != null && s != undefined) {
        var data = all_paymethods.filter(x => x.key == s.toString())
        if (data && data.length > 0) {
            return data[0].name;
        }
    }
    return 'none';
}

export { status_dict, get_status_str, get_pay_method_str, station_types }