import { BasicLayout, BlankLayout } from '@/layouts'
import account_route from './parts/router.account'
import admin_route from './parts/router.admin'
import customer_route from './parts/router.customer'
import app_route from './parts/router.app'
import dashboard_route from './parts/router.dashboard'
import exception_route from './parts/router.exceptions'
import login_route from './parts/router.login'
import flow_route from './parts/router.flow'
import setting_route from './parts/router.setting'
import metroad_route from './parts/router.metroad'

export const asyncRouterMap = [
  //异常页面
  ...exception_route,
  //登陆注册
  ...login_route,
  {
    path: '/',
    name: 'index',
    component: BasicLayout,
    meta: { title: '首页' },
    redirect: '/dashboard/workplace',
    children: [
      //菜单
      ...dashboard_route,
      ...account_route,
      //...customer_route,
      //...app_route,
      //...flow_route,
      //...setting_route,
      ...metroad_route,
      //...admin_route,
    ]
  },
  {
    //测试页面
    path: '/test',
    component: BlankLayout,
    redirect: '/test/home',
    children: [
      {
        path: '/test/home',
        name: 'TestHome',
        component: () => import('@/views/Home')
      }
    ]
  },
  {
    //默认匹配
    path: '*', redirect: '/404', hidden: true
  }
]

function m(x) {
  if (!x) {
    return '';
  }
  if (x.startsWith(':')) {
    return ':';
  }
  return x;
}

function check(menu) {
  if (!menu.path) {
    console.error('no path', menu);
    return;
  }
  var name = menu.path.split('/').map(m).filter(x => x.length > 0).join('-');

  if (!menu.name) {
    menu.name = name;
    console.log(menu.name);
  }

  (menu.children || []).forEach(x => check(x));
}

asyncRouterMap.forEach(x => check(x));
console.log('menu checked');