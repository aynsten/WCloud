<template>
  <div>
    <a-card size="small" :loading="loading">
      <span slot="extra">
        <a-button type="primary" icon="plus" size="small" @click="show_form">添加</a-button>
      </span>

      <div v-if="data_list.length>0">
        <a-popconfirm
          v-for="(x,i) in data_list"
          :key="i"
          :title="`确定要删除吗？`"
          okText="删除"
          cancelText="取消"
          placement="right"
          @confirm="()=>{delete_(x)}"
        >
          <a-button type="dashed" size="small" style="margin:10px">{{`${x.Peroid}天`}}</a-button>
        </a-popconfirm>
      </div>

      <a-alert v-else message="暂无"></a-alert>
    </a-card>

    <a-modal v-model="add_form.show" title="添加">
      <a-form>
        <a-form-item label="订单周期">
          <a-input-number v-model="add_form.data.Peroid"></a-input-number>
        </a-form-item>
      </a-form>
      <span slot="footer">
        <a-button type="primary" size="small" @click="save">保存</a-button>
      </span>
    </a-modal>
  </div>
</template>

<script>
import { post_form } from '@/api/ajax'
import { mixin } from '@/utils/mixin'

export default {
  mixins: [mixin],
  data() {
    return {
      loading: false,
      data_list: [],
      add_form: {
        show: false,
        data: {}
      }
    }
  },
  mounted() {
    this.load_all()
  },
  methods: {
    show_form() {
      this.add_form.data = {}
      this.add_form.show = true
    },
    load_all() {
      this.loading = true
      post_form(this, '/api/metro-ad/admin/MetroSetting/GetOrderPeroid', {})
        .then(res => {
          var data = res.Data
          data.forEach((x, i) => {
            x.id = i
          })
          this.data_list = data
        })
        .finally(() => {
          this.loading = false
        })
    },
    save_data(data) {
      this.loading = true
      post_form(this, '/api/metro-ad/admin/MetroSetting/SetOrderPeroid', {
        data: JSON.stringify(data)
      })
        .then(res => {
          if (res.Success) {
            this.load_all()
          } else {
            alert(res.ErrorMsg)
          }
        })
        .finally(() => {
          this.loading = false
        })
    },
    save() {
      this.add_form.show = false
      var data = this.copy_data(this.data_list).concat(this.add_form.data)
      this.save_data(data)
    },
    delete_(item) {
      if (!confirm('确定删除？')) {
        return
      }
      var data = this.copy_data(this.data_list).filter(x => x.id != item.id)
      this.save_data(data)
    }
  }
}
</script>