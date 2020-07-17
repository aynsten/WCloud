<template>
  <div>
    <a-table
      :dataSource="table.data"
      :columns="table.columns"
      :pagination="table.pagination"
      :rowKey="x=>x.UID"
      :loading="table.loading"
      @change="change_page"
      size="small"
    >
      <span slot="name" slot-scope="text,item">
        <a-badge :status="color(item.Status)" :text="item.Flow.Name||'--'" />
        <span v-show="false">{{item.Flow.Name||'--'}}</span>
      </span>
      <span slot="desc" slot-scope="text,item">{{item.Flow.Description||'--'}}</span>
      <span slot="current" slot-scope="text,item">
        <a-tag v-for="(x,i) in (item.CurrentNodes||[])" :key="i">{{x.Name||'--'}}</a-tag>
      </span>
      <span slot="action" slot-scope="text,item" style="float:right;">
        <a-tooltip v-if="item.Status===0" style="margin-right:10px;">
          <template slot="title">
            <span>开始流程</span>
          </template>
          <a-button
            type="primary"
            icon="play-circle"
            @click="start_flow(item)"
            size="small"
            :loading="item.start_loading"
          ></a-button>
        </a-tooltip>
        <a-button
          type="dashed"
          icon="eye"
          @click="show(item)"
          size="small"
          style="margin-right:10px;"
        ></a-button>
      </span>
    </a-table>
  </div>
</template>

<script>
import { post_form } from '@/api/ajax'

export default {
  data() {
    return {
      table: {
        loading: false,
        start_loading: false,
        pagination: {
          current: 1,
          pageSize: 1,
          total: 0
        },
        data: [],
        columns: [
          {
            title: '流程',
            width: 200,
            scopedSlots: { customRender: 'name' }
          },
          {
            title: '描述',
            scopedSlots: { customRender: 'desc' }
          },
          {
            title: '当前节点',
            scopedSlots: { customRender: 'current' }
          },
          {
            width: 200,
            scopedSlots: { customRender: 'action' }
          }
        ]
      }
    }
  },
  methods: {
    start_flow(item) {
      if (!confirm('确定开启流程')) {
        return
      }
      item.start_loading = true
      post_form(this, '/api/erp/admin/Flow/StartProcessFlow', {
        flow_uid: item.UID
      })
        .then(res => {
          if (res.Success) {
            this.load_my_flow()
          } else {
            alert(res.ErrorMsg)
          }
        })
        .finally(() => {
          item.start_loading = false
        })
    },
    show(item) {},
    color(status) {
      if (status === 0) {
        return 'default'
      }
      if (status === 1) {
        return 'processing'
      }
      if (status === 2) {
        return 'success'
      }
      return 'default'
    },
    change_page(pagination) {
      this.table.pagination = { ...pagination }
      this.load_my_flow()
    },
    load_my_flow() {
      this.table.loading = true
      post_form(this, '/api/erp/admin/Flow/QueryMyFlow', {
        page: this.table.pagination.current
      })
        .then(res => {
          this.table.data = res.Data.DataList.map(x => ({ ...x, start_loading: false }))
          this.table.pagination.pageSize = res.Data.PageSize
          this.table.pagination.total = res.Data.ItemCount
        })
        .finally(() => {
          this.table.loading = false
        })
    }
  },
  created() {
    this.load_my_flow()
  }
}
</script>