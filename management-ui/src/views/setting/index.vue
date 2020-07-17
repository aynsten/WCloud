<template>
  <div class="page-header-index-wide">
    <a-card
      :bordered="false"
      :bodyStyle="{ padding: '16px 0', height: '100%' }"
      :style="{ height: '100%' }"
    >
      <div class="account-settings-info-main" :class="device">
        <div class="account-settings-info-left">
          <a-menu
            :mode="device == 'mobile' ? 'horizontal' : 'inline'"
            :selectedKeys="selectedKeys"
            type="inner"
            size="small"
          >
            <a-menu-item key="product-manage">
              <router-link :to="{ name: 'product-manage' }">
                <a-icon type="shopping"></a-icon>
                <span>商品管理</span>
              </router-link>
            </a-menu-item>
            <a-menu-item key="unit-manage">
              <router-link :to="{ name: 'unit-manage' }">
                <a-icon type="calculator"></a-icon>
                <span>计量单位</span>
              </router-link>
            </a-menu-item>
            <a-menu-item key="supplier-manage">
              <router-link :to="{ name: 'supplier-manage' }">
                <a-icon type="team"></a-icon>
                <span>供应商</span>
              </router-link>
            </a-menu-item>
            <a-menu-item key="warehouse-manage">
              <router-link :to="{ name: 'warehouse-manage' }">
                <a-icon type="home"></a-icon>
                <span>仓库管理</span>
              </router-link>
            </a-menu-item>
            <a-menu-item key="tag-manage">
              <router-link :to="{ name: 'tag-manage' }">
                <a-icon type="tag"></a-icon>
                <span>标签管理</span>
              </router-link>
            </a-menu-item>
            <a-menu-item key="queue-manage">
              <router-link :to="{ name: 'queue-manage' }">
                <a-icon type="file-sync"></a-icon>
                <span>消息队列</span>
              </router-link>
            </a-menu-item>
            <a-menu-item key="cache-manage">
              <router-link :to="{ name: 'cache-manage' }">
                <a-icon type="clock-circle"></a-icon>
                <span>缓存管理</span>
              </router-link>
            </a-menu-item>
          </a-menu>
        </div>
        <div class="account-settings-info-right">
          <div class="account-settings-info-title">
            <span>{{ $route.meta.title }}</span>
          </div>
          <route-view></route-view>
        </div>
      </div>
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

<style lang="less" scoped>
.account-settings-info-main {
  width: 100%;
  display: flex;
  height: 100%;
  overflow: auto;

  &.mobile {
    display: block;

    .account-settings-info-left {
      border-right: unset;
      border-bottom: 1px solid #e8e8e8;
      width: 100%;
      height: 50px;
      overflow-x: auto;
      overflow-y: scroll;
    }
    .account-settings-info-right {
      padding: 20px 40px;
    }
  }

  .account-settings-info-left {
    border-right: 1px solid #e8e8e8;
    width: 224px;
  }

  .account-settings-info-right {
    flex: 1 1;
    padding: 8px 40px;

    .account-settings-info-title {
      color: rgba(0, 0, 0, 0.85);
      font-size: 20px;
      font-weight: 500;
      line-height: 28px;
      margin-bottom: 12px;
    }
    .account-settings-info-view {
      padding-top: 12px;
    }
  }
}
</style>
