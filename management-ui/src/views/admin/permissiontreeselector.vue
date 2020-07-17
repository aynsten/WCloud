<template>
  <div>
    <a-spin tip="Loading..." :spinning="this.loading">
      <searchtree :data="permissions" :checked="selected" @check="(x)=>{check_box(x);}" />
    </a-spin>
  </div>
</template>

<script>
import searchtree from '@/coms/searchtree_'
import { post_form } from '@/api/ajax'
import { mixin } from '@/utils/mixin'

export default {
  components: {
    searchtree
  },
  mixins: [mixin],
  props: {
    selected_uids: {
      type: Array,
      default: () => []
    }
  },
  data() {
    return {
      loading: true,
      permissions: [],
      selected: []
    }
  },
  methods: {
    query_data() {
      post_form(this, '/api/member/admin/Permission/QueryAllPermissions', {})
        .then(res => {
          this.permissions = res.Data
          this.find_in_tree(this.permissions, node => {
            node.title = node.raw_data.Description || node.title
          })
        })
        .finally(() => {
          this.loading = false
        })
    },
    check_box(x) {
      this.$emit('on_select', x)
    }
  },
  created() {
    this.selected = this.selected_uids
    this.query_data()
  },
  watch: {
    selected_uids: function(cur, old) {
      this.selected = cur
    }
  }
}
</script>