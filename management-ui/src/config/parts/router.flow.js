import { RouteView } from '@/layouts'

export default [
    {
        path: '/flow',
        name: 'flow',
        redirect: '/flow/flow-list',
        component: RouteView,
        meta: { title: '流程', keepAlive: false, icon: 'branches' },
        children: [
            {
                path: '/flow/flow-index',
                name: 'flow-index',
                redirect: '/flow/flow-list',
                component: () => import('@/views/flow/index'),
                meta: { title: '工作流程', keepAlive: false },
                hideChildrenInMenu: true,
                children: [
                    {
                        path: '/flow/flow-list',
                        name: 'flow-list',
                        component: () => import('@/views/flow/flow_list'),
                        meta: { title: '流程', keepAlive: false },
                    },
                    {
                        path: '/flow/my-flow',
                        name: 'my-flow',
                        component: () => import('@/views/flow/my_flow'),
                        meta: { title: '我发起的', keepAlive: false },
                    },
                    {
                        path: '/flow/assigned_to_me',
                        name: 'flow-assigned-to-me',
                        component: () => import('@/views/flow/assigned_to_me'),
                        meta: { title: '等待处理', keepAlive: false },
                    },
                    {
                        path: '/flow/handled_by_me',
                        name: 'flow-handled-by-me',
                        component: () => import('@/views/flow/handled_by_me'),
                        meta: { title: '流程', keepAlive: false },
                    }
                ]
            },
        ]
    },
]