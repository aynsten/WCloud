<template>
  <a-card :bordered="false" size="small">
    <span slot="extra">
      <span style="margin-right:10px;">
        <span style="margin-right:8px;">禁用</span>
        <a-tooltip placement="top">
          <template slot="title">
            <span>切换账号状态</span>
          </template>
          <a-switch
            size="small"
            v-model="query_form.isremove"
            :loading="loading"
            @change="()=>{switch_change()}"
          />
        </a-tooltip>
      </span>
      <a-divider type="vertical" />
      <a-button type="primary" size="small" icon="plus" @click="()=>{add_user_form.show=true}">添加</a-button>
    </span>
    <a-spin :spinning="loading">
      <a-table
        size="small"
        :columns="table.columns"
        :dataSource="table.data"
        :pagination="table.pagination"
        @change="handleTableChange"
        :rowKey="record => record.UID"
      >
        <span slot="contact" slot-scope="text, record">
          <div v-if="record.ContactPhone">{{record.ContactPhone}}</div>
          <div v-if="record.ContactEmail">{{record.ContactEmail}}</div>
        </span>
        <span slot="Roles" slot-scope="text, record">
          <a-tag v-for="(role,i) in record.Roles" color="green" :key="i">{{role.NodeName}}</a-tag>
        </span>
        <span slot="action" slot-scope="text, record">
          <span v-if="record.IsRemove>0">
            <a @click="()=>{activeadmin(record)}">解禁</a>
          </span>
          <span v-else>
            <a @click="show_add_role_form(record)">分配角色</a>
            <a-divider type="vertical" />
            <a-dropdown>
              <a class="ant-dropdown-link">
                更多
                <a-icon type="down" />
              </a>
              <a-menu slot="overlay">
                <a-menu-item>
                  <a @click="show_reset_pwd_form(record)">重置密码</a>
                </a-menu-item>
                <a-menu-item>
                  <a @click="deleteadmin(record)">禁用</a>
                </a-menu-item>
              </a-menu>
            </a-dropdown>
          </span>
        </span>
      </a-table>
    </a-spin>

    <a-modal title="分配角色" style="top: 20px;" :width="800" v-model="setroles.show">
      <div style="margin-bottom:10px;">
        <a-tag>{{this.setroles.user_name}}</a-tag>
      </div>
      <stree
        :data="setroles.treeData"
        :checked="setroles.checkedKeys"
        @check="x=>setroles.checkedKeys=x"
      />
      <template slot="footer">
        <a-button
          type="primary"
          size="small"
          :loading="setroles.loading"
          :disabled="set_role_button_disabled"
          @click="handleAddROk"
        >保存</a-button>
      </template>
    </a-modal>

    <a-modal title="添加用户" :width="800" v-model="add_user_form.show">
      <a-form>
        <a-form-item label="用户名">
          <a-input placeholder="请输入用户名，字母或者数字" v-model="add_user_form.data.UserName" />
        </a-form-item>
        <a-form-item label="昵称">
          <a-input placeholder="请输入昵称" v-model="add_user_form.data.NickName" />
        </a-form-item>
        <a-form-item label="手机号">
          <a-input placeholder="请输入手机号" v-model="add_user_form.data.ContactPhone" />
        </a-form-item>
        <a-form-item label="邮箱">
          <a-input placeholder="请输入邮箱" v-model="add_user_form.data.ContactEmail" />
        </a-form-item>
      </a-form>
      <template slot="footer">
        <a-button
          type="primary"
          size="small"
          :loading="add_user_form.loading"
          @click="handleAddOk"
        >保存</a-button>
      </template>
    </a-modal>

    <a-modal title="重设密码" style="top: 20px;" :width="800" v-model="reset_pwd.show">
      <div style="margin-bottom:10px;">
        <a-tag>{{this.reset_pwd.user_name}}</a-tag>
      </div>

      <a-input placeholder="输入密码" v-model="reset_pwd.pwd" />
      <template slot="footer">
        <a-button type="primary" size="small" :loading="reset_pwd.loading" @click="on_reset_pwd">保存</a-button>
      </template>
    </a-modal>
  </a-card>
</template>

<script>
import { post_form } from '@/api/ajax'
import stree from '@/coms/searchtree_'
import { mixin } from '@/utils/mixin'

