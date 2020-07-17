<template>
  <div>
    <a-spin :spinning="loading">
      <a-alert message="没有权限" v-if="has_permission('xx--------------')"></a-alert>
      <a-alert message="删除缓存须谨慎" banner style="margin-bottom:10px;" />
      <a-collapse accordion size="small">
        <a-collapse-panel v-for="(x,i) in data" :header="x.TypeName" :key="i">
          <a-card
            v-for="(m,j) in x.Methods"
            :key="j"
            :title="`${m.Name}`"
            hoverable
            size="small"
            style="margin-bottom:10px;"
          >
            <span slot="extra">
              <a-button
                type="danger"
                icon="delete"
                size="small"
                :loading="m.loading"
                @click="()=>{delete_(x.TypeName,m)}"
              >删除缓存</a-button>
            </span>
            <a-form v-if="m.Params.length>0">
              <a-form-item
                v-for="(p,e) in m.Params"
                :key="e"
                :label="p.Name"
                :label-col="{ span: 5 }"
                :wrapper-col="{ span: 12 }"
              >
                <a-input
                  v-if="p.TypeName=='String'"
                  v-model="p.Value"
                  :placeholder="p.FullTypeName"
                ></a-input>
                <a-alert v-else message="无法识别的参数类型，请不要使用此方法"></a-alert>
              </a-form-item>
            </a-form>
            <a-alert v-else message="无参数，可以直接删除缓存"></a-alert>
          </a-card>
        </a-collapse-panel>
      </a-collapse>
    </a-spin>
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
      data: []
    }
  },
  mounted() {
    this.query_all()
  },
  methods: {
    delete_(cls, m) {
      if (!confirm('确定删除缓存？')) {
        return
      }
      var obj = {}
      m.Params.forEach(p => {
        obj[p.Name] = p.Value
      })
      m.loading = true
      post_form(this, '/api/member/admin/CacheManage/RemoveCacheKey', {
        type_name: cls,
        method_name: m.Name,
        param_data: JSON.stringify(obj)
      })
        .then(res => {
          if (res.Success) {
            alert('删除成功')
          } else {
            alert(res.ErrorMsg)
          }
        })
        .finally(() => {
          m.loading = false
        })
    },
    query_all() {
      this.loading = true
      post_form(this, '/api/member/admin/CacheManage/Query', {})
        .then(res => {
          var res_data = res.Data
          res_data.forEach(x => {
            x.Methods.forEach(m => {
              m.loading = false
              m.Params.forEach(p => {
                p.Value = null
                if (p.TypeName === 'String') {
                  p.Value = ''
                } else if (['Int32', 'Int64', 'Long', 'Double'].indexOf(p.TypeName) >= 0) {
                  p.Value = 0
                } else {
                  //
                }
              })
            })
          })

          this.data = res.Data
        })
        .finally(() => {
          this.loading = false
        })
    }
  }
}
</script>