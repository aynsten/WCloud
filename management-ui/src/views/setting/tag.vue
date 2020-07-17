<template>
  <div>
    <a-card
      v-for="(g,index) in mixed_group"
      :key="index"
      :title="g.Key"
      :bordered="true"
      :loading="loading"
      size="small"
      style="margin-bottom:10px;"
    >
      <a-button v-if="g.extra" type="link" slot="extra">只读</a-button>
      <a-popover v-for="(m,i) in data.filter(x=>x.Group===g.Value)" :key="i" :title="m.TagName">
        <template slot="content">
          <p>{{m.Desc}}</p>
          <a-button-group size="small">
            <a-button type="primary" @click="show_form(m)">修改</a-button>
            <a-button>数据迁移</a-button>
            <a-button type="danger" @click="delete_tag(m)">删除</a-button>
          </a-button-group>
        </template>
        <a-tag color="green" @click="show_form(m)">{{m.TagName}}</a-tag>
      </a-popover>
      <a-tag
        v-if="!g.extra"
        @click="show_form({'Group':g.Value})"
        style="background: #fff; borderStyle: dashed;"
      >
        <a-icon type="plus" />新建标签
      </a-tag>
    </a-card>
    <a-modal title="标签" size="small" :width="800" v-model="tag_form.show">
      <a-form>
        <a-form-item label="名称">
          <a-input v-model="tag_form.data.TagName" :disabled="not_empty(tag_form.data.UID)">
            <a-icon slot="suffix" type="tag" />
          </a-input>
        </a-form-item>
        <a-form-item label="描述">
          <a-textarea v-model="tag_form.data.Desc" />
        </a-form-item>
        <a-form-item label="图标">
          <a-input v-model="tag_form.data.Icon">
            <a-icon slot="suffix" type="bell" />
          </a-input>
        </a-form-item>
        <a-form-item label="Image">
          <a-input v-model="tag_form.data.Image" />
        </a-form-item>
        <a-form-item label="分组">
          <a-select
            v-model="tag_form.data.Group"
            :disabled="not_empty(tag_form.data.UID)||true"
            defaultValue="default"
            style="width: 200px"
          >
            <a-select-option value="default">默认分组</a-select-option>
            <a-select-option v-for="(m,i) in groups" :key="i" :value="m.Value">{{m.Key}}</a-select-option>
          </a-select>
        </a-form-item>
      </a-form>
      <template slot="footer">
        <a-button type="primary" size="small" :loading="tag_form.loading" @click="save_tag">保存</a-button>
      </template>
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
      groups: [],
      data: [],
      tag_form: {
        show: false,
        data: {},
        loading: false
      }
    }
  },
  mounted() {
    this.queryGroup()
    this.queryAll()
  },
  computed: {
    mixed_group() {
      var buildin_groups = this.groups.map(x => x.Value)
      var tag_groups = this.data.map(x => x.Group)
      var not_in_buildin = [...new Set(tag_groups.filter(x => buildin_groups.indexOf(x) < 0))]
      var extra_groups = not_in_buildin.map(x => ({ Key: x, Value: x, extra: true }))
      return [...this.groups, ...extra_groups]
    }
  },
  methods: {
    delete_tag(data) {
      if (!confirm('确定删除?')) {
        return
      }
      post_form(this, '/api/erp/admin/common/Tag/Delete', {
        uid: data.UID
      }).then(res => {
        if (res.Success) {
          alert('删除成功')
          this.queryAll()
        } else {
          alert(res.ErrorMsg)
        }
      })
    },
    show_form(data) {
      this.tag_form.data = {
        Group: 'default'
      }
      if (data) {
        this.tag_form.data = this.copy_data(data)
      }
      this.tag_form.show = true
    },
    save_tag() {
      this.tag_form.loading = true
      post_form(this, '/api/erp/admin/common/Tag/Save', {
        data: JSON.stringify(this.tag_form.data)
      })
        .then(res => {
          if (res.Success) {
            this.tag_form.show = false
            this.queryAll()
          } else {
            alert(res.ErrorMsg)
          }
        })
        .finally(() => {
          this.tag_form.loading = false
        })
    },
    queryGroup() {
      post_form(this, '/api/erp/admin/common/Tag/QueryGroups', {}).then(res => {
        this.groups = res.Data.map(x => ({
          Key: x.Key,
          Value: (x.Value || '').toString()
        }))
      })
    },
    queryAll() {
      this.loading = true
      post_form(this, '/api/erp/admin/common/Tag/QueryAll', {})
        .then(res => {
          this.data = res.Data
        })
        .finally(() => {
          this.loading = false
        })
    }
  }
}
</script>
