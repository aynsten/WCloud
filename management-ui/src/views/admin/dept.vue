<template>
  <a-card :bordered="false" size="small">
    <a-table
      :dataSource="table.data"
      :columns="table.columns"
      :expandedRowKeys="table.expandedKeys"
      :rowKey="x=>x.key"
      :pagination="false"
      :loading="loading"
      size="small"
    >
      <span slot="desc" slot-scope="text,item">{{item.raw_data.Description||'--'}}</span>
      <span slot="action" slot-scope="text,item">
        <a-button-group size="small" style="float:right;">
          <a-dropdown :trigger="['click']">
            <a-button type="dashed" icon="ellipsis" />
            <a-menu slot="overlay">
              <a-menu-item
                key="edit"
                :disabled="!node_editable(item)"
                @click="show_dept_form(item)"
              >
                <a-icon type="edit" />
                <span>修改</span>
              </a-menu-item>
              <a-menu-item key="append" @click="show_add_child_dept_form(item)">
                <a-icon type="plus" />
                <span>添加下级</span>
              </a-menu-item>
              <a-menu-divider />
              <a-menu-item
                key="delete"
                :disabled="!node_deleteable(item)"
                @click="confirmDelete(item)"
              >
                <a-icon type="delete" />
                <span>删除</span>
              </a-menu-item>
            </a-menu>
          </a-dropdown>
        </a-button-group>
      </span>
    </a-table>

    <a-modal title="部门管理" :width="800" v-model="dept_form.show">
      <a-form>
        <a-form-item label="父部门" v-if="(dept_form.data.ParentName||'').length>0">
          <a-tag color="green">{{dept_form.data.ParentName}}</a-tag>
        </a-form-item>

        <a-form-item label="部门名称">
          <a-input placeholder="起一个名字" v-model="dept_form.data.NodeName" />
        </a-form-item>

        <a-form-item label="描述">
          <a-textarea v-model="dept_form.data.Description" placeholder="..." />
        </a-form-item>
      </a-form>
      <template slot="footer">
        <a-button type="primary" size="small" :loading="dept_form.loading" @click="save_role">保存</a-button>
      </template>
    </a-modal>
  </a-card>
</template>

<script>
import { post_form } from '@/api/ajax'
import { mixin } from '@/utils/mixin'

export default {
  components: {
  },
  mixins: [mixin],
  data() {
    return {
      loading: false,
      dept_form: {
        show: false,
        loading: false,
        data: {
          ParentName: '',
          NodeName: '',
          Description: ''
        }
      },
      table: {
        columns: [
          {
            title: '名称',
            dataIndex: 'title'
          },
          {
            title: '描述',
            scopedSlots: { customRender: 'desc' }
          },
          {
            scopedSlots: { customRender: 'action' }
          }
        ],
        data: [],
        expandedKeys: []
      }
    }
  },
  created() {
    this.getDept()
  },
  methods: {
    show_dept_form(row) {
      if (!row) {
        alert('未能获取当前节点数据')
      }
      this.dept_form.data = {}
      this.dept_form.data = this.copy_data(row.raw_data)
      this.dept_form.show = true
    },
    show_add_child_dept_form(row) {
      var data = this.copy_data(row.raw_data)

      this.dept_form.data = {}
      this.dept_form.data.ParentName = data.NodeName
      this.dept_form.data.ParentUID = data.UID
      this.dept_form.data.Level = data.Level + 1

      this.dept_form.show = true
    },
    save_role() {
      this.dept_form.loading = true
      post_form(this, '/api/member/admin/Dept/Save', {
        data: JSON.stringify(this.dept_form.data)
      })
        .then(res => {
          if (res.Success) {
            this.getDept()
            this.dept_form.show = false
          } else {
            alert(res.ErrorMsg)
          }
        })
        .finally(() => {
          this.dept_form.loading = false
        })
    },
    set_slots() {
      var data_list = this.table.data

      var all_keys = this.find_in_tree(data_list, node => {
        node.scopedSlots = { title: 'title' }
      })

      this.table.data = data_list
      this.table.expandedKeys = all_keys
    },
    getDept() {
      this.loading = true
      post_form(this, '/api/member/admin/Dept/Query', {})
        .then(res => {
          if (res.Success) {
            this.table.data = [
              {
                key: 'all',
                title: '全部部门',
                children: res.Data,
                raw_data: {
                  UID: '',
                  Level: 0,
                  NodeName: '全部部门'
                }
              }
            ]
            this.set_slots()
          } else {
            alert(res.ErrorMsg)
          }
        })
        .finally(() => {
          this.loading = false
        })
    },
    confirmDelete(record) {
      if (!confirm('确定删除？')) {
        return
      }
      post_form(this, '/api/member/admin/Dept/Delete', { uid: record.raw_data.UID }).then(res => {
        if (res.Success) {
          this.getDept()
        } else {
          alert(res.ErrorMsg)
        }
      })
    }
  }
}
</script>
