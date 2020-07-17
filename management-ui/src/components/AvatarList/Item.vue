<template>
  <a-tooltip v-if="tips !== ''">
    <template slot="title">{{ tips }}</template>
    <a-avatar :size="avatarSize" :src="src">{{name}}</a-avatar>
  </a-tooltip>
  <a-avatar v-else :size="avatarSize" :src="src">{{name}}</a-avatar>
</template>

<script>
export default {
  name: 'AvatarItem',
  props: {
    tips: {
      type: String,
      default: '',
      required: false
    },
    src: {
      type: String,
      default: ''
    },
    name: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      size: this.$parent.size
    }
  },
  computed: {
    avatarSize() {
      return (this.size !== 'mini' && this.size) || 20
    }
  },
  watch: {
    '$parent.size'(val) {
      this.size = val
    }
  }
}
</script>
