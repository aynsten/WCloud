<template>
  <div>
    <a-modal v-model="approve.show" title="订单审核">
      <a-form v-if="!approve.data.approved">
        <a-form-item label="理由">
          <a-textarea v-model="approve.data.comment" @change="max_len"></a-textarea>
        </a-form-item>
      </a-form>
      <span v-else>确认审核通过吗？</span>
      <span slot="footer">
        <a-button
          type="primary"
          size="small"
          :loading="approve.loading"
          @click="handle_approve"
        >{{approve.data.approved?'同意':'驳回'}}</a-button>
      </span>
    </a-modal>
  </div>
</template>

<script>
import { post_form } from '@/api/ajax'
import { mixin } from '@/utils/mixin'
import timeview from '@/coms/timeview'
import upload from '../upload'
import imagepreview from '@/coms/imagepreview'
import stationType from '../station_type'
import orderItemsCom from './order_detail_items'
import orderInfo from './order_detail_info'
import orderPayment from './order_detail_payment'

export default {
  components: {
    timeview,
    upload,
    imagepreview,
    stationType,
    orderItemsCom,
    orderInfo,
    orderPayment
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
      approve: {
        loading: false,
        show: false,
        data: {}
      },
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
    }
  },
  methods: {
    max_len(d) {
      var v = this.approve.data.comment || ''
      this.approve.data.comment = v.slice(0, 30)
    },
    show_approve_form(approved) {
      this.approve.data = {
        comment: '',
        approved: approved
      }
      this.approve.show = true
    },
    handle_approve() {
      this.approve.loading = true
      post_form(this, '/api/metro-ad/admin/MetroOrder/ApproveOrder', {
        approved: this.approve.data.approved,
        comment: this.approve.data.comment,
        order_uid: this.order.data.UID
      })
        .then(res => {
          if (res.Success) {
            this.approve.show = false
            this.$emit('change')
          } else {
            alert(res.ErrorMsg)
          }
        })
        .finally(() => {
          this.approve.loading = false
        })
    }
  }
}
</script>