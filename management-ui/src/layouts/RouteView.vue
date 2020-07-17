<script>
import config from '@/config/settings'

export default {
  name: 'RouteView',
  props: {
    keepAlive: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {}
  },
  render() {
    const {
      $route: { meta },
      $store: { getters }
    } = this
    const inKeep = (
      <keep-alive>
        <router-view />
      </keep-alive>
    )
    const notKeep = <router-view />
    var keep = this.keepAlive || meta.keepAlive // || config.multiTab
    return keep ? inKeep : notKeep
  }
}
</script>
