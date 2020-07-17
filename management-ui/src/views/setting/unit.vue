<template>
  <div>
    <a-card size="small" :bordered="false">
      <span slot="extra">
        <a-button type="primary" size="small" icon="plus" @click="show_form({})">添加</a-button>
      </span>
      <a-table
        :dataSource="table.data"
        :columns="table.columns"
        :rowKey="x=>x.UID"
        :pagination="false"
        :loading="table.loading"
        size="small"
      >
        <span slot="desc" slot-scope="text, record">
          <span>{{record.Desc||'--'}}</span>
        </span>
        <span slot="action" slot-scope="text, record">
          <a-button-group size="small">
            <a-button type="dashed" icon="edit" size="small" @click="show_form(record)"></a-button>
            <a-dropdown>
              <a-button type="dashed" icon="ellipsis" size="small"></a-button>
              <a-menu slot="overlay">
                <a-menu-item>
                  <a @click="delete_(record)">删除</a>
                </a-menu-item>
              </a-menu>
            </a-dropdown>
          </a-button-group>
        </span>
      </a-table>
    </a-card>
    <a-modal title="标签" size="small" :width="800" v-model="x_form.show">
      <a-form>
        <a-form-item label="名称">
          <a-input v-model="x_form.data.Name" :disabled="not_empty(x_form.data.UID)">
            <a-icon slot="suffix" type="tag" />
          </a-input>
        </a-form-item>
        <a-form-item label="描述">
          <a-textarea v-model="x_form.data.Desc" />
        </a-form-item>
      </a-form>
      <template slot="footer">
        <a-button type="primary" size="small" :loading="x_form.loading" @click="save_">保存</a-button>
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
      table: {
        loading: false,
        columns: [
          {
            width: '200px',
            dataIndex: 'Name'
          },
          {
            title: '描述',
            scopedSlots: { customRender: 'desc' }
          },
          {
            width: '100px',
            scopedSlots: { customRender: 'action' }
          }
        ],
        data: []
      },
      x_form: {
        show: false,
        loading: false,
        data: {}
      }
    }
  },
  mounted() {
    this.query_all()
  },
  computed: {},
  methods: {
    query_all() {
      this.table.loading = true
      post_form(this, '/api/erp/admin/Unit/QueryAll', {})
        .then(res => {
          if (res.Success) {
            this.table.data = res.Data
          } else {
            alert(res.ErrorMsg)
          }
        })
        .finally(() => {
          this.table.loading = false
        })
    },
    show_form(data) {
      this.x_form.data = this.copy_data(data || {})
      this.x_form.show = true
    },
    delete_(data) {
      if (!confirm('确定删除？')) {
        return
      }
      post_form(this, '/api/erp/admin/Unit/Delete', {
        uid: data.UID
      })
        .then(res => {
          if (res.Success) {
            this.query_all()
          } else {
            alert(res.ErrorMsg)
          }
        })
        .finally(() => {})
    },
    save_() {
      this.x_form.loading = true
      post_form(this, '/api/erp/admin/Unit/Save', {
        data: JSON.stringify(this.x_form.data)
      })
        .then(res => {
          if (res.Success) {
            this.x_form.show = false
            this.query_all()
          } else {
            alert(res.ErrorMsg)
          }
        })
        .finally(() => {
          this.x_form.loading = false
        })
    }
  }
}
</script>
