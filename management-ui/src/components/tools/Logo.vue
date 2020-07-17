<template>
  <div class="logo">
    <router-link :to="{name:'dashboard'}">
      <LogoSvg v-show="false" alt="logo" />
      <img src="~@/assets/mj.jpeg" alt />
      <h1 v-if="showTitle">{{ title }}</h1>
    </router-link>
  </div>
</template>

<script>
import LogoSvg from '@/assets/logo.svg?inline'

export default {
  name: 'Logo',
  components: {
    LogoSvg
  },
  props: {
    title: {
      type: String,
      default: '平台管理',
      required: false
    },
    showTitle: {
      type: Boolean,
      default: true,
      required: false
    }
  }
}
</script>
