<template>
  <div>
    <a-tag v-if="record.StationType===0">普通站点</a-tag>
    <a-tag v-if="record.StationType===1" color="blue">特色站点</a-tag>
    <a-tag v-if="record.StationType===2" color="red">核心站点</a-tag>
  </div>
</template>

<script>
export default {
  props: {
    tp: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      record: {
        StationType: 0
      }
    }
  },
  watch: {
    tp(val) {
      this.update(val)
    }
  },
  mounted() {
    this.update(this.tp)
  },
  methods: {
    update(xx) {
      this.record.StationType = xx
    }
  }
}
</script>