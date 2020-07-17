<template>
  <div>
    <a-card size="small" :loading="loading">
      <span slot="extra">
        <a-button type="primary" icon="plus" size="small" @click="show_form(null)">添加</a-button>
      </span>

      <a-table
        :dataSource="data_list"
        :columns="columns"
        :rowKey="x=>x.UID"
        :pagination="false"
        size="small"
      >
        <span slot="action" slot-scope="text,record">
          <a-button-group size="small">
            <a-button type="dashed" @click="()=>{show_station_manage(record)}">管理站点</a-button>
            <a-dropdown :trigger="['click']">
              <a-button type="dashed" icon="ellipsis" />
              <a-menu slot="overlay">
                <a-menu-item key="edit" @click="show_form(record)">
                  <a-icon type="edit" />
                  <span>修改</span>
                </a-menu-item>
                <a-menu-item key="delete" @click="delete_(record)">
                  <a-icon type="delete" />
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
        <a-form-item label="线路名称">
          <a-input v-model="add_form.data.Name"></a-input>
        </a-form-item>
        <a-form-item label="描述">
          <a-input v-model="add_form.data.Desc"></a-input>
        </a-form-item>
      </a-form>
      <span slot="footer">
        <a-button type="primary" size="small" :loading="add_form.loading" @click="save">保存</a-button>
      </span>
    </a-modal>

    <a-modal v-model="station_manage.show" title="站点管理" :width="1000" :footer="false">
      <metro-station ref="metro_station_com" @closeModal="()=>{station_manage.show=false}" />
    </a-modal>
  </div>
</template>

<script>
import { post_form } from '@/api/ajax'
import { mixin } from '@/utils/mixin'
import metro_station from './metro_station'

export default {
  components: {
    'metro-station': metro_station
  },
  mixins: [mixin],
  data() {
    return {
      loading: false,
      data_list: [],
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
      station_manage: {
        show: false
      }
    }
  },
  mounted() {
    this.load_all()
  },
  methods: {
    show_station_manage(item) {
      this.station_manage.show = true
      this.$nextTick(() => {
        //console.log(this.$refs.metro_station_com)
        this.$refs.metro_station_com.load_data(item.UID)
      })
    },
    show_form(data) {
      this.add_form.data = {}
      if (data) {
        this.add_form.data = this.copy_data(data)
      }
      this.add_form.show = true
    },
    load_all() {
      this.loading = true
      post_form(this, '/api/metro-ad/admin/MetroLine/QueryAll', {})
        .then(res => {
          var data = res.Data
          this.data_list = data
        })
        .finally(() => {
          this.loading = false
        })
    },
    save() {
      this.add_form.loading = true
      post_form(this, '/api/metro-ad/admin/MetroLine/Save', {
        data: JSON.stringify(this.add_form.data)
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
      post_form(this, '/api/metro-ad/admin/MetroLine/Delete', {
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