export default {
  components: {
    stree
  },
  mixins: [mixin],
  data() {
    return {
      loading: false,
      query_form: {
        isremove: false
      },
      reset_pwd: {
        show: false,
        loading: false,
        user_uid: '',
        user_name: '',
        pwd: ''
      },
      add_user_form: {
        show: false,
        loading: false,
        data: {
          UserName: '',
          NickName: '',
          ContactPhone: '',
          ContactEmail: ''
        }
      },
      setroles: {
        show: false,
        loading: false,
        treeData: [],
        checkedKeys: [],
        user_uid: '',
        user_name: '',
        expand_keys: [],
        old_data: ''
      },
      table: {
        data: [],
        pagination: {
          total: 0,
          current: 1
        },
        columns: [
          {
            title: '用户名',
            width: '150px',
            dataIndex: 'UserName'
          },
          {
            title: '昵称',
            width: '150px',
            dataIndex: 'NickName'
          },
          {
            title: '联系方式',
            width: '250px',
            scopedSlots: { customRender: 'contact' }
          },
          {
            title: '',
            scopedSlots: { customRender: 'Roles' }
          },
          {
            title: '操作',
            width: '150px',
            scopedSlots: { customRender: 'action' }
          }
        ]
      }
    }
  },
  created() {
    this.getUser()
    this.query_roles()
  },
  methods: {
    show_reset_pwd_form(user) {
      this.reset_pwd.show = true
      this.reset_pwd.user_uid = user.UID
      this.reset_pwd.user_name = user.NickName || user.UserName || 'unknow'
      this.reset_pwd.pwd = '123'
    },
    show_add_role_form(user) {
      this.setroles.user_uid = user.UID
      this.setroles.user_name = user.NickName || user.UserName || 'unknow'
      this.setroles.checkedKeys = user.Roles.map(x => x.UID)
      this.setroles.old_data = this.str_arr_fingle_print(this.setroles.checkedKeys)
      this.setroles.show = true
    },
    on_reset_pwd() {
      if (!confirm('确定重置该用户密码？')) {
        return
      }
      var _this = this
      this.reset_pwd.loading = true
      post_form(this, '/api/member/admin/Account/ResetPwd', this.reset_pwd)
        .then(res => {
          if (!res.Success) {
            this.$message.error(res.ErrorMsg, 3)
          }
          _this.reset_pwd.show = false
        })
        .finally(() => {
          this.reset_pwd.loading = false
        })
    },
    switch_change() {
      this.table.pagination.current = 1
      this.getUser()
    },
    query_roles() {
      var _this = this
      post_form(this, '/api/member/admin/Role/QueryAntTree', {}).then(res => {
        _this.setroles.treeData = res.Data
      })
    },
    handleAddOk() {
      this.add_user_form.loading = true
      post_form(this, '/api/member/admin/User/AddAdmin', {
        data: JSON.stringify(this.add_user_form.data)
      })
        .then(res => {
          if (!res.Success) {
            this.$message.error(res.ErrorMsg, 3)
          } else {
            this.add_user_form.show = false
            this.add_user_form.data = {}
            this.getUser()
          }
        })
        .finally(() => {
          this.add_user_form.loading = false
        })
    },
    deleteadmin(record) {
      if (!confirm('确定禁用？')) {
        return
      }
      post_form(this, '/api/member/admin/User/DeleteAdmin', {
        uid: record.UID
      }).then(res => {
        if (!res.Success) {
          this.$message.error(res.ErrorMsg, 3)
        } else {
          this.getUser()
        }
      })
    },
    activeadmin(record) {
      if (!confirm('确定激活用户？')) {
        return
      }
      post_form(this, '/api/member/admin/User/ActiveAdmin', {
        uid: record.UID
      }).then(res => {
        if (!res.Success) {
          this.$message.error(res.ErrorMsg, 3)
        } else {
          this.getUser()
        }
      })
    },
    handleAddROk() {
      this.setroles.loading = true
      post_form(this, '/api/member/admin/Role/SetUserRole', {
        user_uid: this.setroles.user_uid,
        role: JSON.stringify(this.setroles.checkedKeys)
      })
        .then(res => {
          if (!res.Success) {
            this.$message.error(res.ErrorMsg, 3)
          } else {
            this.setroles.show = false
            this.getUser()
          }
        })
        .finally(() => {
          this.setroles.loading = false
        })
    },
    handleTableChange(pagination) {
      this.table.pagination = { ...pagination }
      this.getUser()
    },
    getUser() {
      this.loading = true
      post_form(this, '/api/member/admin/User/Query', {
        page: this.table.pagination.current,
        isremove: this.query_form.isremove
      })
        .then(res => {
          this.table.data = res.Data.Data
          this.table.pagination.total = res.Data.ItemCount
        })
        .finally(() => {
          this.loading = false
        })
    }
  },
  computed: {
    set_role_button_disabled() {
      var new_data = this.str_arr_fingle_print(this.setroles.checkedKeys)
      var res = this.setroles.old_data === new_data
      return res
    }
  }
}
</script>
