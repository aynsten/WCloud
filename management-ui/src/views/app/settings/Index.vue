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
            :style="{ border: '0', width: device == 'mobile' ? '560px' : 'auto'}"
            :selectedKeys="selectedKeys"
            type="inner"
            @openChange="onOpenChange"
          >
            <a-menu-item key="setting_menu">
              <router-link :to="{ name: 'setting_menu' }">菜单</router-link>
            </a-menu-item>
            <a-menu-item key="setting_alias">
              <router-link :to="{ name: 'setting_alias' }">词语</router-link>
            </a-menu-item>
            <a-menu-item key="setting_setting">
              <router-link :to="{ name: 'setting_setting' }">配置</router-link>
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
import { mixin } from '@/utils/mixin.js'

export default {
  components: {
    RouteView
  },
  mixins: [mixin],
  data() {
    return {
      // horizontal  inline
      mode: 'inline',

      openKeys: [],
      selectedKeys: [],

      // cropper
      preview: {},
      pageTitle: ''
    }
  },
  created() {
    this.updateMenu()
  },
  methods: {
    onOpenChange(openKeys) {
      this.openKeys = openKeys
    },
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
