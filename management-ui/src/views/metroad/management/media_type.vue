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
          :title="`确定要删除 ${x.Name} 吗？`"
          okText="删除"
          cancelText="取消"
          placement="right"
          @confirm="()=>{delete_(x)}"
        >
          <a-button type="dashed" size="small" style="margin:10px" :loading="x.loading">{{x.Name}}</a-button>
        </a-popconfirm>
      </div>

      <a-alert v-else message="暂无"></a-alert>
    </a-card>

    <a-modal v-model="add_form.show" title="添加">
      <a-form>
        <a-form-item label="媒体类型">
          <a-input v-model="add_form.data.Name"></a-input>
        </a-form-item>
      </a-form>
      <span slot="footer">
        <a-button type="primary" size="small" :loading="add_form.loading" @click="save">保存</a-button>
      </span>
    </a-modal>
  </div>
</template>

<script>
import { post_form } from '@/api/ajax'

export default {
  data() {
    return {
      loading: false,
      data_list: [],
      add_form: {
        show: false,
        loading: false,
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
      post_form(this, '/api/metro-ad/admin/MetroMediaType/QueryAll', {})
        .then(res => {
          var data = res.Data
          data.forEach(x => {
            x.loading = false
          })
          this.data_list = data
        })
        .finally(() => {
          this.loading = false
        })
    },
    save() {
      this.add_form.loading = true
      post_form(this, '/api/metro-ad/admin/MetroMediaType/Save', {
        data: JSON.stringify(this.add_form.data)
      })
        .then(res => {
          if (res.Success) {
            this.add_form.show = false
            this.load_all()
          } else {
            alert(res.ErrorMsg)
          }
        })
        .finally(() => {
          this.add_form.loading = false
        })
    },
    delete_(item) {
      if (!confirm('确定删除？')) {
        return
      }

      item.loading = true
      post_form(this, '/api/metro-ad/admin/MetroMediaType/Delete', {
        uid: item.UID
      })
        .then(res => {
          if (res.Success) {
            this.load_all()
          } else {
            alert(res.ErrorMsg)
          }
        })
        .finally(() => {
          item.loading = false
        })
    }
  }
}
</script>