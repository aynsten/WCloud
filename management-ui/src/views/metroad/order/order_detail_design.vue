<template>
  <div>
    <div v-if="order.data.OrderDesigns">
      <div v-for="(item,i) in order.data.OrderDesigns" :key="i">
        <a-card size="small" style="margin-bottom:10px;">
          <span slot="extra">
            <a-badge v-if="order.data.ConfirmedDesignUID===item.UID" status="success" text="设计确认" />
          </span>
          <div style="margin-bottom:10px;">
            <p>
              <usericon :data="item.Admin" />
              <a-tag>
                <timeview :time_str="item.CreateTimeUtc" />
              </a-tag>
            </p>
            <b>{{item.Comment}}</b>
          </div>
          <div v-if="item.DesignImages" style="margin-bottom:10px;">
            <span v-for="(x,i) in item.DesignImages" :key="i" style="margin:10px;">
              <imagepreview :src="x" alt />
            </span>
          </div>
        </a-card>
      </div>
    </div>
    <a-button
      v-if="[2].indexOf(s)>=0"
      type="dashed"
      block
      icon="plus"
      @click="show_add_design_form"
    >添加设计稿</a-button>
    <a-modal v-model="form.design.show" title="添加设计稿" size="small">
      <a-form>
        <a-form-item label="备注">
          <a-textarea v-model="form.design.data.Comment"></a-textarea>
        </a-form-item>
        <a-form-item label="上传设计稿">
          <upload
            :image_list="form.design.data.DesignImages"
            :max_count="9"
            @change="data=>{form.design.data.DesignImages=data;}"
          />
        </a-form-item>
      </a-form>
      <span slot="footer">
        <a-button type="primary" size="small" :loading="form.design.loading" @click="save_design">确定</a-button>
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
import usericon from '@/coms/usericon'

export default {
  components: {
    timeview,
    upload,
    imagepreview,
    usericon
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
      },
      form: {
        design: {
          show: false,
          loading: false,
          data: {}
        }
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
    show_add_design_form() {
      this.form.design.data = {
        Comment: '',
        DesignImages: []
      }
      this.form.design.show = true
    },
    save_design() {
      this.form.design.loading = true
      var data = this.form.design.data
      data.OrderUID = this.order.data.UID

      post_form(this, '/api/metro-ad/admin/MetroOrder/AddOrderDesign', {
        data: JSON.stringify(data)
      })
        .then(res => {
          if (res.Success) {
            this.form.design.show = false
            this.$emit('change')
          } else {
            alert(res.ErrorMsg)
          }
        })
        .finally(() => {
          this.form.design.loading = false
        })
    }
  }
}
</script>