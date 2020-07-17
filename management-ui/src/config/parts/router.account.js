import { RouteView } from '@/layouts'

export default [
    {
        path: '/account/settings',
        redirect: '/account/settings/menu',
        component: RouteView,
        meta: { title: '个人资料', keepAlive: true },
        hideChildrenInMenu: true,
        hidden: true,
        children: [
            {
                path: '/account/settings/menu',
                redirect: '/account/settings/profile',
                component: () => import('@/views/account/settings/Index'),
                meta: { title: '个人资料', keepAlive: true },
                hideChildrenInMenu: true,
                children: [
                    {
                        path: '/account/settings/profile',
                        component: () => import('@/views/account/settings/BaseSetting'),
                        meta: { title: '个人资料', keepAlive: false }
                    },
                    {
                        path: '/account/settings/change-pwd',
                        component: () => import('@/views/account/settings/pwd'),
                        meta: { title: '修改密码', keepAlive: false }
                    },
                    {
                        path: '/account/settings/binding',
                        component: () => import('@/views/account/settings/Binding'),
                        meta: { title: '用户管理', keepAlive: true }
                    }
                ]
            },
            {
                path: '/account/center',
                component: () => import('@/views/account/center/Index'),
                meta: { title: '个人中心', keepAlive: true },
                hideChildrenInMenu: true,
            }
        ]
    },
]