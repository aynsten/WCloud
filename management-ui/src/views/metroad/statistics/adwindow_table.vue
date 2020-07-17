<template>
  <div>
    <div v-if="not_empty(data_list)">
      <a-table
        :dataSource="data_list"
        :columns="columns"
        :rowKey="x=>x.id"
        :pagination="false"
        size="small"
      ></a-table>
    </div>
    <a-empty v-else />
  </div>
</template>

<script>
import { RouteView } from '@/layouts'
import { mixin } from '@/utils/mixin'
import { station_types } from '../utils'

export default {
  components: {
    RouteView
  },
  mixins: [mixin],
  props: {
    data: {
      type: Array,
      default: () => []
    }
  },
  data() {
    return {
      data_list: [],
      columns: [
        {
          title: '线路',
          width: '150px',
          dataIndex: 'LineName'
        },
        {
          title: '站点',
          width: '150px',
          dataIndex: 'StationName'
        },
        {
          title: '站点类型',
          width: '150px',
          dataIndex: 'StationTypeName'
        },
        {
          title: '点位',
          width: '150px',
          dataIndex: 'AdWindowName'
        },
        {
          title: '收益（元）',
          width: '150px',
          dataIndex: 'Price'
        },
        {
          title: '点位投放使用率',
          width: '150px',
          dataIndex: 'Rate',
          customRender: x => `${(x * 100).toFixed(2)}%`
        }
      ]
    }
  },
  created() {
    this.set_ad_windows(this.data)
  },
  methods: {
    set_ad_windows(d) {
      var ad_data = d || []
      var i = 0
      this.data_list = ad_data.map(x => {
        var tp = this.first_(station_types.filter(d => d.key === x.StationType))
        return {
          ...x,
          StationTypeName: (tp || {}).name,
          id: ++i
        }
      })
      console.log(this.data_list)
    }
  },
  watch: {
    data(val) {
      this.set_ad_windows(val)
    }
  }
}
</script>