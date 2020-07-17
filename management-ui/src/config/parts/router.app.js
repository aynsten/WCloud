import { RouteView } from '@/layouts'

export default [
    {
        path: '/app',
        name: 'app',
        redirect: '/app/app-list',
        component: RouteView,
        meta: { title: '应用列表', keepAlive: false, icon: 'appstore' },
        children: [
            {
                path: '/app/app-list',
                name: 'app-list',
                component: () => import('@/views/app/applist'),
                meta: { title: '应用列表', keepAlive: false }
            },
            {
                path: '/app/settings',
                name: 'sysperm',
                component: () => import('@/views/app/settings/Index'),
                meta: { title: '应用配置' },
                redirect: '/app/app-list',
                hideChildrenInMenu: true,
                hidden: true,
                children: [
                    {
                        path: '/app/settings/:app/setting_menu',
                        name: 'setting_menu',
                        component: () => import('@/views/app/settings/menu'),
                        meta: { title: '菜单', keepAlive: false },
                    },
                    {
                        path: '/app/settings/:app/setting_alias',
                        name: 'setting_alias',
                        component: () => import('@/views/app/settings/alias'),
                        meta: { title: '词语', keepAlive: false },
                    },
                    {
                        path: '/app/settings/:app/setting_setting',
                        name: 'setting_setting',
                        component: () => import('@/views/app/settings/setting'),
                        meta: { title: '配置', keepAlive: false },
                    }
                ]
            }
        ]
    }
]