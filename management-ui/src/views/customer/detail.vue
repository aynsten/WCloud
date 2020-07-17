<template>
  <a-card :bordered="false" size="small">
    <a-modal style="top: 20px;" :width="800" v-model="show" :afterClose="close">1231321</a-modal>
  </a-card>
</template>

<script>
export default {
  components: {},
  data() {
    return {
      show: true
    }
  },
  created() {
      console.log('come')
  },
  methods: {
    close() {
      this.$router.push({ name: 'org-list' })
      console.log('back')
    }
  }
}
</script>
