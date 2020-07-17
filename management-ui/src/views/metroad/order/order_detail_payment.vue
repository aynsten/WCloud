<template>
  <div>
    <a-modal v-model="payment.show" title="手动设置支付">
      <a-form>
        <a-form-item>
          一共
          <a-tag color="red">{{order.data.TotalPrice}}</a-tag>元
        </a-form-item>
        <a-form-item label="支付方式">
          <a-select v-model="payment.data.pay_method">
            <a-select-option :key="0" :value="0">微信</a-select-option>
            <a-select-option :key="1" :value="1">支付宝</a-select-option>
            <a-select-option :key="3" :value="3">银联</a-select-option>
            <a-select-option :key="4" :value="4">转账</a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="第三方支付号">
          <a-input v-model="payment.data.external_payment_no"></a-input>
        </a-form-item>
        <a-form-item label="上传凭证">
          <upload @change="e=>{payment.data.img=e;}" />
        </a-form-item>
        <a-form-item label="备注">
          <a-textarea v-model="payment.data.comment" @change="max_len"></a-textarea>
        </a-form-item>
      </a-form>
      <span slot="footer">
        <a-button type="primary" size="small" :loading="payment.loading" @click="set_payed">确认</a-button>
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

export default {
  components: {
    timeview,
    upload,
    imagepreview,
    stationType,
    orderItemsCom,
    orderInfo
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
      payment: {
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
      var v = this.payment.data.comment || ''
      this.payment.data.comment = v.slice(0, 30)
    },
    show_payment_form() {
      this.payment.data = {
        pay_method: 0,
        img: [],
        comment: '',
        external_payment_no: ''
      }
      this.payment.show = true
    },
    set_payed() {
      this.payment.loading = true
      post_form(this, '/api/metro-ad/admin/MetroOrder/SetPayed', {
        img: JSON.stringify(this.payment.data.img),
        comment: this.payment.data.comment,
        external_payment_no: this.payment.data.external_payment_no,
        pay_method: this.payment.data.pay_method,
        order_uid: this.order.data.UID
      })
        .then(res => {
          if (res.Success) {
            this.payment.show = false
            this.$emit('change')
          } else {
            alert(res.ErrorMsg)
          }
        })
        .finally(() => {
          this.payment.loading = false
        })
    }
  }
}
</script>