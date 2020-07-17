<template>
  <a-card
    :bordered="false"
    :bodyStyle="{ padding: '16px 0', height: '100%' }"
    :style="{ height: '100%' }"
    size="small"
  >
    <div style="padding:10px;">
      <a-menu
        :selectedKeys="selectedKeys"
        mode="horizontal"
        type="inner"
        size="small"
        style="margin-bottom:10px"
      >
        <a-menu-item v-for="(item) in menu_data" :key="item.route_name">
          <router-link :to="{ name: item.route_name }">
            <a-icon :type="item.icon"></a-icon>
            <span>{{item.menu_name}}</span>
          </router-link>
        </a-menu-item>
      </a-menu>
      <route-view></route-view>
    </div>
  </a-card>
</template>

<script>
import { RouteView } from '@/layouts'
import { mixin } from '@/utils/mixin'

export default {
  components: {
    RouteView
  },
  mixins: [mixin],
  data() {
    return {
      selectedKeys: [],
      menu_data: [
        {
          route_name: 'metroad-manage-metro-line',
          menu_name: '地铁线',
          icon: 'home'
        },
        {
          route_name: 'metroad-manage-media-type',
          menu_name: '媒体类型',
          icon: 'home'
        },
        {
          route_name: 'metroad-manage-order-peroid',
          menu_name: '订单周期管理',
          icon: 'shopping-cart'
        }
      ]
    }
  },
  created() {
    this.updateMenu()
  },
  methods: {
    updateMenu() {
      const routes = this.$route.matched.concat()
      this.selectedKeys = [routes.pop().name]
    }
  },
  watch: {
    $route(val) {
      this.updateMenu()
    }
  }
}
</script>