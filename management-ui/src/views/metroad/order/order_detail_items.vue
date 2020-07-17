<template>
  <div>
    <a-table
      v-if="order.data.OrderItems"
      :dataSource="order.data.OrderItems"
      :columns="order.columns"
      :pagination="false"
      :rowKey="x=>x.UID"
      size="small"
      style="margin-bottom:10px;"
    >
      <span slot="type" slot-scope="text,record">
        <stationType :tp="record.MetroStationType" />
      </span>
      <span slot="size" slot-scope="text,record">{{`${record.Width}cm*${record.Height}cm`}}</span>
    </a-table>
  </div>
</template>

<script>
import { post_form } from '@/api/ajax'
import { mixin } from '@/utils/mixin'
import timeview from '@/coms/timeview'
import upload from '../upload'
import imagepreview from '@/coms/imagepreview'
import stationType from '../station_type'

export default {
  components: {
    timeview,
    upload,
    imagepreview,
    stationType
  },
  mixins: [mixin],
  props: {
    order_data: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      order: {
        data: {},
        columns: [
          {
            title: '线路',
            dataIndex: 'MetroLineName'
          },
          {
            title: '站点',
            dataIndex: 'MetroStationName'
          },
          {
            title: '类型',
            scopedSlots: { customRender: 'type' }
          },
          {
            title: '橱窗',
            dataIndex: 'AdWindowName'
          },
          {
            title: '类型',
            dataIndex: 'MediaTypeName'
          },
          {
            title: '单价',
            dataIndex: 'Price'
          },
          {
            title: '尺寸',
            scopedSlots: { customRender: 'size' }
          }
        ]
      }
    }
  },
  watch: {
    order_data(val) {
      this.order.data = val || {}
    }
  },
  mounted() {
    this.order.data = this.order_data || {}
  },
  computed: {},
  methods: {}
}
</script>