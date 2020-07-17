<template>
  <a-layout :class="['layout', device]">
    <!-- SideMenu -->
    <a-drawer
      v-if="isMobile()"
      placement="left"
      :wrapClassName="`drawer-sider ${navTheme}`"
      :closable="false"
      :visible="collapsed"
      @close="drawerClose"
    >
      <side-menu
        mode="inline"
        :menus="menus"
        :theme="navTheme"
        :collapsed="false"
        :collapsible="true"
        @menuSelect="menuSelect"
      ></side-menu>
    </a-drawer>

    <side-menu
      v-else
      mode="inline"
      :menus="menus"
      :theme="navTheme"
      :collapsed="collapsed"
      :collapsible="true"
    ></side-menu>

    <a-layout
      :class="['sidemenu', `content-width-Fluid`]"
      :style="{ paddingLeft: '0', minHeight: '100vh' }"
    >
      <!-- layout header -->
      <transition name="showHeader">
        <div class="header-animat">
          <a-layout-header
            :class="[ sidebarOpened ? 'ant-header-side-opened' : 'ant-header-side-closed', ]"
            :style="{ padding: '0' }"
          >
            <div class="header">
              <a-icon class="trigger" :type="icon_type()" @click="toggle" />
              <user-menu></user-menu>
            </div>
          </a-layout-header>
        </div>
      </transition>

      <!-- layout content -->
      <a-layout-content :style="{ height: '100%', margin: '24px 24px 0'}">
        <multi-tab v-if="multiTab"></multi-tab>
        <transition name="page-transition">
          <route-view />
        </transition>
      </a-layout-content>

      <!-- layout footer -->
      <a-layout-footer>
        <global-footer v-show="false" />
      </a-layout-footer>
    </a-layout>
  </a-layout>
</template>

<script>
import config from '@/config/settings'
import UserMenu from '@/components/tools/UserMenu'
import RouteView from './RouteView'
import MultiTab from '@/components/MultiTab'
import SideMenu from '@/components/Menu/SideMenu'
import GlobalFooter from '@/components/GlobalFooter'

import { mixin } from '@/utils/mixin'
import { asyncRouterMap } from '@/config/router.config.js'

export default {
  name: 'BasicLayout',
  mixins: [mixin],
  components: {
    RouteView,
    MultiTab,
    UserMenu,
    SideMenu,
    GlobalFooter
  },
  data() {
    return {
      collapsed: false,
      menus: [],
      navTheme: config.navTheme,
      multiTab: config.multiTab
    }
  },
  watch: {
    sidebarOpened(val) {
      this.collapsed = !val
    },
    '$store.state.user.permissions': function(val) {
      console.log('permission changed')
      this.set_menu()
    }
  },
  created() {},
  mounted() {
    this.set_menu()
    this.collapsed = !this.sidebarOpened

    const userAgent = navigator.userAgent
    if (userAgent.indexOf('Edge') >= 0) {
      this.$nextTick(() => {
        this.collapsed = !this.collapsed
        setTimeout(() => {
          this.collapsed = !this.collapsed
        }, 16)
      })
    }
  },
  methods: {
    set_menu() {
      var __filter__ = x => {
        if (this.not_empty(x.children)) {
          x.children.forEach(c => __filter__(c))
        }

        var p = x.permission
        if (this.not_empty(p)) {
          var some_no_match = p.some(d => !this.has_permission(d))
          x.hidden = some_no_match
        }

        if (this.not_empty(x.children)) {
          if (x.children.filter(x => !x.hidden).length <= 0) {
            x.hidden = true
          }
        }
      }

      var m = asyncRouterMap.find(item => item.path === '/').children

      m = this.copy_data(m)

      m.forEach(x => __filter__(x))

      console.log(m)

      this.menus = m
    },
    icon_type() {
      if (this.isDesktop()) {
        return this.collapsed ? 'menu-fold' : 'menu-unfold'
      } else {
        return this.collapsed ? 'menu-unfold' : 'menu-fold'
      }
    },
    triggerWindowResizeEvent() {
      const event = document.createEvent('HTMLEvents')
      event.initEvent('resize', true, true)
      event.eventType = 'message'
      window.dispatchEvent(event)
    },
    toggle() {
      this.collapsed = !this.collapsed
      this.$store.commit('SET_SIDEBAR_TYPE', !this.collapsed)
      this.triggerWindowResizeEvent()
    },
    menuSelect() {
      if (!this.isDesktop()) {
        this.collapsed = false
      }
    },
    drawerClose() {
      this.collapsed = false
    }
  }
}
</script>

<style lang="less">
@import url('../components/global.less');

.header-animat {
  position: relative;
  z-index: @ant-global-header-zindex;
}
.showHeader-enter-active {
  transition: all 0.25s ease;
}
.showHeader-leave-active {
  transition: all 0.5s ease;
}
.showHeader-enter,
.showHeader-leave-to {
  opacity: 0;
}
/*
 * The following styles are auto-applied to elements with
 * transition="page-transition" when their visibility is toggled
 * by Vue.js.
 *
 * You can easily play with the page transition by editing
 * these styles.
 */

.page-transition-enter {
  opacity: 0;
}

.page-transition-leave-active {
  opacity: 0;
}

.page-transition-enter .page-transition-container,
.page-transition-leave-active .page-transition-container {
  -webkit-transform: scale(1.1);
  transform: scale(1.1);
}
</style>
