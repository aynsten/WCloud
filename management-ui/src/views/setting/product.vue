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
        :pagination="table.pagination"
        :loading="table.loading"
        @change="change_page"
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
        <a-form-item label="价格">
          <a-input-number :min="0" v-model="x_form.data.Price" />
        </a-form-item>
        <a-form-item label="计量单位">
          <a-select v-model="x_form.data.UnitUID" defaultValue="lucy" style="width: 120px">
            <a-select-option value="jack">Jack</a-select-option>
            <a-select-option value="lucy">Lucy</a-select-option>
            <a-select-option value="disabled" disabled>Disabled</a-select-option>
            <a-select-option value="Yiminghe">yiminghe</a-select-option>
          </a-select>
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
        condition: {
          q: ''
        },
        pagination: {
          current: 1,
          pageSize: 1,
          total: 0
        },
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
            width: '200px',
            dataIndex: 'Price'
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
    change_page(pagination) {
      this.table.pagination = { ...pagination }
      this.query_all()
    },
    query_all() {
      this.table.loading = true
      post_form(this, '/api/erp/admin/Product/Query', {
        ...this.table.condition,
        page: this.table.pagination.current
      })
        .then(res => {
          if (res.Success) {
            this.table.data = res.Data.DataList
            this.table.pagination.pageSize = res.Data.PageSize
            this.table.pagination.total = res.Data.ItemCount
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
      post_form(this, '/api/erp/admin/Product/Delete', {
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
      post_form(this, '/api/erp/admin/Product/Save', {
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
