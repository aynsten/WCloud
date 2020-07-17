<template>
  <div>
    <a-card :style="{ height: '100%' }" size="small">
      <a-row>
        <a-col :span="24">
          <a-menu
            :selectedKeys="selectedKeys"
            mode="horizontal"
            size="small"
            style="margin-bottom:10px;"
          >
            <a-menu-item key="flow-list">
              <router-link :to="{ name: 'flow-list' }">
                <a-icon type="menu"></a-icon>
                <span>审批</span>
              </router-link>
            </a-menu-item>
            <a-menu-item key="my-flow">
              <router-link :to="{ name: 'my-flow' }">
                <a-icon type="solution"></a-icon>
                <span>我发起的</span>
              </router-link>
            </a-menu-item>
            <a-menu-item key="flow-assigned-to-me">
              <router-link :to="{ name: 'flow-assigned-to-me' }">
                <a-icon type="flag"></a-icon>
                <span>等待审批</span>
              </router-link>
            </a-menu-item>
            <a-menu-item key="flow-handled-by-me">
              <router-link :to="{ name: 'flow-handled-by-me' }">
                <a-icon type="lock"></a-icon>
                <span>我审批的</span>
              </router-link>
            </a-menu-item>
          </a-menu>
        </a-col>
        <a-col :span="24">
          <div style="display:none;">
            <span>{{ $route.meta.title }}</span>
          </div>
          <route-view></route-view>
        </a-col>
      </a-row>
    </a-card>
  </div>
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
      selectedKeys: []
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