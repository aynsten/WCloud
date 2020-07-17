<template>
  <div>
    <a-card size="small" :bordered="false" :loading="loading" v-if="(line_uid||'').length>0">
      <span slot="extra">
        <a-button type="primary" size="small" icon="plus" @click="show_form(null)">添加站点</a-button>
      </span>
      <a-table
        :dataSource="data_list"
        :columns="columns"
        :rowKey="x=>x.UID"
        :pagination="false"
        size="small"
      >
        <span slot="type" slot-scope="text,record">
          <stationType :tp="record.StationType" />
        </span>
        <span slot="action" slot-scope="text,record">
          <a-button-group size="small">
            <a-button type="dashed" @click="()=>{show_ad_window_manage(record)}">管理橱窗</a-button>
            <a-dropdown :trigger="['click']">
              <a-button type="dashed" icon="ellipsis" />
              <a-menu slot="overlay">
                <a-menu-item key="edit" @click="show_form(record)">
                  <a-icon type="edit" />
                  <span>修改</span>
                </a-menu-item>
                <a-menu-item key="delete">
                  <a-icon type="delete" @click="delete_(record)" />
                  <span>删除</span>
                </a-menu-item>
              </a-menu>
            </a-dropdown>
          </a-button-group>
        </span>
      </a-table>
    </a-card>
    <a-modal v-model="add_form.show" title="添加">
      <a-form>
        <a-form-item label="站点">
          <a-input v-model="add_form.data.Name"></a-input>
        </a-form-item>
        <a-form-item label="描述">
          <a-input v-model="add_form.data.Desc"></a-input>
        </a-form-item>
        <a-form-item>
          <a-select v-model="add_form.data.StationType" style="width: 120px">
            <a-select-option v-for="(x,i) in station_types" :key="i" :value="x.key">{{x.name}}</a-select-option>
          </a-select>
        </a-form-item>
      </a-form>
      <span slot="footer">
        <a-button type="primary" size="small" :loading="add_form.loading" @click="save">保存</a-button>
      </span>
    </a-modal>

    <a-modal v-model="ad_window_manage.show" title="橱窗管理" :width="900" :footer="false">
      <ad-window
        ref="ad_window_com"
        :station_uid_x="ad_window_manage.data.UID"
        @closeModal="()=>{ad_window_manage.show=false}"
      />
    </a-modal>
  </div>
</template>

<script>
import { post_form } from '@/api/ajax'
import { mixin } from '@/utils/mixin'
import ad_window from './ad_window'
import stationType from '../station_type'
import { station_types } from '../utils'

export default {
  components: {
    'ad-window': ad_window,
    stationType
  },
  mixins: [mixin],
  data() {
    return {
      line_uid: '',
      loading: false,
      data_list: [],
      station_types: station_types,
      columns: [
        {
          dataIndex: 'Name',
          title: '名称',
          width: '150px'
        },
        {
          title: '描述',
          dataIndex: 'Desc'
        },
        {
          scopedSlots: { customRender: 'type' },
          width: '200px'
        },
        {
          title: '操作',
          scopedSlots: { customRender: 'action' },
          width: '200px'
        }
      ],
      add_form: {
        show: false,
        loading: false,
        data: {}
      },
      ad_window_manage: {
        show: false,
        data: {}
      }
    }
  },
  mounted() {
    this.load_all()
  },
  methods: {
    show_ad_window_manage(item) {
      this.ad_window_manage.data = item
      this.ad_window_manage.show = true
      this.$nextTick(() => {
        this.$refs.ad_window_com.load_data(item.UID)
      })
    },
    show_form(item) {
      this.add_form.data = {
        StationType: 0
      }
      if (item) {
        this.add_form.data = this.copy_data(item)
      }
      this.add_form.show = true
    },
    load_all() {
      if ((this.line_uid || '').length <= 0) {
        return
      }
      this.loading = true
      post_form(this, '/api/metro-ad/admin/MetroStation/QueryByLine', {
        line_uid: this.line_uid
      })
        .then(res => {
          this.data_list = res.Data
        })
        .finally(() => {
          this.loading = false
        })
    },
    load_data(metro_line_uid) {
      this.line_uid = metro_line_uid
      console.log(metro_line_uid)
      this.load_all()
    },
    save() {
      this.add_form.loading = true
      post_form(this, '/api/metro-ad/admin/MetroStation/Save', {
        data: JSON.stringify({
          ...this.add_form.data,
          MetroLineUID: this.line_uid
        })
      })
        .then(res => {
          if (res.Success) {
            this.add_form.show = false
            this.load_all()
          } else {
            alert(res.ErrorMsg)
          }
        })
        .finally(() => {
          this.add_form.loading = false
        })
    },
    delete_(item) {
      if (!confirm('确定删除？')) {
        return
      }

      item.loading = true
      post_form(this, '/api/metro-ad/admin/MetroStation/Delete', {
        uid: item.UID
      })
        .then(res => {
          if (res.Success) {
            this.load_all()
          } else {
            alert(res.ErrorMsg)
          }
        })
        .finally(() => {
          item.loading = false
        })
    }
  }
}
</script>