<template>
  <a-card :bordered="false" size="small">
    <a-form layout="inline" style="margin-bottom:10px;">
      <a-form-item label="客户名">
        <a-input placeholder="请输入" v-model="query_form.keyword" />
      </a-form-item>
      <a-form-item>
        <span>
          <a-button type="primary" @click="search">查询</a-button>
          <a-button style="margin-left: 8px;" @click="()=>{add_org_form.show = true}">新增</a-button>
          <a-divider type="vertical" />
          <span style="margin-right:8px;">禁用</span>
          <a-switch
            size="small"
            v-model="query_form.isremove"
            :loading="loading"
            @change="()=>{switch_change()}"
          />
        </span>
      </a-form-item>
    </a-form>

    <a-spin :spinning="loading">
      <a-table
        size="small"
        :columns="org_table.columns"
        :dataSource="org_table.data"
        :pagination="org_table.pagination"
        @change="handleTableChange"
        :rowKey="record => record.UID"
      >
        <span slot="name" slot-scope="text,record">
          <router-link :to="{ name: 'org-detail',query:{id:record.UID} }">{{record.OrgName}}</router-link>
        </span>
        <span slot="contact" slot-scope="text,record">
          <div>{{record.Phone}}</div>
          <div>{{record.OrgWebSite}}</div>
        </span>
        <span slot="expired" slot-scope="text,record">
          <timeview :time_str="record.ExpiredTimeUtc" />
        </span>
        <span slot="owner" slot-scope="text,record" @click="handleDel(record)">
          <avatar-list :max-length="3" v-if="record.Owner.length>0">
            <avatar-list-item
              v-for="(item,i) in record.Owner"
              :key="i"
              :tips="item.UserName"
              :src="item.UserImg"
              :name="(item.UserName||'').slice(0,3)"
            />
          </avatar-list>
          <a-button type="dashed" shape="circle" icon="plus" v-else />
        </span>

        <span slot="action" slot-scope="text, record">
          <a-dropdown>
            <a class="ant-dropdown-link">
              更多
              <a-icon type="down" />
            </a>
            <a-menu slot="overlay">
              <a-menu-item>
                <a href="javascript:;">续费</a>
              </a-menu-item>
              <a-menu-item>
                <a href="javascript:;" @click="confirmDelete(record)">禁用</a>
              </a-menu-item>
            </a-menu>
          </a-dropdown>
        </span>
      </a-table>
    </a-spin>

    <a-modal title="管理员" style="top: 20px;" :width="600" v-model="admin.show" :footer="false">
      <a-table
        size="small"
        :columns="admin.table.columns"
        :dataSource="admin.table.data"
        :pagination="false"
        :rowKey="x=>x.UID"
        :showHeader="false"
        style="margin-bottom:10px;"
      >
        <span slot="name" slot-scope="text, record">
          <span style="margin-right:10px;">
            <a-avatar :src="record.UserImg" size="small">{{record.UserName}}</a-avatar>
          </span>
          <span>{{record.UserName}}</span>
          <span v-if="record.NickName">({{record.NickName}})</span>
        </span>
        <span slot="action" slot-scope="text, record">
          <div style="text-align:right;">
            <a href="javascript:;" @click="ownerDelete(record)">设为普通成员</a>
          </div>
        </span>
      </a-table>

      <a-input-search
        v-model="admin.new_admin"
        size="small"
        placeholder="输入手机号"
        @search="(phone)=>{handleAddOkA(phone)}"
      >
        <a-button slot="enterButton" icon="plus"></a-button>
      </a-input-search>
    </a-modal>

    <a-modal title="新增客户" style="top: 20px;" :width="800" v-model="add_org_form.show">
      <a-form>
        <a-form-item label="客户名">
          <a-input v-model="add_org_form.org.OrgName" />
        </a-form-item>

        <a-form-item label="描述">
          <a-textarea v-model="add_org_form.org.OrgDescription" />
        </a-form-item>

        <a-form-item label="客户网站">
          <a-input v-model="add_org_form.org.OrgWebSite" />
        </a-form-item>

        <a-form-item label="客户手机号">
          <a-input v-model="add_org_form.org.Phone" />
        </a-form-item>
      </a-form>
      <template slot="footer">
        <a-button
          type="primary"
          size="small"
          :loading="add_org_form.loading"
          @click="handleAddOkF"
        >保存</a-button>
      </template>
    </a-modal>
    <route-view></route-view>
  </a-card>
</template>

