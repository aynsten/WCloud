<template>
  <div>
    <a-card size="small" :bordered="false" :loading="loading" v-if="(station_uid||'').length>0">
      <span slot="extra">
        <a-button type="primary" size="small" icon="plus" @click="show_form(null)">添加橱窗</a-button>
      </span>
      <a-table
        :dataSource="data_list"
        :columns="columns"
        :rowKey="x=>x.UID"
        :pagination="false"
        size="small"
      >
        <div slot="img" slot-scope="text,record">
          <span v-for="(x,i) in record.ImageList" :key="i" style="margin-right:5px;">
            <imgpreview :src="x" :size="40" />
          </span>
        </div>
        <div slot="status" slot-scope="text,record">
          <a-badge :status="record.IsActive?'success':'error'" />
        </div>
        <div slot="media" slot-scope="text,record">
          <a-tag>{{media_name(record)}}</a-tag>
        </div>
        <span slot="action" slot-scope="text,record">
          <a-button-group size="small">
            <a-dropdown :trigger="['click']">
              <a-button type="dashed" icon="ellipsis" />
              <a-menu slot="overlay">
                <a-menu-item key="edit" @click="show_form(record)">
                  <a-icon type="edit" />
                  <span>修改</span>
                </a-menu-item>
                <a-menu-item key="delete" @click="delete_ad(record)">
                  <a-icon type="delete" />
                  <span>删除</span>
                </a-menu-item>
              </a-menu>
            </a-dropdown>
          </a-button-group>
        </span>
      </a-table>
    </a-card>
    <a-modal v-model="add_form.show" title="添加" :width="800">
      <a-form>
        <a-form-item label="橱窗名称">
          <a-input v-model="add_form.data.Name"></a-input>
        </a-form-item>
        <a-form-item label="描述">
          <a-input v-model="add_form.data.Desc"></a-input>
        </a-form-item>
        <a-form-item label="媒体类型">
          <a-select v-model="add_form.data.MediaTypeUID" defaultValue="-1" style="width: 120px">
            <a-select-option key="-1" value="-1">请选择</a-select-option>
            <a-select-option
              v-for="(x,i) in add_form.media_types"
              :key="i"
              :value="x.UID"
            >{{x.Name}}</a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="尺寸（厘米）">
          <a-input-group>
            <span>宽：</span>
            <a-input-number
              v-model="add_form.data.Width"
              :min="0"
              :width="150"
              style="margin-right:10px;"
            />
            <span>高：</span>
            <a-input-number v-model="add_form.data.Height" :min="0" :width="150" />
          </a-input-group>
        </a-form-item>
        <a-form-item label="日单价（元）">
          <a-input-number v-model="add_form.data.Price" :min="0" />
        </a-form-item>
        <a-form-item label="示例图">
          <upload :image_list="add_form.data.ImageList" @change="x=>{add_form.data.ImageList=x;}" />
        </a-form-item>
        <a-form-item label="是否启用">
          <a-switch :checked="add_form.data.IsActive" @change="e=>{add_form.data.IsActive=e;}"></a-switch>
        </a-form-item>
      </a-form>
      <span slot="footer">
        <a-button type="primary" size="small" :loading="add_form.loading" @click="save">保存</a-button>
      </span>
    </a-modal>
  </div>
</template>

<script>
import { post_form, post_form_data } from '@/api/ajax'
import { mixin } from '@/utils/mixin'
import upload from '../upload'
import imgpreview from '@/coms/imagepreview'

export default {
  components: {
    upload,
    imgpreview
  },
  mixins: [mixin],
  props: {
    station_uid_x: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      station_uid: '',
      loading: false,
      data_list: [],
      columns: [
        {
          title: '状态',
          scopedSlots: { customRender: 'status' },
          width: '100px'
        },
        {
          dataIndex: 'Name',
          title: '名称',
          width: '100px'
        },
        {
          title: '描述',
          dataIndex: 'Desc'
        },
        {
          title: '示例图',
          scopedSlots: { customRender: 'img' },
          width: '200px'
        },
        {
          title: '媒体类型',
          scopedSlots: { customRender: 'media' },
          width: '150px'
        },
        {
          title: '操作',
          scopedSlots: { customRender: 'action' },
          width: '100px'
        }
      ],
      add_form: {
        show: false,
        loading: false,
        media_types: [],
        data: {}
      }
    }
  },
  mounted() {
    //this.station_uid = this.station_uid_x
    //this.load_all()
    this.load_media_type()
  },
  methods: {
    show_form(item) {
      this.add_form.data = {
        Name: '',
        Desc: '',
        MediaTypeUID: '-1',
        Height: 0,
        Width: 0,
        Price: 0,
        PriceInCent: 0,
        IsActive: true,
        ImageList: []
      }
      if (item) {
        this.add_form.data = this.copy_data(item)
      }
      this.add_form.show = true
    },
    load_all() {
      if ((this.station_uid || '').length <= 0) {
        return
      }
      this.loading = true
      post_form(this, '/api/metro-ad/admin/MetroAdWindow/QueryByStation', {
        station_uid: this.station_uid
      })
        .then(res => {
          var data = res.Data
          data.forEach(x => {
            x.Price = x.PriceInCent / 100
            x.IsActive = x.IsActive === 1
          })
          this.data_list = data
        })
        .finally(() => {
          this.loading = false
        })
    },
    load_data(metro_station_uid) {
      this.station_uid = metro_station_uid
      console.log(metro_station_uid)
      this.load_all()
    },
    save() {
      this.add_form.loading = true

      var save_data = this.copy_data(this.add_form.data)
      save_data.IsActive = save_data.IsActive ? 1 : 0
      save_data.PriceInCent = save_data.Price * 100

      post_form(this, '/api/metro-ad/admin/MetroAdWindow/Save', {
        data: JSON.stringify({
          ...save_data,
          MetroStationUID: this.station_uid
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
    delete_ad(item) {
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
    },
    load_media_type() {
      post_form(this, '/api/metro-ad/admin/MetroMediaType/QueryAll', {}).then(res => {
        this.add_form.media_types = res.Data
      })
    },
    media_name(item) {
      var res = this.add_form.media_types.filter(x => x.UID === item.MediaTypeUID)
      if (res.length > 0) {
        return res[0].Name
      } else {
        return '--'
      }
    }
  }
}
</script>