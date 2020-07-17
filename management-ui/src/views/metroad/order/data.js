import { status_dict } from '../utils'

var status_list = status_dict;

var list_columns = [
    {
        title: '订单号',
        dataIndex: 'OrderNo'
    },
    {
        title: '状态',
        scopedSlots: { customRender: 'status' }
    },
    {
        title: '下单人',
        scopedSlots: { customRender: 'customer' }
    },
    {
        title: '点位数&站点数',
        scopedSlots: { customRender: 'info' }
    },
    {
        title: '开始结束时间',
        scopedSlots: { customRender: 'time' }
    },
    {
        title: '金额',
        dataIndex: 'TotalPrice'
    },
    {
        title: '下单时间',
        scopedSlots: { customRender: 'create_time' }
    },
    {
        title: '备注',
        scopedSlots: { customRender: 'remark' }
    },
    {
        title: '操作',
        scopedSlots: { customRender: 'action' }
    }
];

var list_columns_tf = [
    {
        title: '投放号',
        dataIndex: 'TFNo'
    },
    {
        title: '状态',
        scopedSlots: { customRender: 'status' }
    },
    {
        title: '订单号',
        dataIndex: 'OrderNo'
    },
    {
        title: '下单人',
        scopedSlots: { customRender: 'customer' }
    },
    {
        title: '开始结束时间',
        scopedSlots: { customRender: 'time' }
    },
    {
        title: '下单时间',
        scopedSlots: { customRender: 'create_time' }
    },
    {
        title: '备注',
        scopedSlots: { customRender: 'remark' }
    },
    {
        title: '操作',
        scopedSlots: { customRender: 'action' }
    }
];

var order_tab = [
    {
        key: '0',
        name: '待审批',
        param: {
            multi_status: [0]
        }
    },
    {
        key: '1',
        name: '待付款',
        param: {
            multi_status: [1]
        }
    },
    {
        key: '2',
        name: '已付款',
        param: {
            multi_status: [2, 4, 5, 6]
        }
    },
    {
        key: '-1',
        name: '已驳回',
        param: {
            multi_status: [-1]
        }
    },
    {
        key: '-2',
        name: '已取消',
        param: {
            multi_status: [-2]
        }
    }
];

var tf_tab = [
    {
        key: '2_no_design',
        name: '设计待上传',
        param: {
            has_design: false,
            multi_status: [2]
        }
    },
    {
        key: '2_has_design',
        name: '设计待确认',
        param: {
            has_design: true,
            multi_status: [2]
        }
    },
    {
        key: '4',
        name: '待上刊',
        param: {
            multi_status: [4]
        }
    },
    {
        key: '5_normal',
        name: '投放中',
        param: {
            is_expired: false,
            multi_status: [5]
        }
    },
    {
        key: '5_expired',
        name: '待下刊',
        param: {
            is_expired: true,
            multi_status: [5]
        }
    },
    {
        key: '6',
        name: '已完成',
        param: {
            multi_status: [6]
        }
    }
];

export { status_list, list_columns, list_columns_tf, tf_tab, order_tab }