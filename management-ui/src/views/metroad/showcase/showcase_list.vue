<template>
  <div>
    <a-card size="small">
      <span slot="extra">
        <a-button type="primary" size="small" icon="plus" @click="show_update_form(null)">添加案例</a-button>
      </span>
      <a-table
        :dataSource="table.data"
        :columns="table.columns"
        :pagination="false"
        :rowKey="x=>x.UID"
        :loading="table.loading"
        size="small"
      >
        <span slot="active" slot-scope="text,record">
          <a-badge :status="record.IsActive>0?'success':'error'"></a-badge>
        </span>
        <span slot="img" slot-scope="text,record">
          <span v-for="(x,i) in record.ImageList" :key="i" style="margin-right:5px;">
            <imgpreview :src="x" :size="40" />
          </span>
        </span>
        <span slot="admin" slot-scope="text,record">admin-{{record.Id}}</span>
        <span slot="action" slot-scope="text,record">
          <a-button-group size="small">
            <a-button type="primary" @click="show_update_form(record)">编辑</a-button>
            <a-button type="danger" @click="del(record)">删除</a-button>
          </a-button-group>
        </span>
      </a-table>
    </a-card>
    <a-modal v-model="add_form.show" title="案例">
      <a-form>
        <a-form-item label="名称">
          <a-input v-model="add_form.data.Name"></a-input>
        </a-form-item>
        <a-form-item label="描述">
          <a-textarea v-model="add_form.data.Desc"></a-textarea>
        </a-form-item>
        <a-form-item label="关联点位">
          <adwindowselector
            :selected="add_form.data.AdWindowUIDList"
            @change="(x)=>{add_form.data.AdWindowUIDList=x;}"
          />
        </a-form-item>
        <a-form-item label="图片">
          <upload
            :image_list="add_form.data.ImageList"
            @change="(x)=>{add_form.data.ImageList=x;}"
          />
        </a-form-item>
        <a-form-item label="案例状态">
          <a-switch v-model="add_form.data.IsActiveBool"></a-switch>
        </a-form-item>
      </a-form>
      <span slot="footer">
        <a-button type="primary" size="small" :loading="add_form.loading" @click="save">保存</a-button>
      </span>
    </a-modal>
  </div>
</template>

<script>
import { post_form } from '@/api/ajax'
import { mixin } from '@/utils/mixin'
import timeview from '@/coms/timeview'
import imgpreview from '@/coms/imagepreview'
import upload from '../upload'
import adwindowselector from '../adwindowselector'

export default {
  components: {
    timeview,
    imgpreview,
    upload,
    adwindowselector
  },
  mixins: [mixin],
  data() {
    return {
      add_form: {
        show: false,
        loading: false,
        data: {}
      },
      table: {
        loading: false,
        data: [],
        columns: [
          {
            scopedSlots: { customRender: 'active' }
          },
          {
            title: '名称',
            dataIndex: 'Name'
          },
          {
            title: '描述',
            dataIndex: 'Desc'
          },
          {
            title: '图片',
            scopedSlots: { customRender: 'img' }
          },
          {
            scopedSlots: { customRender: 'admin' }
          },
          {
            scopedSlots: { customRender: 'action' }
          }
        ]
      }
    }
  },
  methods: {
    del(m) {
      if (!confirm('确定删除？')) {
        return
      }
      post_form(this, '/api/metro-ad/admin/Case/Delete', {
        uid: m.UID
      }).then(res => {
        if (res.Success) {
          alert('删除成功')
          this.load_data()
        } else {
          alert(res.ErrorMsg)
        }
      })
    },
    show_update_form(m) {
      var data = {
        Name: '',
        Desc: '',
        ImageList: [],
        AdWindowUIDList: [],
        IsActive: 1,
        IsActiveBool: true
      }
      if (m) {
        data = this.copy_data(m)
      }
      data.IsActiveBool = this.add_form.data.IsActive > 0

      this.add_form.data = data
      this.add_form.show = true
    },
    save() {
      this.add_form.loading = true
      var data = this.add_form.data
      data.IsActive = data.IsActiveBool ? 1 : 0
      post_form(this, '/api/metro-ad/admin/Case/Save', {
        data: JSON.stringify(data)
      })
        .then(res => {
          if (res.Success) {
            this.add_form.show = false
            this.load_data()
          } else {
            alert(res.ErrorMsg)
          }
        })
        .finally(() => {
          this.add_form.loading = false
        })
    },
    load_data() {
      this.table.loading = true
      post_form(this, '/api/metro-ad/admin/Case/QueryShowCase', {})
        .then(res => {
          this.table.data = res.Data
        })
        .finally(() => {
          this.table.loading = false
        })
    }
  },
  created() {
    this.load_data()
  }
}
</script>