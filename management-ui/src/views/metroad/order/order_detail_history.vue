<template>
  <div>
    <a-card
      v-if="not_empty(order.data.OrderHistories)"
      size="small"
      title="历史记录"
      :loading="false"
      style="margin-bottom:10px;"
    >
      <a-timeline>
        <a-timeline-item v-for="(item,i) in order.data.OrderHistories" :key="i">
          <div>
            <timeview :time_str="item.CreateTimeUtc" />
          </div>
          <a-tag>{{parse_status_str(item.Status)}}</a-tag>
        </a-timeline-item>
      </a-timeline>
    </a-card>
  </div>
</template>
<script>
import { post_form } from '@/api/ajax'
import { mixin } from '@/utils/mixin'
import timeview from '@/coms/timeview'
import upload from '../upload'
import imagepreview from '@/coms/imagepreview'
import { get_status_str } from '../utils'

export default {
  components: {
    timeview,
    upload,
    imagepreview
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
        data: {}
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
  computed: {
    s() {
      return this.order.data.Status
    },
    status_str() {
      return get_status_str(this.s)
    }
  },
  methods: {
    parse_status_str(s) {
      return get_status_str(s)
    }
  }
}
</script>