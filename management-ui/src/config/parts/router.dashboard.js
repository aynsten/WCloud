import { RouteView } from '@/layouts'

export default [
    {
        path: '/dashboard',
        name: 'dashboard',
        redirect: '/dashboard/workplace',
        component: RouteView,
        meta: { title: '仪表盘', keepAlive: true, icon: 'home' },
        hidden: true,
        children: [
            {
                path: '/dashboard/analysis',
                name: 'Analysis',
                component: () => import('@/views/dashboard/Analysis'),
                meta: { title: '分析页', keepAlive: false }
            },
            {
                path: '/dashboard/workplace',
                name: 'Workplace',
                component: () => import('@/views/dashboard/Workplace'),
                meta: { title: '工作台', keepAlive: true }
            },
            {
                path: '/dashboard/editor',
                name: 'editor',
                component: () => import('@/views/other/editor'),
                meta: { title: '富文本', keepAlive: true }
            },
            {
                path: '/dashboard/tree',
                name: 'tree',
                component: () => import('@/views/other/tree'),
                meta: { title: 'tree', keepAlive: true }
            },
            {
                path: '/dashboard/drag',
                name: 'drag',
                component: () => import('@/views/other/drag'),
                meta: { title: 'drag', keepAlive: true }
            },
            {
                path: '/dashboard/drag_',
                name: 'drag_',
                component: () => import('@/views/other/drag_'),
                meta: { title: 'drag_', keepAlive: true }
            },
            {
                path: '/dashboard/graph',
                name: 'graph',
                component: () => import('@/views/other/graph'),
                meta: { title: 'graph', keepAlive: true }
            },
            {
                path: '/dashboard/calendar',
                name: 'calendar',
                component: () => import('@/views/other/cal'),
                meta: { title: 'calendar', keepAlive: true }
            }
        ]
    }
]