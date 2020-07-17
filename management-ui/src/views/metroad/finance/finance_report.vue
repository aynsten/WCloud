<template>
  <div>
    <a-card size="small">
      <a-card size="small" style="margin-bottom:10px;">
        <a-range-picker
          :value="query.time_range"
          :placeholder="['开始月份', '结束月份']"
          format="YYYY-MM"
          :mode="['month','month']"
          @panelChange="(x,m)=>{query.time_range=x;}"
          @change="(x,m)=>{query.time_range=x;}"
          style="margin-right:10px;"
        />
        <a-button type="primary" icon="search" style="float:right;" @click="load_data()">查询</a-button>
      </a-card>
      <a-table
        :dataSource="table.data"
        :columns="table.columns"
        :rowKey="x=>`${x.Year}-${x.Month}`"
        :loading="table.loading"
        :pagination="false"
        size="small"
      >
        <span slot="detail" slot-scope="text,record">
          <a-button
            type="primary"
            size="small"
            icon="api"
            @click="go_detail(record.Year,record.Month)"
          >详情</a-button>
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
import { report_columns } from './data'
import { parse_month_range } from '@/utils/utils'

export default {
  components: {
    timeview
  },
  mixins: [mixin],
  data() {
    return {
      query: {
        time_range: []
      },
      table: {
        loading: false,
        data: [],
        columns: report_columns
      }
    }
  },
  methods: {
    go_detail(year, month) {
      var time_str = `${year}-${month.toString().padStart(2, '0')}-01`
      var start = moment(time_str)
      var end = moment(time_str).add(1, 'month')
      this.$router.push({
        name: 'metroad-finance-finance-flow',
        query: {
          start_time: start.format('YYYY-MM-DD'),
          end_time: end.format('YYYY-MM-DD')
        }
      })
    },
    load_data() {
      this.table.loading = true
      console.log(this.query.time_range)
      var date = parse_month_range(this.query.time_range)
      var p = {
        ...date
      }
      post_form(this, '/api/metro-ad/admin/MetroFinanceFlow/QueryReport', p)
        .then(res => {
          this.table.data = res.Data
        })
        .finally(() => {
          this.table.loading = false
        })
    }
  },
  created() {
    this.load_data()
  }
}
</script>