<template>
  <div>
    <orderInfo :order_data="order.data" />
    <a-divider></a-divider>
    <orderItemsCom :order_data="order.data" />
    <a-divider></a-divider>

    <a-row>
      <a-col :span="24">
        <a-button-group style="float:right;">
          <a-tooltip v-if="false">
            <template slot="title">慎用：强制修改状态</template>
            <a-button type="danger" icon="warning" @click="change">修改状态</a-button>
          </a-tooltip>
          <a-button
            v-if="[-1,0].indexOf(s)>=0"
            type="danger"
            icon="minus-circle"
            @click="close_order"
          >关闭订单</a-button>
          <a-button
            v-if="[0].indexOf(s)>=0"
            type="dashed"
            icon="close-circle"
            @click="show_approve_form(false)"
          >驳回</a-button>
          <a-button
            v-if="[0].indexOf(s)>=0"
            type="dashed"
            icon="check"
            @click="show_approve_form(true)"
          >同意</a-button>
          <a-button
            v-if="[1].indexOf(s)>=0"
            type="primary"
            icon="wallet"
            @click="show_payment_form"
          >手动付款</a-button>
        </a-button-group>
      </a-col>
    </a-row>

    <orderApprove ref="order_approve" :order_data="order.data" @change="change" />
    <orderPayment ref="order_payment" :order_data="order.data" @change="change" />
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
import orderApprove from './order_detail_approve'

export default {
  components: {
    timeview,
    upload,
    imagepreview,
    stationType,
    orderItemsCom,
    orderInfo,
    orderPayment,
    orderApprove
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
      close: {
        loading: false
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
    change() {
      this.$emit('change')
    },
    show_payment_form() {
      this.$refs.order_payment.show_payment_form()
    },
    show_approve_form(approved) {
      this.$refs.order_approve.show_approve_form(approved)
    },
    close_order() {
      if (!confirm('确定关闭订单？')) {
        return
      }
      if (!confirm('再次确定关闭订单？')) {
        return
      }
      this.close.loading = true
      post_form(this, '/api/metro-ad/admin/MetroOrder/CloseOrder', {
        order_uid: this.order.data.UID
      })
        .then(res => {
          if (res.Success) {
            this.$emit('change')
          } else {
            alert(res.ErrorMsg)
          }
        })
        .finally(() => {
          this.close.loading = false
        })
    }
  }
}
</script>