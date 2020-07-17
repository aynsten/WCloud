var flow = {
    name: "请假工作流",
    nodes: [
        {
            UID: 'create',
            node_name: "创建",
            //创建后有两个分支：
            //创建-人事-结束
            //创建-人事-经理-结束
            //条件判断可以通过反射来完成，不需要再工作流中嵌入代码，避免不稳定因素
            to: ['hr-1', 'hr-2'],
            //当前节点可以操作的人
            node_operator: {
                //所有人
                all: true,
                //角色为人事
                roles: ['人事'],
                //指定的几个人
                user_uids: ['zhangsan', 'lisi', 'wangwu']
            }
        },
        {
            UID: 'hr-1',
            node_name: "审批",
            to: ['finish'],
            //当前节点的流转条件
            condition: [
                {
                    description: '请假天数小于等于3天',
                    field_name: 'Days',
                    operator: '<=',
                    value: 3
                }
            ],
            node_operator: {
                roles: ['人事'],
            }
        },
        {
            UID: 'hr-2',
            node_name: "审批",
            to: ['manager'],
            condition: [
                {
                    description: '请假天数大于3天',
                    field_name: 'Days',
                    operator: '>',
                    value: 3
                }
            ],
            node_operator: {
                roles: ['人事'],
            }
        },
        {
            UID: 'manager',
            node_name: "审批",
            to: ['finish'],
            node_operator: {
                roles: ['经理'],
            }
        },
        {
            UID: 'finish',
            node_name: "结束",
        }
    ]
}