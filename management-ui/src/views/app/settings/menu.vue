<template>
  <a-card :bordered="false">
    <a-alert message="在此处添加的权限将不纳入任何分组和系统" banner style="margin-bottom:20px;" />
    <a-spin :spinning="loading">
      <a-tree
        :treeData="table.data"
        :expandedKeys="table.expandedKeys"
        @expand="(x)=>{table.expandedKeys=x;}"
        showLine
      >
        <template slot="title" slot-scope="item">
          <a-row style="min-width:600px">
            <a-col :span="12">{{item.raw_data.Description||item.title}}</a-col>
            <a-col :span="12">
              <a-button-group size="small" style="float:right;">
                <a-dropdown :trigger="['click']">
                  <a-button type="dashed" icon="ellipsis" />
                  <a-menu slot="overlay">
                    <a-menu-item
                      key="edit"
                      :disabled="!node_editable(item)"
                      @click="show_permission_form(item)"
                    >
                      <a-icon type="edit" />
                      <span>修改</span>
                    </a-menu-item>
                    <a-menu-item key="append" @click="show_add_child_permission_form(item)">
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
            </a-col>
          </a-row>
        </template>
      </a-tree>
    </a-spin>

    <a-modal title="权限管理" :width="800" v-model="permission_form.show">
      <a-form>
        <a-form-item label="父权限" v-if="(permission_form.data.ParentName||'').length>0">
          <a-tag color="green">{{permission_form.data.ParentName}}</a-tag>
        </a-form-item>

        <a-form-item label="名称">
          <a-input v-model="permission_form.data.Description" />
        </a-form-item>

        <a-form-item label="权限值">
          <a-input
            v-model="permission_form.data.NodeName"
            :disabled="(permission_form.data.UID||'').length>0"
          >
            <a-icon slot="addonAfter" type="flag" />
          </a-input>
        </a-form-item>
      </a-form>
      <template slot="footer">
        <a-button
          type="primary"
          size="small"
          :loading="permission_form.loading"
          @click="save_role"
        >保存</a-button>
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
      permission_form: {
        show: false,
        loading: false,
        data: {
          ParentName: '',
          NodeName: '',
          Description: ''
        }
      },
      table: {
        data: [],
        expandedKeys: []
      }
    }
  },
  created() {
    this.getDept()
  },
  methods: {
    show_permission_form(row) {
      if (!row) {
        alert('未能获取当前节点数据')
      }
      this.permission_form.data = {}
      this.permission_form.data = this.copy_data(row.raw_data)
      this.permission_form.show = true
    },
    show_add_child_permission_form(row) {
      var data = this.copy_data(row.raw_data)

      this.permission_form.data = {}
      this.permission_form.data.ParentName = data.NodeName
      this.permission_form.data.ParentUID = data.UID
      this.permission_form.data.Level = data.Level + 1

      this.permission_form.show = true
    },
    save_role() {
      this.permission_form.loading = true
      post_form(this, '/api/member/admin/Permission/Save', {
        data: JSON.stringify(this.permission_form.data)
      })
        .then(res => {
          if (res.Success) {
            this.getDept()
            this.permission_form.show = false
          } else {
            alert(res.ErrorMsg)
          }
        })
        .finally(() => {
          this.permission_form.loading = false
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
      post_form(this, '/api/member/admin/Permission/QueryAntTree', {})
        .then(res => {
          if (res.Success) {
            this.table.data = [
              {
                key: 'all',
                title: '全部权限',
                children: res.Data,
                raw_data: {
                  UID: '',
                  Level: 0,
                  NodeName: '全部权限'
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
      post_form(this, '/api/member/admin/Permission/Delete', { uid: record.raw_data.UID }).then(res => {
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
