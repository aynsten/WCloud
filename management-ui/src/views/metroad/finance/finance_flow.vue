<template>
  <div>
    <a-card size="small">
      <a-card size="small" style="margin-bottom:10px;">
        <a-select
          v-model="query.pay_method"
          :loading="query.pay_method_loading"
          style="margin-right:10px;width:200px;"
        >
          <a-select-option :key="-100" :value="-100">全部支付方式</a-select-option>
          <a-select-option
            v-for="(x,i) in query.pay_method_data"
            :key="i"
            :value="x.Value"
          >{{x.Key}}</a-select-option>
        </a-select>
        <a-range-picker v-model="query.time_range" style="margin-right:10px;" />
        <a-button type="primary" icon="search" style="float:right;" @click="load_data()">查询</a-button>
      </a-card>
      <a-table
        :dataSource="table.data"
        :columns="table.columns"
        :rowKey="x=>x.UID"
        :loading="table.loading"
        :pagination="table.pagination"
        size="small"
        @change="page_change"
      >
        <span slot="order_no" slot-scope="text,record">
          <span v-if="record.OrderUID">
            <router-link
              :to="{ name: 'metroad-order-order-detail', params: { uid: record.OrderUID } }"
            >{{record.OrderNo}}</router-link>
          </span>
        </span>
        <span slot="pay_method" slot-scope="text,record">
          <span>{{pay_way_str(record.PayMethod)}}</span>
        </span>
        <span slot="direction" slot-scope="text,record">
          <span>{{record.FlowDirection===0?'入账':'出账'}}</span>
        </span>
        <span slot="time" slot-scope="text,record">
          <timeview :time_str="record.CreateTimeUtc" />
        </span>
        <span slot="status" slot-scope="text,record">
          <a-tag v-if="record" color="green">交易完成</a-tag>
        </span>
      </a-table>
    </a-card>
  </div>
</template>

<script>
import { post_form } from '@/api/ajax'
import { mixin } from '@/utils/mixin'
import timeview from '@/coms/timeview'
import { get_pay_method_str } from '../utils'
import moment from 'moment'
import { parse_date_range, parse_pagination } from '@/utils/utils'
import { columns } from './data'

export default {
  components: {
    timeview
  },
  mixins: [mixin],
  data() {
    return {
      query: {
        pay_method_loading: false,
        pay_method_data: [],
        pay_method: -100,
        time_range: []
      },
      table: {
        pagination: {
          current: 1,
          pageSize: 1,
          total: 0
        },
        loading: false,
        data: [],
        columns: columns
      }
    }
  },
  methods: {
    pay_way_str(s) {
      return get_pay_method_str(s)
    },
    page_change(pagination) {
      this.table.pagination = { ...pagination }
      this.load_data()
    },
    load_data() {
      this.table.loading = true
      var p = {
        page: this.table.pagination.current,
        ...parse_date_range(this.query.time_range)
      }
      if (this.query.pay_method !== -100) {
        p.pay_method = this.query.pay_method
      }
      post_form(this, '/api/metro-ad/admin/MetroFinanceFlow/QueryByConditions', p)
        .then(res => {
          this.table.data = res.Data.DataList.map(x => ({ ...x }))
          this.table.pagination = parse_pagination(this.table.pagination, res)
        })
        .finally(() => {
          this.table.loading = false
        })
    },
    load_pay_method() {
      this.query.pay_method_loading = true
      post_form(this, '/api/metro-ad/admin/MetroFinanceFlow/PayMethod', {})
        .then(res => {
          this.query.pay_method_data = res.Data
        })
        .finally(() => {
          this.query.pay_method_loading = false
        })
    }
  },
  created() {
    var start_time = this.$route.query.start_time
    var end_time = this.$route.query.end_time
    if (start_time && end_time) {
      try {
        var val = [moment(start_time), moment(end_time)]
        this.query.time_range = val
      } catch (e) {
        console.log(e)
      }
    }
    this.load_pay_method()
    this.load_data()
  }
}
</script>