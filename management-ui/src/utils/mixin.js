// import Vue from 'vue'
import { deviceEnquire, DEVICE_TYPE } from './device'
import { mapState } from 'vuex'

const mixin = {
  computed: {
    ...mapState({
      sidebarOpened: state => state.app.sidebar,
      device: state => state.app.device,
      my_permissions: state => state.user.permissions,
    })
  },
  methods: {
    has_permission(key) {
      var res = this.my_permissions.indexOf(key) >= 0;
      if (!res) {
        console.log('当前用户没有权限：', key);
      }
      return res;
    },
    str_arr_fingle_print(data) {
      data = data || [];
      var res = [...data].map(x => (x || '')).sort((a, b) => a.localeCompare(b)).join('->');
      return res;
    },
    find_in_tree(data, callback) {
      var repeat = [];

      var find = function (node) {
        if (repeat.indexOf(node.key) >= 0) {
          return
        }
        repeat.push(node.key);

        if (callback) {
          callback(node)
        }

        var children = node.children || [];
        children.forEach(m => find(m))
      };

      if (!this.empty(data)) {
        data.forEach(x => find(x));
      }

      return repeat;
    },
    first_(data) {
      if (this.not_empty(data)) {
        return data[0];
      }
      else {
        return null;
      }
    },
    not_empty(data) {
      return data && data.length > 0
    },
    empty(data) {
      return !this.not_empty(data)
    },
    not_empty_arr(data) {
      return data && data.length > 0
    },
    empty_arr(data) {
      return !this.not_empty(data)
    },
    to_json(data) {
      return JSON.stringify(data);
    },
    parse_json(data) {
      return JSON.parse(data);
    },
    copy_data(data) {
      return this.parse_json(this.to_json(data))
    },
    node_available(item) {
      return item.raw_data.Level >= 1
    },
    node_editable(item) {
      return item.raw_data.Level >= 1
    },
    node_deleteable(item) {
      return this.empty(item.children) && item.raw_data.Level >= 1
    },
    isMobile() {
      return this.device === DEVICE_TYPE.MOBILE
    },
    isDesktop() {
      return this.device === DEVICE_TYPE.DESKTOP
    },
    isTablet() {
      return this.device === DEVICE_TYPE.TABLET
    },
  },
  mounted() {
    const { $store } = this
    deviceEnquire(deviceType => {
      switch (deviceType) {
        case DEVICE_TYPE.DESKTOP:
          $store.commit('TOGGLE_DEVICE', 'desktop')
          $store.commit('SET_SIDEBAR_TYPE', true)
          break
        case DEVICE_TYPE.TABLET:
          $store.commit('TOGGLE_DEVICE', 'tablet')
          $store.commit('SET_SIDEBAR_TYPE', false)
          break
        case DEVICE_TYPE.MOBILE:
        default:
          $store.commit('TOGGLE_DEVICE', 'mobile')
          $store.commit('SET_SIDEBAR_TYPE', true)
          break
      }
    })
  }
}

export { mixin }
