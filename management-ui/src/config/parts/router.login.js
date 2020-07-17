import { UserLayout } from '@/layouts'

export default [
    {
        path: '/account',
        component: UserLayout,
        redirect: '/account/login',
        hidden: true,
        children: [
            {
                path: '/account/login',
                name: 'login',
                component: () => import('@/views/account/login/Login')
            },
            {
                path: '/account/register',
                name: 'register',
                component: () => import('@/views/account/login/Register')
            },
            {
                path: '/account/register-result',
                name: 'registerResult',
                component: () => import('@/views/account/login/RegisterResult')
            }
        ]
    },
]