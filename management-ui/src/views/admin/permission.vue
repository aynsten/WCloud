<template>
  <div>
    <a-card :loading="this.loading" size="small" :bordered="false">
      <a-table
        :columns="columns"
        :dataSource="permissions"
        :pagination="false"
        :expandedRowKeys="expandedRowKeys"
        @expandedRowsChange="(e)=>{expandedRowKeys = e;}"
        size="small"
      ></a-table>
    </a-card>
  </div>
</template>

<script>
import { post_form } from '@/api/ajax'
import { mixin } from '@/utils/mixin'

export default {
  components: {},
  mixins: [mixin],
  data() {
    return {
      loading: true,
      expandedRowKeys: [],
      permissions: [],
      columns: [
        {
          title: '权限',
          dataIndex: 'title'
        }
      ]
    }
  },
  methods: {
    query_data() {
      post_form(this, '/api/member/admin/Permission/QueryAllPermissions', {})
        .then(res => {
          this.permissions = res.Data
          this.expandedRowKeys = this.find_in_tree(this.permissions, node => {
            node.title = node.raw_data.Description || node.title
          })
        })
        .finally(() => {
          this.loading = false
        })
    }
  },
  created() {
    this.query_data()
  }
}
</script>