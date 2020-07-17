<template>
  <div class="card-list">
    <a-card
      v-for="(group_uid,i) in all_groups()"
      :key="i"
      :title="group_name(group_uid)"
      style="margin-bottom:10px;"
    >
      <a-dropdown slot="extra" v-if="group_avaliable(group_uid)">
        <a-button type="dashed" size="small" icon="ellipsis" />
        <a-menu slot="overlay">
          <a-menu-item @click="confirmDelete(group_uid)">
            <a-icon type="delete" />
            <span>删除</span>
          </a-menu-item>
        </a-menu>
      </a-dropdown>

      <a-tag
        v-for="(per,j) in item_in_group(group_uid)"
        :key="j"
        closable
        color="green"
        @close="(e)=>{warning(per.UID);e.preventDefault();}"
      >{{per.NodeName}}</a-tag>

      <a-button
        shape="circle"
        size="small"
        icon="plus"
        v-if="group_avaliable(group_uid)"
        @click="handleAdd(group_uid)"
      />
    </a-card>

    <div style="margin:10px 0;">
      <div v-show="add_group">
        <a-input-search
          v-model="addgn.GroupName"
          placeholder="添加分组名"
          @search="(phone)=>{AddGroup()}"
        >
          <a-button slot="enterButton" icon="plus"></a-button>
        </a-input-search>
      </div>

      <a-button v-show="!add_group" type="dashed" icon="plus" @click="()=>{add_group=true;}">添加分组</a-button>
    </div>
    <a-modal title="新增权限" style="top: 20px;" :width="800" v-model="visibleAdd" @ok="handleAddOk">
      <a-form>
        <a-form-item label="权限名">
          <a-input v-model="mdlAdd.NodeName" />
        </a-form-item>

        <a-form-item label="权限描述">
          <a-textarea :rows="5" v-model="mdlAdd.Description" />
        </a-form-item>

        <a-form-item label="权限类型">
          <a-select v-model="mdlAdd.PermissionType">
            <a-select-option value="1">平台</a-select-option>
            <a-select-option value="2">应用</a-select-option>
          </a-select>
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script>
import { post_form } from '@/api/ajax'

export default {
  components: {},
  data() {
    return {
      addgroupuid: '',
      mdlAdd: {},
      visibleAdd: false,
      sysKey: '',
      dataSource: [],
      groups: [],
      add_group: false,
      addgn: {
        GroupName: '',
        SystemKey: ''
      }
    }
  },
  methods: {
    handleAdd(uid) {
      this.addgroupuid = uid
      this.visibleAdd = true
    },
    handleAddOk() {
      this.mdlAdd.SystemKey = this.sysKey
      this.mdlAdd.GroupUID = this.addgroupuid

      post_form(this, '/api/member/admin/Permission/Save', {
        data: JSON.stringify(this.mdlAdd)
      }).then(res => {
        if (!res.Success) {
          this.$message.error(res.ErrorMsg, 3)
        } else {
          this.visibleAdd = false
          this.mdlAdd = {}
          this.Querydata()
        }
      })
    },
    warning(UID) {
      if (confirm('确定删除？') == true) {
        this.on_delete(UID)
        return true
      } else {
        return false
      }
    },
    confirmDelete(group_uid) {
      if (!confirm('确定删除？')) {
        return
      }
      var _this = this
      post_form(this, '/api/member/admin/Permission/DeleteGroup', {
        uid: group_uid
      }).then(res => {
        _this.Querydata()
      })
    },
    Querydata() {
      var _this = this
      post_form(this, '/api/member/admin/Permission/Query', {}).then(res => {
        console.log('res', res)
        _this.groups = res.Data.Groups.filter(x => x.SystemKey == _this.sysKey)
        _this.dataSource = res.Data.Permissions.filter(x => x.SystemKey == _this.sysKey)
        console.log('dataSource', _this.dataSource)
      })
    },
    on_delete(uid) {
      var _this = this
      post_form(this, '/api/member/admin/Permission/Delete', {
        uid: uid
      }).then(res => {
        _this.Querydata()
      })
      return false
    },
    all_groups() {
      var group_uids = this.groups.map(x => x.UID)
      var permission_group_uid = this.dataSource.map(x => x.GroupUID)

      //这个集合里可能出现null，null的数据会被加入默认分组
      var set = [...new Set([...group_uids, ...permission_group_uid])]

      return set
    },
    group_name(uid) {
      var name = (this.groups.find(x => x.UID == uid) || {}).GroupName
      return name || '默认分组'
    },
    group_avaliable(uid) {
      var group = this.groups.find(x => x.UID == uid)
      return group ? true : false
    },
    item_in_group(uid) {
      return this.dataSource.filter(m => m.GroupUID == uid)
    },
    AddGroup() {
      var _this = this
      this.addgn.SystemKey = this.sysKey
      var json = JSON.stringify(this.addgn)
      post_form(this, '/api/member/admin/Permission/AddGroup', {
        data: json
      }).then(res => {
        _this.add_group = false
        _this.addgn.GroupName = ''
        _this.Querydata()
      })
    }
  },
  created() {
    this.sysKey = this.$route.params.app
    if ((this.sysKey || '').length <= 0) {
      this.$router.push({ name: 'app-list' })
      return
    }
    console.log('system:', this.sysKey)
    this.Querydata()
  }
}
</script>