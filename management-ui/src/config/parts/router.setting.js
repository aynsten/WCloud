import { RouteView } from '@/layouts'

export default [
    {
        path: '/setting',
        name: 'setting',
        redirect: '/setting/manage/tag-manage',
        component: RouteView,
        meta: { title: '设置', keepAlive: true, icon: 'setting' },
        children: [
            {
                path: '/setting/manage',
                name: 'setting-manage-menu',
                redirect: '/setting/manage/tag-manage',
                meta: { title: '系统设置', keepAlive: true },
                hideChildrenInMenu: true,
                component: () => import('@/views/setting/index'),
                children: [
                    {
                        path: '/setting/manage/product-manage',
                        name: 'product-manage',
                        component: () => import('@/views/setting/product'),
                        meta: { title: '商品管理', keepAlive: false }
                    },
                    {
                        path: '/setting/manage/tag-manage',
                        name: 'tag-manage',
                        component: () => import('@/views/setting/tag'),
                        meta: { title: '标签管理', keepAlive: false }
                    },
                    {
                        path: '/setting/manage/supplier-manage',
                        name: 'supplier-manage',
                        component: () => import('@/views/setting/supplier'),
                        meta: { title: '供应商管理', keepAlive: false }
                    },
                    {
                        path: '/setting/manage/warehouse-manage',
                        name: 'warehouse-manage',
                        component: () => import('@/views/setting/warehouse'),
                        meta: { title: '仓库管理', keepAlive: false }
                    },
                    {
                        path: '/setting/manage/unit-manage',
                        name: 'unit-manage',
                        component: () => import('@/views/setting/unit'),
                        meta: { title: '计量单位', keepAlive: false }
                    },
                    {
                        path: '/setting/manage/queue-manage',
                        name: 'queue-manage',
                        component: () => import('@/views/setting/queue'),
                        meta: { title: '消息队列', keepAlive: false }
                    },
                    {
                        path: '/setting/manage/cache-manage',
                        name: 'cache-manage',
                        component: () => import('@/views/setting/cache'),
                        meta: { title: '缓存', keepAlive: false }
                    },
                ]
            },
        ]
    },
]