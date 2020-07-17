import { RouteView } from '@/layouts'

export default [
    {
        path: '/customer',
        name: 'customer',
        redirect: '/org/org-list',
        component: RouteView,
        meta: { title: '客户管理', keepAlive: false, icon: 'user' },
        children: [
            {
                path: '/customer/org-list',
                name: 'customer-org-list',
                component: () => import('@/views/org/OrgList'),
                meta: { title: '租户列表', keepAlive: false },
                hideChildrenInMenu: true,
                hidden: true,
                children: [
                    {
                        path: '/org/org-list/org-detail',
                        name: 'org-detail',
                        component: () => import('@/views/org/detail'),
                        meta: { title: '客户详情', keepAlive: false },
                    }
                ]
            },
            {
                path: '/customer/customer-list',
                name: 'customer-customer-list',
                component: () => import('@/views/customer/customer'),
                meta: { title: '客户列表', keepAlive: false },
                hideChildrenInMenu: true,
                children: [
                ]
            },
        ]
    },
]