<script>
import AvatarList from '@/components/AvatarList'
import { post_form } from '@/api/ajax'
import { RouteView } from '@/layouts'
import timeview from '@/coms/timeview'

const AvatarListItem = AvatarList.AvatarItem

export default {
  components: {
    AvatarList,
    AvatarListItem,
    RouteView,
    timeview
  },
  data() {
    return {
      query_form: {
        keyword: '',
        isremove: false
      },
      org_table: {
        data: [],
        pagination: {
          current: 1,
          total: 0
        },
        columns: [
          {
            title: '',
            scopedSlots: { customRender: 'name' }
          },
          {
            title: '',
            dataIndex: 'OrgDescription'
          },
          {
            title: '',
            scopedSlots: { customRender: 'contact' }
          },
          {
            title: '成员数量',
            dataIndex: 'MemeberCount'
          },
          {
            title: '过期时间',
            scopedSlots: { customRender: 'expired' }
          },
          {
            title: '管理员',
            scopedSlots: { customRender: 'owner' }
          },
          {
            title: '操作',
            width: '150px',
            scopedSlots: { customRender: 'action' }
          }
        ]
      },
      admin: {
        show: false,
        org: {},
        new_admin: '',
        table: {
          data: [],
          columns: [
            {
              title: '',
              scopedSlots: { customRender: 'name' }
            },
            {
              title: '',
              width: '150px',
              scopedSlots: { customRender: 'action' }
            }
          ]
        }
      },
      add_org_form: {
        show: false,
        loading: false,
        org: {
          OrgName: '',
          OrgDescription: '',
          OrgWebSite: '',
          Phone: ''
        }
      },
      loading: false
    }
  },
  created() {
    this.getOrg()
  },
  methods: {
    search() {
      this.org_table.pagination.current = 1
      this.getOrg()
    },
    switch_change() {
      this.org_table.pagination.current = 1
      this.getOrg()
    },
    getOrg() {
      this.loading = true
      post_form(this, '/api/member/admin/Org/Query', {
        q: this.query_form.keyword,
        isremove: this.query_form.isremove,
        page: this.org_table.pagination.current
      })
        .then(res => {
          if (res.Success) {
            this.org_table.data = res.Data.data
            this.org_table.pagination.total = res.Data.totalCount
          } else {
            alert(res.ErrorMsg)
          }
        })
        .finally(() => {
          this.loading = false
        })
    },
    handleAddOkA(phone) {
      if (phone.length <= 0) {
        return
      }
      if (!confirm('确认添加' + phone + '？')) {
        return
      }
      post_form(this, '/api/member/admin/Org/AddAdminMember', {
        phone: phone,
        org_uid: this.admin.org.UID
      }).then(res => {
        if (!res.Success) {
          alert(res.ErrorMsg)
        } else {
          this.admin.new_admin = ''
          this.admin.show = false
          this.getOrg()
        }
      })
    },
    handleAddOkF() {
      this.add_org_form.loading = true
      post_form(this, '/api/member/admin/Org/Save', {
        data: JSON.stringify(this.add_org_form.org)
      })
        .then(res => {
          if (!res.Success) {
            alert(res.ErrorMsg)
          } else {
            this.add_org_form.show = false
            this.getOrg()
          }
        })
        .finally(() => {
          this.add_org_form.loading = false
        })
    },

    handleDel(record) {
      this.admin.table.data = record.Owner
      this.admin.org = record
      this.admin.show = true
    },

    ownerDelete(record) {
      if (!confirm('确认禁用？')) {
        return
      }
      var _this = this
      this.loading = true

      post_form(this, '/api/member/admin/Org/RemoveOwner', {
        org_uid: this.admin.org.UID,
        user_uid: record.UID
      })
        .then(res => {
          if (!res.Success) {
            alert(res.ErrorMsg)
          } else {
            this.getOrg()
          }
        })
        .finally(() => {
          this.admin.show = false
          this.loading = false
        })
    },

    confirmDelete(record) {
      if (!confirm('确认禁用？')) {
        return
      }
      this.loading = true
      post_form(this, '/api/member/admin/Org/Delete', {
        uid: record.UID
      })
        .then(res => {
          if (!res.Success) {
            this.$message.error(res.ErrorMsg, 3)
          } else {
            this.loading = false
            this.getOrg()
          }
        })
        .finally(() => {
          this.loading = false
        })
    },
    handleTableChange(pagination) {
      this.org_table.pagination = { ...pagination }
      this.getOrg()
    }
  }
}
</script>
