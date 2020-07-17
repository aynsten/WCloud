<template>
  <a-card :bordered="false" size="small">
    <span slot="extra">
      <span v-if="false" style="margin-right:10px;">
        <span style="margin-right:8px;">禁用</span>
        <a-tooltip placement="top">
          <template slot="title">
            <span>切换账号状态</span>
          </template>
          <a-switch
            size="small"
            v-model="query_form.isremove"
            :loading="loading"
            @change="()=>{switch_change()}"
          />
        </a-tooltip>
      </span>
    </span>
    <a-spin :spinning="loading">
      <a-table
        size="small"
        :columns="table.columns"
        :dataSource="table.data"
        :pagination="table.pagination"
        @change="handleTableChange"
        :rowKey="record => record.UID"
      >
        <span slot="contact" slot-scope="text, record">
          <a-tag>{{record.UserPhone&&record.UserPhone.Phone||'用户未绑定手机'}}</a-tag>
        </span>
        <span slot="avatar" slot-scope="text, record">
          <a-avatar size="small" :src="record.UserImg||'/admin/logo.png'"></a-avatar>
        </span>
        <span slot="time" slot-scope="text, record">
          <timeview :time_str="record.CreateTimeUtc" />
        </span>
        <span slot="order_count" slot-scope="text,record">
          <a-tag v-if="record.order_count>0">{{record.order_count}}</a-tag>
          <span v-else>--</span>
        </span>
        <span slot="price_count" slot-scope="text,record">
          <a-tag v-if="record.price_count>0">{{record.price_count}}</a-tag>
          <span v-else>--</span>
        </span>
      </a-table>
    </a-spin>
  </a-card>
</template>

<script>
import { post_form } from '@/api/ajax'
import { mixin } from '@/utils/mixin'
import timeview from '@/coms/timeview'

export default {
  components: {
    timeview
  },
  mixins: [mixin],
  data() {
    return {
      loading: false,
      query_form: {
        isremove: false
      },
      table: {
        data: [],
        pagination: {
          total: 0,
          current: 1
        },
        columns: [
          {
            title: '编号',
            width: '50px',
            dataIndex: 'Id'
          },
          {
            title: '用户名',
            width: '150px',
            dataIndex: 'UserName'
          },
          {
            title: '昵称',
            width: '150px',
            dataIndex: 'NickName'
          },
          {
            title: '头像',
            width: '100px',
            scopedSlots: { customRender: 'avatar' }
          },
          {
            title: '手机号',
            width: '250px',
            scopedSlots: { customRender: 'contact' }
          },
          {
            title: '注册时间',
            width: '250px',
            scopedSlots: { customRender: 'time' }
          },
          {
            title: '订单数',
            width: '80px',
            scopedSlots: { customRender: 'order_count' }
          },
          {
            title: '金额',
            width: '80px',
            scopedSlots: { customRender: 'price_count' }
          },
          {
            title: '操作',
            width: '150px',
            scopedSlots: { customRender: 'action' }
          }
        ]
      }
    }
  },
  created() {
    this.getUser()
  },
  methods: {
    switch_change() {
      this.table.pagination.current = 1
      this.getUser()
    },
    handleTableChange(pagination) {
      this.table.pagination = { ...pagination }
      this.getUser()
    },
    getUser() {
      this.loading = true
      post_form(this, '/api/member/admin/Customer/Query_', {
        page: this.table.pagination.current,
        isremove: this.query_form.isremove
      })
        .then(res => {
          this.table.data = res.Data.DataList.map(x => ({ ...x, order_count: 0, price_count: 0 }))
          this.table.pagination.total = res.Data.ItemCount
          //查询订单数
          this.getOrderCount()
        })
        .finally(() => {
          this.loading = false
        })
    },
    getOrderCount() {
      var user_uids = this.table.data.map(x => x.UID)
      if (user_uids.length <= 0) {
        return
      }
      var _this = this
      post_form(this, '/api/metro-ad/admin/MetroOrder/UserOrderCount', {
        data: JSON.stringify(user_uids)
      }).then(res => {
        if (res.Success) {
          this.table.data.forEach(x => {
            var d = res.Data.filter(m => m.UserUID === x.UID)
            var group = _this.first_(d)
            if (group) {
              x.order_count = group.OrderCount
              x.price_count = group.PriceSum
            }
          })
        }
      })
    }
  }
}
</script>
