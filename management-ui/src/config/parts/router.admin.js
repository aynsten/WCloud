import { RouteView } from '@/layouts'

export default [
    {
        path: '/user',
        name: 'user',
        redirect: '/user/manage/user-manage',
        component: RouteView,
        meta: { title: '组织架构', keepAlive: true, icon: 'team' },
        children: [
            {
                path: '/user/manage',
                name: 'user-manage-menu',
                redirect: '/user/manage/user-manage',
                meta: { title: '组织架构', keepAlive: true },
                hideChildrenInMenu: true,
                component: () => import('@/views/admin/index'),
                children: [
                    {
                        path: '/user/manage/user-manage',
                        name: 'user-manage',
                        component: () => import('@/views/admin/user'),
                        meta: { title: '用户管理', keepAlive: false }
                    },
                    {
                        path: '/user/manage/dept-manage',
                        name: 'dept-manage',
                        component: () => import('@/views/admin/dept'),
                        meta: { title: '部门管理', keepAlive: false }
                    },
                    {
                        path: '/user/manage/role-manage_',
                        name: 'role-manage_',
                        component: () => import('@/views/admin/role_'),
                        meta: { title: '角色管理', keepAlive: false }
                    },
                    {
                        path: '/user/manage/permission-manage',
                        name: 'permission-manage',
                        component: () => import('@/views/admin/permission'),
                        meta: { title: '权限', keepAlive: false }
                    }
                ]
            },
        ]
    },
]