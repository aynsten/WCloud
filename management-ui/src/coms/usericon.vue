<template>
  <span>
    <a-avatar :src="user.UserImg||'/admin/logo.png'" :shape="shape" style="margin-right:5px;" />
    <span style="margin-right:5px;">{{user.NickName||user.UserName}}</span>
  </span>
</template>

<script>
export default {
  props: {
    data: {
      type: Object
    },
    show_name: {
      type: Boolean,
      default: true
    },
    icon_shape: {
      type: String,
      default: 'square'
    }
  },
  data() {
    return {
      user: {},
      name: true,
      shape: 'square'
    }
  },
  mounted() {
    this.set_data(this.data)
    this.set_name(this.show_name)
    this.set_shape(this.icon_shape)
  },
  watch: {
    data(val) {
      this.set_data(val)
    },
    show_name(val) {
      this.set_name(val)
    },
    icon_shape(val) {
      this.set_shape(val)
    }
  },
  methods: {
    set_data(d) {
      this.user = d || {}
    },
    set_name(n) {
      this.name = n
    },
    set_shape(s) {
      if (s) {
        this.shape = s
      }
    }
  }
}
</script>