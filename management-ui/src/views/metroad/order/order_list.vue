<template>
  <div>
    <a-card size="small" title="订单列表" :headStyle="{backgroundColor:'aliceblue'}">
      <span slot="extra">
        <router-link :to="{name:'metroad-order-order-list-tf'}">
          <a-button type="dashed" size="small" icon="left">投放列表</a-button>
        </router-link>
      </span>
      <a-card size="small" style="margin-bottom:10px;">
        <a-input v-model="query.user_name" style="width:300px;margin-right:10px;" placeholder="用户"></a-input>
        <span style="margin-right:5px;">下单时间</span>
        <a-range-picker v-model="query.time_range" style="margin-right:10px;" />
        <a-button
          type="primary"
          icon="search"
          style="float:right;"
          @click="()=>{table.pagination.current=1;load_data();}"
        >搜索</a-button>
      </a-card>

      <a-card size="small" style="margin-bottom:10px;background-color:rgb(250,250,250)">
        <a-radio-group :value="query.status" @change="(x)=>{reset_and_load_data(x.target.value)}">
          <a-radio-button v-for="x in status_list" :key="x.key" :value="x.key">{{x.name}}</a-radio-button>
        </a-radio-group>
      </a-card>

      <a-table
        :dataSource="table.data"
        :columns="table.columns"
        :pagination="table.pagination"
        :rowKey="x=>x.UID"
        :loading="table.loading"
        @change="change_page"
        size="small"
      >
        <span slot="time" slot-scope="text,record">
          <div>
            <timeview :time_str="record.AdStartTimeUtc" time_format="YYYY年MM月DD日" />
          </div>
          <div>
            <timeview :time_str="record.AdEndTimeUtc" time_format="YYYY年MM月DD日" />
          </div>
          <div>一共{{record.TotalDays}}天</div>
        </span>
        <span slot="customer" slot-scope="text,record">
          <div v-if="record.User">
            <usericon :data="record.User" />
            <div>{{record.User.UserPhone&&record.User.UserPhone.Phone||'用户未绑定手机'}}</div>
          </div>
        </span>
        <span slot="info" slot-scope="text,record">
          <a-tag>{{[...new Set(record.OrderItems.map(x=>x.AdWindowUID))].length}}</a-tag>
          <a-badge status="default" />
          <a-tag>{{[...new Set(record.OrderItems.map(x=>x.MetroStationUID))].length}}</a-tag>
        </span>
        <span slot="create_time" slot-scope="text,record">
          <timeview :time_str="record.CreateTimeUtc" />
        </span>
        <span slot="remark" slot-scope="text,record">
          <div style="max-width:200px;word-break:break-all;">{{record.CustomerDemand||'--'}}</div>
        </span>
        <span slot="status" slot-scope="text,record">
          <a-tag>{{status_str(record)}}</a-tag>
        </span>
        <span slot="action" slot-scope="text,record">
          <router-link :to="{ name: 'metroad-order-order-detail', params: { uid: record.UID } }">
            <a-button type="primary" icon="eye" size="small">详情</a-button>
          </router-link>
        </span>
      </a-table>
    </a-card>
  </div>
</template>

<script>
import { post_form } from '@/api/ajax'
import { mixin } from '@/utils/mixin'
import timeview from '@/coms/timeview'
import usericon from '@/coms/usericon'
import { get_status_str } from '../utils'
import { parse_date_range } from '@/utils/utils'
import moment from 'moment'
import { status_list, list_columns, order_tab } from './data'

export default {
  components: {
    timeview,
    usericon
  },
  mixins: [mixin],
  data() {
    return {
      status_list: [],
      query: {
        status: '0',
        user_name: '',
        time_range: [],
        start_time_utc: '',
        end_time_utc: ''
      },
      table: {
        loading: false,
        pagination: {
          current: 1,
          pageSize: 1,
          total: 0
        },
        data: [],
        columns: list_columns
      }
    }
  },
  watch: {
    $route: function(val) {
      this.init_page()
    }
  },
  mounted() {
    this.set_status()
    this.init_page()
  },
  methods: {
    set_status() {
      this.status_list = order_tab
    },
    update_tab() {
      var current_status = this.status_list.filter(x => x.key == this.query.status)
      if (current_status && current_status.length > 0) {
        this.$route.meta.title = `订单列表(${current_status[0].name})`
        console.log(this.$route.meta.title)
      }
    },
    init_page() {
      var query_status = this.$route.query.status
      if (this.status_list.map(x => x.key).indexOf(query_status) >= 0) {
        this.query.status = query_status
      }

      //this.update_tab()
      this.load_data()
    },
    expired_data(data) {
      return moment().add(-8, 'hour') > moment(data.AdEndTimeUtc) && data.Status == 5
    },
    status_str(s) {
      var status = s.Status
      if (status === 2) {
        return s.DesignCount > 0 ? '设计待确认' : '设计待上传'
      }
      if (status === 5 && this.expired_data(s)) {
        return '待下刊'
      }
      return get_status_str(status)
    },
    reset_and_load_data(x) {
      this.$router.push({ name: 'metroad-order-order-list', query: { status: x } })
    },
    change_page(pagination) {
      this.table.pagination = { ...pagination }
      this.load_data()
    },
    load_data() {
      this.table.loading = true

      var t = parse_date_range(this.query.time_range)

      this.query.start_time_utc = t.start_time_utc
      this.query.end_time_utc = t.end_time_utc

      var p = {
        page: this.table.pagination.current,
        ...this.copy_data(this.query)
      }

      var status_data = this.first_(this.status_list.filter(x => x.key == p.status))
      if (!status_data) {
        return
      }
      p = {
        ...p,
        ...(status_data.param || {})
      }
      var multi_status = p.multi_status || []
      p.multi_status = JSON.stringify(multi_status)

      delete p.status
      delete p.time_range

      post_form(this, '/api/metro-ad/admin/MetroOrder/QueryOrderByConditions', { ...p })
        .then(res => {
          if (res.Success) {
            this.table.data = res.Data.DataList.map(x => ({ ...x }))
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