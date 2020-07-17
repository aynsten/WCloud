import { RouteView } from '@/layouts'

export default [
    {
        path: '/customer',
        name: 'customer',
        redirect: '/org/org-list',
        component: RouteView,
        meta: { title: '用户管理', keepAlive: false, icon: 'user' },
        permission: ['manage-user'],
        children: [
            {
                path: '/customer/customer-list',
                name: 'customer-customer-list',
                component: () => import('@/views/customer/customer'),
                meta: { title: '用户列表', keepAlive: false },
                hideChildrenInMenu: true,
                children: [
                ]
            },
        ]
    },
    {
        path: '/metroad/order',
        name: 'metroad-order',
        redirect: '/metroad/order/order-list',
        meta: { title: '订单管理', keepAlive: true, icon: 'shopping-cart' },
        //component:()=>import('@/views/metroad/order/index'),
        component: RouteView,
        permission: ['metro-ad-order'],
        children: [
            {
                path: '/metroad/order/order-list',
                name: 'metroad-order-order-list',
                component: () => import('@/views/metroad/order/order_list'),
                hidden: true,
                meta: { title: '订单列表', keepAlive: false }
            },
            {
                path: '/metroad/order/order-list-tf',
                name: 'metroad-order-order-list-tf',
                component: () => import('@/views/metroad/order/order_list_tf'),
                hidden: true,
                meta: { title: '投放列表', keepAlive: false }
            },
            {
                path: '/metroad/order/order-detail/:uid',
                name: 'metroad-order-order-detail',
                component: () => import('@/views/metroad/order/order_detail'),
                meta: { title: '订单详情', keepAlive: false },
                hidden: true
            },
            {
                path: '/metroad/order/order-list-0',
                name: 'metroad-order-order-list-0',
                redirect: { name: 'metroad-order-order-list', query: { status: '0' } },
                meta: { title: '订单列表待审核', keepAlive: false }
            },
            {
                path: '/metroad/order/order-list-1',
                name: 'metroad-order-order-list-1',
                redirect: { name: 'metroad-order-order-list', query: { status: '1' } },
                meta: { title: '订单列表待付款', keepAlive: false }
            },
            {
                path: '/metroad/order/order-list-2',
                name: 'metroad-order-order-list-2',
                redirect: { name: 'metroad-order-order-list', query: { status: '2' } },
                meta: { title: '订单列表已付款', keepAlive: false }
            },
            {
                path: '/metroad/order/order-list--1',
                name: 'metroad-order-order-list--1',
                redirect: { name: 'metroad-order-order-list', query: { status: '-1' } },
                meta: { title: '订单列表已驳回', keepAlive: false }
            },
            {
                path: '/metroad/order/order-list--2',
                name: 'metroad-order-order-list--2',
                redirect: { name: 'metroad-order-order-list', query: { status: '-2' } },
                meta: { title: '订单列表已取消', keepAlive: false }
            },
        ]
    },
    {
        path: '/metroad/statistics',
        name: 'metroad-statistics',
        redirect: { name: 'metroad-order-order-list', query: { status: '4' } },
        meta: { title: '投放统计', keepAlive: false, icon: 'bar-chart' },
        //component:()=>import('@/views/metroad/order/index'),
        hidden: true,
        component: RouteView,
        children: [
            {
                path: '/metroad/statistics/all',
                name: 'metroad-statistics-all',
                redirect: { name: 'metroad-statistics-adwindow' },
                meta: { title: '投放统计', keepAlive: false },
                component: () => import('@/views/metroad/statistics/index'),
                children: [
                    {
                        path: '/metroad/statistics/adwindow',
                        name: 'metroad-statistics-adwindow',
                        meta: { title: '投放统计', keepAlive: false },
                        component: () => import('@/views/metroad/statistics/adwindow_statistics'),
                    }
                ]
            }
        ]
    },
    {
        path: '/metroad/order/tf',
        name: 'metroad-order-tf',
        redirect: { name: 'metroad-order-order-list', query: { status: '4' } },
        meta: { title: '投放管理', keepAlive: true, icon: 'notification' },
        //component:()=>import('@/views/metroad/order/index'),
        permission: ['metro-ad-order-tf'],
        component: RouteView,
        children: [
            {
                path: '/metroad/order/tf-2',
                name: 'metroad-order-tf-2',
                redirect: { name: 'metroad-order-order-list-tf', query: { status: '2_no_design' } },
                meta: { title: '投放列表-设计待上传', keepAlive: true, },
            },
            {
                path: '/metroad/order/tf-2-yes',
                name: 'metroad-order-tf-2-yes',
                redirect: { name: 'metroad-order-order-list-tf', query: { status: '2_has_design' } },
                meta: { title: '投放列表-设计待确认', keepAlive: true, },
            },
            {
                path: '/metroad/order/tf-4',
                name: 'metroad-order-tf-4',
                redirect: { name: 'metroad-order-order-list-tf', query: { status: '4' } },
                meta: { title: '投放列表-待上刊', keepAlive: true, },
            },
            {
                path: '/metroad/order/tf-5',
                name: 'metroad-order-tf-5',
                redirect: { name: 'metroad-order-order-list-tf', query: { status: '5_normal' } },
                meta: { title: '投放列表-投放中', keepAlive: true, },
            },
            {
                path: '/metroad/order/tf-5_expired',
                name: 'metroad-order-tf-5_expired',
                redirect: { name: 'metroad-order-order-list-tf', query: { status: '5_expired' } },
                meta: { title: '投放列表-待下刊', keepAlive: true, },
            },
            {
                path: '/metroad/order/tf-6',
                name: 'metroad-order-tf-6',
                redirect: { name: 'metroad-order-order-list-tf', query: { status: '6' } },
                meta: { title: '投放列表-已完成', keepAlive: true, },
            },
            {
                path: '/metroad/order/tf-statistics',
                name: 'metroad-order-tf-statistics',
                redirect: { name: 'metroad-statistics-adwindow' },
                meta: { title: '投放统计', keepAlive: true, },
            },
        ]
    },
    {
        path: '/metroad',
        name: 'metroad',
        redirect: '/metroad/order/order-list',
        component: RouteView,
        meta: { title: '广告位设置', keepAlive: true, icon: 'setting' },
        permission: ['metro-ad-manage'],
        children: [
            {
                path: '/metroad/manage/metro-line-',
                name: 'metroad-manage-metro-line-',
                redirect: '/metroad/manage/metro-line',
                meta: { title: '广告位管理', keepAlive: false }
            },
            {
                path: '/metroad/manage/media-type-',
                name: 'metroad-manage-media-type-',
                redirect: '/metroad/manage/media-type',
                meta: { title: '媒体类型管理', keepAlive: false }
            },
            {
                path: '/metroad/manage/order-peroid-',
                name: 'metroad-manage-order-peroid-',
                redirect: '/metroad/manage/order-peroid',
                meta: { title: '订单周期管理', keepAlive: false }
            },
            {
                path: '/metroad/manage',
                name: 'metroad-manage',
                redirect: '/metroad/manage/metro-line',
                meta: { title: '广告设置', keepAlive: true },
                hideChildrenInMenu: true,
                hidden: true,
                component: () => import('@/views/metroad/management/index'),
                children: [
                    {
                        path: '/metroad/manage/metro-line',
                        name: 'metroad-manage-metro-line',
                        component: () => import('@/views/metroad/management/metro_line'),
                        meta: { title: '地铁线', keepAlive: false }
                    },
                    {
                        path: '/metroad/manage/media-type',
                        name: 'metroad-manage-media-type',
                        component: () => import('@/views/metroad/management/media_type'),
                        meta: { title: '媒体类型', keepAlive: false }
                    },
                    {
                        path: '/metroad/manage/order-peroid',
                        name: 'metroad-manage-order-peroid',
                        component: () => import('@/views/metroad/management/order_peroid'),
                        meta: { title: '订单周期管理', keepAlive: false }
                    },
                ]
            },
        ]
    },
    {
        path: '/metroad/finance',
        name: 'metroad-finance',
        redirect: '/metroad/finance/finance-flow',
        meta: { title: '财务管理', keepAlive: true, icon: 'stock' },
        //component: () => import('@/views/metroad/finance/index'),
        permission: ['metro-ad-finance'],
        component: RouteView,
        children: [
            {
                path: '/metroad/finance/finance-flow',
                name: 'metroad-finance-finance-flow',
                component: () => import('@/views/metroad/finance/finance_flow'),
                //meta: { title: '交易流水', keepAlive: false },
                meta: { title: '交易日流水', keepAlive: false }
            },
            {
                path: '/metroad/finance/finance-report',
                name: 'metroad-finance-finance-report',
                component: () => import('@/views/metroad/finance/finance_report'),
                //meta: { title: '汇总报告', keepAlive: false },
                meta: { title: '交易流水', keepAlive: false }
            },
        ]
    },
    {
        path: '/metroad/case',
        name: 'metroad-case',
        redirect: '/metroad/case/case-list',
        meta: { title: '案例管理', keepAlive: true, icon: 'picture' },
        //component: () => import('@/views/metroad/showcase/index'),
        permission: ['metro-ad-showcase'],
        component: RouteView,
        children: [
            {
                path: '/metroad/case/case-list',
                name: 'metroad-case-case-list',
                component: () => import('@/views/metroad/showcase/showcase_list'),
                meta: { title: '案例列表', keepAlive: false }
            },
        ]
    },

    {
        path: '/metroad/operationlog',
        name: 'metroad-operationlog',
        redirect: '/metroad/operationlog/operation-log',
        meta: { title: '操作日志', keepAlive: true, icon: 'code' },
        //component: () => import('@/views/metroad/showcase/index'),
        component: RouteView,
        hidden: true,
        children: [
            {
                path: '/metroad/operationlog/operation-log',
                name: 'metroad-operationlog-operation-log',
                component: () => import('@/views/metroad/operationlog/operation_log'),
                meta: { title: '操作日志', keepAlive: false }
            },
        ]
    },
    {
        path: '/user',
        name: 'user',
        redirect: '/user/manage/user-manage',
        component: RouteView,
        meta: { title: '系统设置', keepAlive: true, icon: 'team' },
        children: [
            {
                path: '/user/manage/dept-manage-',
                name: 'dept-manage-',
                redirect: '/user/manage/dept-manage',
                meta: { title: '部门管理', keepAlive: false },
                permission: ['manage-dept'],
            },
            {
                path: '/user/manage/role-manage_-',
                name: 'role-manage_-',
                redirect: '/user/manage/role-manage_',
                meta: { title: '角色管理', keepAlive: false },
                permission: ['manage-role'],
            },
            {
                path: '/user/manage/permission-manage-',
                name: 'permission-manage-',
                redirect: '/user/manage/permission-manage',
                meta: { title: '权限列表', keepAlive: false },
                permission: ['manage-permission'],
            },
            {
                path: '/metroad/operationlog/operation-log-',
                name: 'metroad-operationlog-operation-log-',
                redirect: '/metroad/operationlog/operation-log',
                meta: { title: '操作记录', keepAlive: false },
                permission: ['metro-ad-operationlog'],
            },
            {
                path: '/user/manage',
                name: 'user-manage-menu',
                redirect: '/user/manage/user-manage',
                meta: { title: '组织架构', keepAlive: true },
                hideChildrenInMenu: true,
                hidden: true,
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