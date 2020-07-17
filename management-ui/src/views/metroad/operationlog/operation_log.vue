<template>
  <div>
    <a-card size="small">
      <a-table
        :dataSource="data_list"
        :columns="columns"
        :pagination="false"
        :rowKey="x=>x.UID"
        size="small"
      >
        <span slot="account_type" slot-scope="text,record">
          <a-tag v-if="record.AccountType===0" color="red">员工</a-tag>
          <a-tag v-else-if="record.AccountType===1">用户</a-tag>
          <a-tag v-else>未知</a-tag>
        </span>
        <span slot="time" slot-scope="text,record">
          <timeview :time_str="record.CreateTimeUtc" />
        </span>
        <p slot="expandedRowRender" slot-scope="record" style="margin: 0">{{record.ExtraDataJson}}</p>
      </a-table>
      <a-button
        type="dashed"
        size="small"
        icon="arrow-down"
        style="margin-top:10px;"
        block
        :loading="loading"
        @click="load_more"
      >加载更多</a-button>
    </a-card>
  </div>
</template>
<script>
import { post_form } from '@/api/ajax'
import { mixin } from '@/utils/mixin'
import timeview from '@/coms/timeview'
import { columns } from './data'

export default {
  components: {
    timeview
  },
  mixins: [mixin],
  data() {
    return {
      loading: false,
      data_list: [],
      columns: columns
    }
  },
  mounted() {
    this.load_more()
  },
  methods: {
    load_more() {
      var p = {}
      if (this.data_list.length > 0) {
        p.min_id = Math.min(...this.data_list.map(x => x.Id))
      }
      this.loading = true
      post_form(this, '/api/metro-ad/admin/MetroOperationLog/QueryByMinID', p)
        .then(res => {
          var ids = this.data_list.map(x => x.Id)
          var data = res.Data.filter(x => ids.indexOf(x.Id) < 0)
          if (data.length <= 0) {
            this.$message.info('没有更多内容')
          } else {
            this.data_list = [...this.data_list, ...data]
          }
        })
        .finally(() => {
          this.loading = false
        })
    }
  }
}
</script>