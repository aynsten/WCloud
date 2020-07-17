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
      <span slot="desc" slot-scope="text,item">{{item.raw_data.RoleDescription||'--'}}</span>
      <span slot="per_count" slot-scope="text,item">
        <a-tag size="small" color="green">{{(item.raw_data.PermissionUIDs||[]).length}}</a-tag>
      </span>
      <span slot="action" slot-scope="text,item">
        <a-button-group size="small" style="float:right;">
          <a-button
            :disabled="!node_available(item)"
            type="dashed"
            size="small"
            icon="setting"
            @click="show_permission_form(item)"
          >关联权限</a-button>
          <a-dropdown :trigger="['click']">
            <a-button type="dashed" icon="ellipsis" />
            <a-menu slot="overlay">
              <a-menu-item
                key="edit"
                :disabled="!node_editable(item)"
                @click="show_role_form(item)"
              >
                <a-icon type="edit" />
                <span>修改</span>
              </a-menu-item>
              <a-menu-item key="append" @click="show_add_child_role_form(item)">
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

    <a-modal title="角色管理" :width="800" v-model="role_form.show">
      <a-form>
        <a-form-item label="父角色" v-if="(role_form.data.ParentName||'').length>0">
          <a-tag color="green">{{role_form.data.ParentName}}</a-tag>
        </a-form-item>

        <a-form-item label="角色名称">
          <a-input
            placeholder="起一个名字"
            v-model="role_form.data.NodeName"
            :disabled="(role_form.data.UID||'').length>0"
          />
        </a-form-item>

        <a-form-item label="描述">
          <a-textarea v-model="role_form.data.RoleDescription" placeholder="..." />
        </a-form-item>
      </a-form>
      <template slot="footer">
        <a-button type="primary" size="small" :loading="role_form.loading" @click="save_role">保存</a-button>
      </template>
    </a-modal>

    <a-modal title="分配权限" width="80%" v-model="set_permission.show">
      <a-row :gutter="10">
        <a-col :span="24">
          <permissiontreeselector
            :selected_uids="this.set_permission.data.checked"
            @on_select="(e)=>{this.set_permission.data.permission_uid=e;this.set_permission.data.checked=e;}"
          />
        </a-col>
      </a-row>
      <template slot="footer">
        <a-button
          type="primary"
          size="small"
          :loading="set_permission.loading"
          :disabled="set_permission_button_disabled"
          @click="set_permission_for_role"
        >保存</a-button>
      </template>
    </a-modal>
  </a-card>
</template>

<script>
import { post_form } from '@/api/ajax'
import permissiontreeselector from './permissiontreeselector'
import { mixin } from '@/utils/mixin'

export default {
  components: {
    permissiontreeselector
  },
  mixins: [mixin],
  data() {
    return {
      loading: false,
      role_form: {
        show: false,
        loading: false,
        data: {
          ParentName: '',
          NodeName: '',
          RoleDescription: ''
        }
      },
      set_permission: {
        show: false,
        loading: false,
        data: {
          role_uid: '',
          permission_uid: [],
          checked: [],
          old_data: ''
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
            title: '权限数量',
            scopedSlots: { customRender: 'per_count' }
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
    this.getRole()
  },
  methods: {
    show_role_form(row) {
      if (!row) {
        alert('未能获取记录')
        return
      }
      this.role_form.data = {}
      this.role_form.data = this.copy_data(row.raw_data)
      this.role_form.show = true
    },
    show_add_child_role_form(row) {
      var data = this.copy_data(row.raw_data)

      this.role_form.data = {}
      this.role_form.data.ParentName = data.NodeName
      this.role_form.data.ParentUID = data.UID
      this.role_form.data.Level = data.Level + 1

      this.role_form.show = true
    },
    save_role() {
      this.role_form.loading = true
      post_form(this, '/api/member/admin/Role/Save', {
        data: JSON.stringify(this.role_form.data)
      })
        .then(res => {
          if (res.Success) {
            this.getRole()
            this.role_form.show = false
          } else {
            alert(res.ErrorMsg)
          }
        })
        .finally(() => {
          this.role_form.loading = false
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
    getRole() {
      this.loading = true
      post_form(this, '/api/member/admin/Role/QueryAntTree', {})
        .then(res => {
          if (res.Success) {
            this.table.data = [
              {
                key: 'all',
                title: '全部角色',
                children: res.Data,
                raw_data: {
                  UID: '',
                  Level: 0,
                  NodeName: '全部角色'
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
    show_permission_form(record) {
      this.set_permission.show = true
      this.set_permission.data.role_uid = record.raw_data.UID
      this.set_permission.data.permission_uid = [...record.raw_data.PermissionUIDs]
      this.set_permission.data.checked = this.set_permission.data.permission_uid
      this.set_permission.data.old_data = this.str_arr_fingle_print(this.set_permission.data.permission_uid)
    },
    set_permission_for_role() {
      this.set_permission.loading = true
      post_form(this, '/api/member/admin/Role/SetRolePermission', {
        role_uid: this.set_permission.data.role_uid,
        permission: JSON.stringify(this.set_permission.data.permission_uid)
      })
        .then(res => {
          if (!res.Success) {
            this.$message.error(res.ErrorMsg, 3)
          } else {
            this.set_permission.show = false
            this.set_permission.data = {}
            this.getRole()
          }
        })
        .finally(() => {
          this.set_permission.loading = false
        })
    },
    confirmDelete(record) {
      if (!confirm('确定删除？')) {
        return
      }
      post_form(this, '/api/member/admin/Role/Delete', { uid: record.raw_data.UID }).then(res => {
        if (res.Success) {
          this.getRole()
        } else {
          alert(res.ErrorMsg)
        }
      })
    }
  },
  computed: {
    set_permission_button_disabled() {
      var new_data = this.str_arr_fingle_print(this.set_permission.data.permission_uid)
      var res = this.set_permission.data.old_data === new_data
      return res
    }
  }
}
</script>