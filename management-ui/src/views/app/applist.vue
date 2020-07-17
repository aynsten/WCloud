<template>
  <div>
    <a-list :grid="grid" :dataSource="dataSource">
      <a-list-item slot="renderItem" slot-scope="item">
        <template>
          <a-card :hoverable="true">
            <img :src="item.Logo" slot="cover" />
            <a-card-meta :title="item.Name" :description="item.Description"></a-card-meta>
            <template class="ant-card-actions" slot="actions">
              <a-icon @click="GotoInfo(item.Key)" type="ellipsis" />
            </template>
          </a-card>
        </template>
      </a-list-item>
    </a-list>
  </div>
</template>

<script>
import { post_form } from '@/api/ajax'

export default {
  data() {
    return {
      grid: { gutter: 24, lg: 4, md: 2, sm: 1, xs: 1 },
      dataSource: []
    }
  },
  methods: {
    GotoInfo(sysid) {
      this.$router.push({ name: 'setting_permission', params: { app: sysid } })
    },
    get_sys() {
      post_form(this, '/api/erp/admin/SubSystem/Query', {}).then(res => {
        if (res.Success) {
          this.dataSource = res.Data
        } else {
          alert(res.ErrorMsg)
        }
      })
    }
  },
  mounted() {
    this.get_sys()
  }
}
</script>

<style lang="less" scoped>
</style>
