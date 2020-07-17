var columns = [
    {
        title: '订单号',
        scopedSlots: { customRender: 'order_no' }
    },
    {
        title: '金额',
        dataIndex: 'Price'
    },
    {
        title: '支付方式',
        scopedSlots: { customRender: 'pay_method' }
    },
    {
        title: '流向',
        scopedSlots: { customRender: 'direction' }
    },
    {
        title: '时间',
        scopedSlots: { customRender: 'time' }
    },
    {
        title: '状态',
        scopedSlots: { customRender: 'status' }
    }
];

var report_columns = [
    {
        title: '年',
        dataIndex: 'Year'
    },
    {
        title: '月份',
        dataIndex: 'Month',
        customRender: (text, record, i) => record.Month.toString().padStart(2, '0')
    },
    {
        title: '营收（元）',
        dataIndex: 'Sum'
    },
    {
        title: '查看',
        width: '100px',
        scopedSlots: { customRender: 'detail' }
    }
];

export { columns, report_columns }