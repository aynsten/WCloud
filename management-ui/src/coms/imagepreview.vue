<template>
  <span class="dd">
    <a-avatar
      :src="src||large_src||'/admin/logo.png'"
      :size="size"
      shape="square"
      @click="()=>{show=true}"
    ></a-avatar>
    <a-modal v-model="show" title="预览" :width="700" :footer="false">
      <img alt style="width: 100%" :src="large_src||src" />
    </a-modal>
  </span>
</template>

<script>
import { post_form, post_form_data } from '@/api/ajax'
import { mixin } from '@/utils/mixin'

export default {
  mixins: [mixin],
  props: {
    src: {
      type: String,
      default: () => [],
      required: true
    },
    large_src: {
      type: String,
      default: null
    },
    size: {
      type: Number,
      default: 150
    }
  },
  data() {
    return {
      show: false
    }
  },
  watch: {
    //
  },
  mounted() {
    //
  },
  methods: {}
}
</script>

<style scoped>
.dd {
  background-color: aliceblue;
}
</style>