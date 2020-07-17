<template>
  <div>
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

      <a-select v-model="select.line_uid" @change="change" style="width:150px;margin-right:10px;">
        <a-select-option :key="-1" value>选择路线</a-select-option>
        <a-select-option v-for="(x,i) in data" :key="i" :value="x.UID">{{x.Name}}</a-select-option>
      </a-select>
      <a-select
        v-model="select.station_type"
        @change="change"
        style="width:150px;margin-right:10px;"
      >
        <a-select-option :key="-1" :value="-1">选择类型</a-select-option>
        <a-select-option v-for="(x,i) in station_types" :key="i" :value="x.key">{{x.name}}</a-select-option>
      </a-select>
      <a-select
        v-model="select.station_uid"
        :loading="select.loading"
        :disabled="matched_stations.length<=0"
        style="width:150px;margin-right:10px;"
      >
        <a-select-option :key="-1" value>选择站点</a-select-option>
        <a-select-option v-for="(x,i) in matched_stations" :key="i" :value="x.UID">{{x.Name}}</a-select-option>
      </a-select>
      <a-button type="primary" icon="search" :loading="query.loading" @click="query_data">查询</a-button>
    </a-card>

    <div v-if="not_empty(month_list)">
      <a-card
        v-for="(x,i) in month_list"
        :key="i"
        :title="x.month"
        :loading="x.loading"
        size="small"
        style="margin-bottom:10px;"
      >
        <span slot="extra">{{x.data.days&&`本月一共${x.data.days}天`}}</span>
        <adwindowTable :data="x.data.data" />
      </a-card>
    </div>
    <a-empty v-else style="margin:30px 0;" />
  </div>
</template>

<script>
import { RouteView } from '@/layouts'
import { mixin } from '@/utils/mixin'
import { post_form } from '@/api/ajax'
import adwindowTable from './adwindow_table'
import { station_types } from '../utils'
import moment from 'moment'

export default {
  components: {
    RouteView,
    adwindowTable
  },
  mixins: [mixin],
  data() {
    return {
      data: [],
      loading: false,
      station_types: station_types,
      select: {
        loading: false,
        line_uid: '',
        station_type: -1,
        station_uid: ''
      },
      query: {
        loading: false,
        time_range: []
      },
      month_list: []
    }
  },
  computed: {
    matched_stations() {
      var lines = this.data.filter(x => x.UID === this.select.line_uid)
      if (lines.length <= 0) {
        return []
      }

      var all_stations = lines[0].Stations

      if (this.select.station_type !== -1) {
        all_stations = all_stations.filter(x => x.StationType === this.select.station_type)
      }
      return all_stations
    }
  },
  created() {
    this.load_data()
  },
  methods: {
    flatData(data) {
      return data.flatMap(x => x.Stations).flatMap(x => x.AdWindows)
    },
    change() {
      var _this = this
      _this.select.station_uid = ''
      _this.select.loading = true
      setTimeout(function() {
        _this.select.loading = false
      }, 500)
    },
    async query_data() {
      console.log(this.query.time_range)
      this.month_list = []
      if (!this.query.time_range || this.query.time_range.length !== 2) {
        alert('请选择日期')
        return
      }
      var [start, end] = this.query.time_range
      var month_list = []
      for (var d = start; d <= end; d = moment(d).add(1, 'month')) {
        month_list.push(moment(d))
      }
      month_list = month_list.map(x => ({
        month: x.format('YYYY-MM'),
        date: x,
        loading: false,
        data: {
          data: []
        }
      }))
      if (month_list.length > 12) {
        alert('时间跨度不能太大')
        return
      }
      this.month_list = month_list
      this.query.loading = true
      await new Promise(async (resolve, reject) => {
        for (var i = 0; i < this.month_list.length; ++i) {
          await this.load_month_data(this.month_list[i])
        }
        resolve(true)
      }).finally(() => {
        this.query.loading = false
      })
    },
    async load_month_data(data) {
      try {
        data.loading = true
        var p = {
          month: data.month,
          line_uid: this.select.line_uid,
          station_type: null,
          station_uid: this.select.station_uid,
          timespan: 8
        }
        if (this.select.station_type != -1) {
          p.station_type = this.select.station_type
        }
        var res = await post_form(this, '/api/metro-ad/admin/MetroStatistic/QueryAdWindowMonthlyUsage', p)
        data.data = res.Data || {}
        //console.log(data.data)
      } finally {
        data.loading = false
      }
    },
    async load_data() {
      this.loading = true
      await post_form(this, '/api/metro-ad/admin/MetroAdWindow/AllAdWindows', {})
        .then(res => {
          var data = res.Data
          this.data = data
        })
        .finally(() => {
          this.loading = false
        })
    }
  }
}
</script>