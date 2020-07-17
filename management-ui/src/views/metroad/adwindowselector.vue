<template>
  <div>
    <a-card size="small">
      <span slot="extra">
        <a-button type="dashed" size="small" @click="()=>{show=true}">选择</a-button>
      </span>
      <div v-if="not_empty(selected_models)">
        <a-tag v-for="(x,i) in selected_models" :key="i" color="green">{{x.Name}}</a-tag>
      </div>
      <a-alert v-else message="请选择"></a-alert>
    </a-card>
    <a-modal v-model="show" title="select" :width="800">
      <a-spin :spinning="loading">
        <a-tabs :defaultActiveKey="0">
          <a-tab-pane v-for="(x,i) in data" :tab="x.Name" :key="i">
            <div v-for="(m,j) in x.Stations" :key="j">
              <a-card
                v-if="not_empty(m.AdWindows)"
                :title="m.Name"
                size="small"
                style="margin-bottom:10px;"
              >
                <a-row>
                  <a-col :span="8" v-for="(b,u) in m.AdWindows" :key="u">
                    <a-checkbox :checked="b.ck" @change="e=>{change_check(e,b)}">{{b.Name}}</a-checkbox>
                  </a-col>
                </a-row>
              </a-card>
            </div>
          </a-tab-pane>
        </a-tabs>
      </a-spin>
      <span slot="footer">
        <a-button type="primary" size="small" @click="()=>{show=false}">确定</a-button>
      </span>
    </a-modal>
  </div>
</template>

<script>
import { post_form } from '@/api/ajax'
import { mixin } from '@/utils/mixin'

export default {
  mixins: [mixin],
  props: {
    selected: {
      type: Array,
      default: () => []
    }
  },
  data() {
    return {
      show: false,
      loading: false,
      data: [],
      selected_uids: []
    }
  },
  computed: {
    selected_models() {
      return this.flatData(this.data).filter(x => x.ck)
    }
  },
  mounted() {
    this.set_data(this.selected)
    this.load_data()
  },
  watch: {
    selected(val) {
      this.set_data(val)
      this.set_selected(this.data)
    }
  },
  methods: {
    change_check(e, m) {
      m.ck = e.target.checked
      var uids = this.flatData(this.data)
        .filter(x => x.ck)
        .map(x => x.UID)

      this.out_data(uids)
    },
    set_data(data) {
      this.selected_uids = data || []
    },
    out_data(data) {
      this.$emit('change', data)
      console.log(data)
    },
    flatData(data) {
      return data.flatMap(x => x.Stations).flatMap(x => x.AdWindows)
    },
    set_selected(data) {
      this.flatData(data).forEach(x => {
        x.ck = this.selected_uids.indexOf(x.UID) >= 0
      })
      return data
    },
    load_data() {
      this.loading = true
      post_form(this, '/api/metro-ad/admin/MetroAdWindow/AllAdWindows', {})
        .then(res => {
          var data = res.Data
          data = this.set_selected(data)
          this.data = data
        })
        .finally(() => {
          this.loading = false
        })
    }
  }
}
</script>