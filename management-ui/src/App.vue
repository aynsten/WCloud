<template>
  <a-locale-provider :locale="locale">
    <div id="app">
      <router-view v-if="routerAlive" />
    </div>
  </a-locale-provider>
</template>

<script>
import zhCN from 'ant-design-vue/lib/locale-provider/zh_CN'
import { mixin } from '@/utils/mixin'

export default {
  mixins: [mixin],
  data() {
    return {
      locale: zhCN,
      routerAlive: true
    }
  },
  provide() {
    return {
      reload: this.reload
    }
  },
  methods: {
    reload() {
      this.routerAlive = false
      this.$nextTick(function() {
        this.routerAlive = true
      })
    }
  },
  created() {
    this.$store.commit('GET_INFO', this)
  }
}
</script>
<style>
#app {
  height: 100%;
}
</style>
