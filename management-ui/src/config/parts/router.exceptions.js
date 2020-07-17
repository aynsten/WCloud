
export default [
    {
        path: '/403',
        component: () => import('@/views/exception/403')
    },
    {
        path: '/404',
        component: () => import('@/views/exception/404')
    },
    {
        path: '/500',
        component: () => import('@/views/exception/500')
    },
]