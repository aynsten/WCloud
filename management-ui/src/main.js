// ie polyfill
import '@babel/polyfill'

import Vue from 'vue'
import App from './App.vue'
import store from './store/'
import Router from 'vue-router'
import { asyncRouterMap } from '@/config/router.config'
import { VueAxios } from './utils/request'

import NProgress from 'nprogress'
import 'nprogress/nprogress.css'

import './core/use'
import './utils/filter'

//TODO 隐藏router的报错
const originalPush = Router.prototype.push;
Router.prototype.push = function push(location) {
  return originalPush.call(this, location).catch(err => {
    //
  })
};

//初始化
function Initializer() {
  store.commit('SET_SIDEBAR_TYPE', true)
}

Vue.config.productionTip = false

Vue.use(VueAxios)
Vue.use(Router)

console.log(process.env.BASE_URL)
var router = new Router({
  mode: 'history',
  base: process.env.BASE_URL,
  scrollBehavior: () => ({ y: 0 }),
  routes: [...asyncRouterMap]
})

router.beforeEach((to, from, next) => {
  NProgress.start();
  next()
});
router.afterEach(transition => {
  NProgress.done();
  NProgress.remove();
});

//创建vue实例
new Vue({
  router,
  store,
  created: Initializer,
  render: h => h(App)
}).$mount('#app')
