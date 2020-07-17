<template>
  <div>
    <div v-if="order.data.OrderDeploymentUp">
      <div v-for="(item,i) in order.data.OrderDeploymentUp" :key="i">
        <p>
          <a-tag v-if="item.OnSiteDeploy===1">有现场施工</a-tag>
          <a-tag v-else>无现场施工</a-tag>
        </p>
        <a-card size="small" style="margin-bottom:10px;">
          <div style="margin-bottom:10px;">
            <p>
              <usericon :data="item.Admin" />
              <a-tag>
                <timeview :time_str="item.CreateTimeUtc" />
              </a-tag>
            </p>
            <b>{{item.Comment}}</b>
          </div>
          <div v-if="item.ImageList" style="margin-bottom:10px;">
            <span v-for="(x,i) in item.ImageList" :key="i" style="margin:10px;">
              <imagepreview :src="x" alt />
            </span>
          </div>
        </a-card>
      </div>
    </div>
    <a-button
      v-if="[4].indexOf(s)>=0"
      type="dashed"
      block
      icon="plus"
      @click="show_add_deploy_form"
    >添加施工图</a-button>
    <a-modal v-model="form.deploy.show" title="添加施工图" size="small">
      <a-form>
        <a-form-item label="是否有现场施工">
          <a-switch v-model="form.deploy.data.OnSiteDeployBool"></a-switch>
        </a-form-item>
        <a-form-item label="备注">
          <a-textarea v-model="form.deploy.data.Comment"></a-textarea>
        </a-form-item>
        <a-form-item label="上传施工图">
          <upload
            :image_list="form.deploy.data.ImageList"
            :max_count="9"
            @change="data=>{form.deploy.data.ImageList=data;}"
          />
        </a-form-item>
      </a-form>
      <span slot="footer">
        <a-button
          type="primary"
          size="small"
          :loading="form.deploy.loading"
          @click="save_deploy_image"
        >确定</a-button>
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
        deploy: {
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
    show_add_deploy_form() {
      this.form.deploy.data = {
        Comment: '',
        ImageList: [],
        OnSiteDeployBool: true
      }
      this.form.deploy.show = true
    },
    save_deploy_image() {
      this.form.deploy.loading = true
      var data = this.form.deploy.data
      data.OrderUID = this.order.data.UID
      data.OnSiteDeploy = data.OnSiteDeployBool ? 1 : 0

      post_form(this, '/api/metro-ad/admin/MetroOrder/AddOrderDeploymentUp', {
        data: JSON.stringify(data)
      })
        .then(res => {
          if (res.Success) {
            this.form.deploy.show = false
            this.$emit('change')
          } else {
            alert(res.ErrorMsg)
          }
        })
        .finally(() => {
          this.form.deploy.loading = false
        })
    }
  }
}
</script>