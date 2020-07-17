<template>
  <div>
    <a-row :gutter="10">
      <a-col :span="16">
        <a-card
          bordered
          size="small"
          :title="`详情（${status_str}）`"
          :loading="order.loading"
          style="margin-bottom:10px;"
        >
          <span slot="extra">
            <a-button type="dashed" size="small" icon="left" @click="go_home">返回列表</a-button>
          </span>
          <orderSummary :order_data="order.data" @change="change" />
        </a-card>
        <a-card
          v-if="[2,4,5,6].indexOf(s)>=0"
          bordered
          size="small"
          title="设计稿"
          :loading="order.loading"
          style="margin-bottom:10px;"
        >
          <orderDesign :order_data="order.data" @change="change" />
        </a-card>
        <a-card
          v-if="[4,5,6].indexOf(s)>=0"
          bordered
          size="small"
          title="上刊"
          :loading="order.loading"
          style="margin-bottom:10px;"
        >
          <deployUp :order_data="order.data" @change="change" />
        </a-card>
        <a-card
          v-if="[5,6].indexOf(s)>=0"
          bordered
          size="small"
          title="下刊"
          :loading="order.loading"
          style="margin-bottom:10px;"
        >
          <deployDown :order_data="order.data" @change="change" />
        </a-card>
      </a-col>
      <a-col :span="8">
        <a-alert :message="`订单状态`" type="warning" style="margin-bottom:10px;">
          <p slot="description">
            <span>当前订单状态：</span>
            <span style="color: red">
              <b>{{status_str}}</b>
            </span>
          </p>
        </a-alert>
        <a-alert
          v-if="s===2&&not_empty(order.data.OrderDesigns)"
          message="已出设计方案，等待客户确认"
          style="margin-bottom:10px;"
        ></a-alert>
        <orderHistory v-if="false" :order_data="order.data" />
      </a-col>
    </a-row>
  </div>
</template>

<script>
import { post_form } from '@/api/ajax'
import { mixin } from '@/utils/mixin'
import timeview from '@/coms/timeview'
import upload from '../upload'
import imagepreview from '@/coms/imagepreview'
import orderSummary from './order_detail_summary'
import orderDesign from './order_detail_design'
import deployUp from './order_detail_deploy_up'
import deployDown from './order_detail_deploy_down'
import orderHistory from './order_detail_history'
import { get_status_str } from '../utils'

export default {
  components: {
    timeview,
    upload,
    imagepreview,
    orderSummary,
    orderDesign,
    deployUp,
    deployDown,
    orderHistory
  },
  mixins: [mixin],
  data() {
    return {
      order: {
        loading: false,
        order_uid: '',
        data: {}
      }
    }
  },
  computed: {
    s() {
      return this.order.data.Status
    },
    status_str() {
      if (this.s === 2) {
        return this.order.data.DesignCount > 0 ? '设计待确认' : '设计待上传'
      }
      return get_status_str(this.s)
    }
  },
  watch: {
    $route: function(val) {
      this.init_page()
    }
  },
  mounted() {
    this.init_page()
  },
  methods: {
    init_page() {
      this.order.order_uid = this.$route.params.uid || ''
      if (this.order.order_uid.length <= 0) {
        this.go_home()
        return
      }
      this.load_data()
    },
    go_home() {
      var status = (this.s || '').toString()
      this.$router.push({ name: 'metroad-order-order-list', query: { status: status } })
    },
    change() {
      this.$message.info('刷新订单中')
      setTimeout(this.load_data, 500)
    },
    load_data() {
      this.order.loading = true
      post_form(this, '/api/metro-ad/admin/MetroOrder/QueryOrderDetail', {
        order_uid: this.order.order_uid
      })
        .then(res => {
          if (res.Success) {
            this.order.data = res.Data
          } else {
            alert(res.ErrorMsg)
            this.go_home()
          }
        })
        .finally(() => {
          this.order.loading = false
        })
    }
  }
}
</script>