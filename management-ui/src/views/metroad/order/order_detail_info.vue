<template>
  <div>
    <table class="xx">
      <tbody>
        <tr class="hover">
          <td>订单号</td>
          <td colspan="3">{{order.data.OrderNo}}</td>
        </tr>
        <tr v-if="order.data.User">
          <td>客户名称</td>
          <td>
            <a-avatar size="small" :src="order.data.User.UserImg||'/admin/logo.png'"></a-avatar>
            <span>
              {{ order.data.User.NickName||order.data.User.UserName }}
              {{ order.data.User.RealName&&`(${order.data.User.RealName})` }}
            </span>
          </td>
          <td>联系电话</td>
          <td>
            <a-tag>
              {{
              order.data.User.UserPhone&&
              order.data.User.UserPhone.Phone||
              '用户未绑定手机'
              }}
            </a-tag>
          </td>
        </tr>
        <tr v-else>
          <td colspan="4">
            <a-alert message="用户信息丢失"></a-alert>
          </td>
        </tr>
        <tr>
          <td>投放日期</td>
          <td>
            <timeview :time_str="order.data.AdStartTimeUtc" time_format="YYYY-MM-DD" />
            <span>~</span>
            <timeview :time_str="order.data.AdEndTimeUtc" time_format="YYYY-MM-DD" />
          </td>
          <td>投放周期</td>
          <td>{{order.data.TotalDays}}天</td>
        </tr>
        <tr>
          <td>应付金额</td>
          <td>{{order.data.TotalPrice}}元</td>
          <td>下单时间</td>
          <td>
            <timeview :time_str="order.data.CreateTimeUtc" />
          </td>
        </tr>
        <tr>
          <td>站点数</td>
          <td>{{station_count}}</td>
          <td>点位数</td>
          <td>{{ad_window_count}}</td>
        </tr>
        <tr>
          <td>备注</td>
          <td colspan="3">
            <div v-if="order.data.ImageList" style="margin-bottom:10px;">
              <span v-for="(x,i) in order.data.ImageList" :key="i" style="margin:10px;">
                <imagepreview :src="x" alt />
              </span>
            </div>
            <a-card v-if="order.data.CustomerDemand" size="small">{{order.data.CustomerDemand}}</a-card>
          </td>
        </tr>
        <tr v-if="order.data.Approver">
          <td>审批人</td>
          <td>
            <a-avatar size="small" :src="order.data.Approver.UserImg||'/admin/logo.png'"></a-avatar>
            <span>
              {{
              order.data.Approver.NickName||
              order.data.Approver.UserName
              }}
            </span>
          </td>
          <td>审批时间</td>
          <td>
            <timeview :time_str="order.data.ApproveTimeUtc" />
          </td>
        </tr>
        <tr v-if="rejected">
          <td>驳回原因</td>
          <td colspan="3">{{order.data.ApproveComment||'--'}}</td>
        </tr>
        <tr v-if="payed">
          <td>支付金额</td>
          <td>{{order.data.TotalPrice}}</td>
          <td>支付方式</td>
          <td>{{pay_method_str}}({{order.data.ExternalPaymentNo||'--'}})</td>
        </tr>
        <tr v-if="payed">
          <td>支付日期</td>
          <td>
            <timeview :time_str="order.data.PayTime" />
          </td>
          <td>支付备注</td>
          <td>{{order.data.PaymentComment}}</td>
        </tr>
        <tr v-if="payed">
          <td>支付凭证</td>
          <td colspan="3">
            <span v-for="(x,i) in order.data.PaymentVoucherList" :key="i" style="margin:10px;">
              <imagepreview :src="x" alt />
            </span>
          </td>
        </tr>
        <tr v-if="order.data.Status==-2">
          <td>取消原因</td>
          <td>{{order.data.CloseReason||'--'}}</td>
          <td>取消时间</td>
          <td>
            <timeview :time_str="order.data.CloseTimeUtc" />
          </td>
        </tr>
      </tbody>
    </table>
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
import { get_pay_method_str } from '../utils'

export default {
  components: {
    timeview,
    upload,
    imagepreview,
    stationType,
    orderItemsCom
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
    ad_window_count() {
      return (this.order.data.OrderItems && this.order.data.OrderItems.length) || 0
    },
    station_count() {
      return (
        (this.order.data.OrderItems && [...new Set(this.order.data.OrderItems.map(x => x.MetroStationUID))].length) || 0
      )
    },
    payed() {
      return this.order.data.PayTime != null
    },
    pay_method_str() {
      return get_pay_method_str(this.order.data.PayMethod)
    },
    rejected() {
      return this.order.data.Status == -1
    }
  },
  methods: {}
}
</script>

<style scoped>
.xx {
  width: 100%;
  margin-bottom: 10px;
}
.xx td {
  padding: 8px;
}
tr.hover td {
  background-color: aliceblue;
}
.xx tr:hover td {
  background-color: aliceblue;
}
</style>