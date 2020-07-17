var columns = [
    {
        title: '用户名',
        dataIndex: 'UserName'
    },
    {
        title: '账号类型',
        scopedSlots: { customRender: 'account_type' }
    },
    {
        title: '平台',
        dataIndex: 'Platform'
    },
    {
        title: '页面',
        dataIndex: 'Page'
    },
    {
        title: '动作',
        dataIndex: 'Action'
    },
    {
        title: '信息',
        dataIndex: 'Message'
    },
    {
        title: '时间',
        scopedSlots: { customRender: 'time' },
        width: 200
    }
];

export { columns }