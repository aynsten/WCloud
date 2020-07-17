<template>
  <div>
    <a-card size="small" :bordered="false">
      <a-table
        :dataSource="table.data"
        :columns="table.columns"
        :rowKey="x=>x.UID"
        :pagination="table.pagination"
        :loading="table.loading"
        @change="change_page"
        size="small"
      >
        <span slot="status" slot-scope="text, record">
          <a-badge v-if="record.Status===0" status="default" text="未开始" />
          <a-badge v-if="record.Status===1" status="processing" text="运行中" />
          <a-badge v-if="record.Status===2" status="success" text="处理成功" />
          <a-badge v-if="record.Status===3" status="error" text="处理失败" />
        </span>
      </a-table>
    </a-card>
  </div>
</template>

<script>
import { post_form } from '@/api/ajax'
import { mixin } from '@/utils/mixin'

export default {
  mixins: [mixin],
  data() {
    return {
      table: {
        condition: {
          q: ''
        },
        pagination: {
          current: 1,
          pageSize: 1,
          total: 0
        },
        loading: false,
        columns: [
          {
            width: '200px',
            title: '任务id',
            dataIndex: 'JobKey'
          },
          {
            width: '200px',
            title: '描述',
            dataIndex: 'Desc'
          },
          {
            width: '200px',
            title: '交换机',
            dataIndex: 'Exchange'
          },
          {
            width: '200px',
            title: '路由id',
            dataIndex: 'RoutingKey'
          },
          {
            width: '200px',
            title: '队列',
            dataIndex: 'Queue'
          },
          {
            width: '200px',
            title: '开始时间',
            dataIndex: 'StartTimeUtc'
          },
          {
            width: '200px',
            title: '结束时间',
            dataIndex: 'EndTimeUtc'
          },
          {
            width: '200px',
            title: '状态',
            scopedSlots: { customRender: 'status' }
          }
        ],
        data: []
      }
    }
  },
  mounted() {
    this.query_all()
  },
  computed: {},
  methods: {
    change_page(pagination) {
      this.table.pagination = { ...pagination }
      this.query_all()
    },
    query_all() {
      this.table.loading = true
      post_form(this, '/api/member/admin/common/Queue/Query', {
        ...this.table.condition,
        page: this.table.pagination.current
      })
        .then(res => {
          if (res.Success) {
            this.table.data = res.Data.DataList
            this.table.pagination.pageSize = res.Data.PageSize
            this.table.pagination.total = res.Data.ItemCount
          } else {
            alert(res.ErrorMsg)
          }
        })
        .finally(() => {
          this.table.loading = false
        })
    }
  }
}
</script>
