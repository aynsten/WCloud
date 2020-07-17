<template>
  <div>
    <a-spin :spinning="loading">
      <a-card title="审批/会签" size="small" style="margin-bottom:10px;">
        <a-card-grid v-for="(item,index) in flows" :key="index" :style="style">
          <a-button type="link" size="small" block @click="create_flow(item)">{{item.Name}}</a-button>
        </a-card-grid>
      </a-card>
      <a-card title="TODO" size="small" style="margin-bottom:10px;">
        <a-card-grid style="width:25%;textAlign:'center'">出差</a-card-grid>
        <a-card-grid style="width:25%;textAlign:'center'">外出</a-card-grid>
        <a-card-grid style="width:25%;textAlign:'center'">用车</a-card-grid>
        <a-card-grid style="width:25%;textAlign:'center'">报销</a-card-grid>
        <a-card-grid style="width:25%;textAlign:'center'">申请立项</a-card-grid>
        <a-card-grid style="width:25%;textAlign:'center'">采购</a-card-grid>
        <a-card-grid style="width:25%;textAlign:'center'">离职</a-card-grid>
      </a-card>
    </a-spin>
    <a-drawer
      title="Create a new account"
      :width="1200"
      @close="onClose"
      :visible="form.show"
      :wrapStyle="{height: 'calc(100% - 108px)',overflow: 'auto',paddingBottom: '108px'}"
    >
      <a-row :gutter="30">
        <a-col :span="16">
          <leaveForm v-if="form.data.FormType==='leave'" />
          <a-alert v-else message="无匹配表单"></a-alert>
        </a-col>
        <a-col :span="8">
          <a-card :bordered="false" style="background-color:rgb(240,240,240)">
            <a-timeline>
              <a-timeline-item color="green">Create a services site 2015-09-01</a-timeline-item>
              <a-timeline-item color="green">
                <a-card size="small" :bordered="false">leader同意了你的请假</a-card>
              </a-timeline-item>
              <a-timeline-item color="red">
                <p>Solve initial network problems 1</p>
                <p>Solve initial network problems 2</p>
                <p>Solve initial network problems 3 2015-09-01</p>
              </a-timeline-item>
              <a-timeline-item>
                <p>Technical testing 1</p>
                <p>Technical testing 2</p>
                <p>Technical testing 3 2015-09-01</p>
              </a-timeline-item>
            </a-timeline>
          </a-card>
        </a-col>
      </a-row>
      <div
        :style="{
          position: 'absolute',
          left: 0,
          bottom: 0,
          width: '100%',
          borderTop: '1px solid #e9e9e9',
          padding: '10px 16px',
          background: '#fff',
          textAlign: 'right',
        }"
      >
        <a-button :style="{marginRight: '8px'}" @click="onClose">Cancel</a-button>
        <a-button @click="onClose" type="primary">Submit</a-button>
      </div>
    </a-drawer>
  </div>
</template>

<script>
import { post_form } from '@/api/ajax'
import leaveForm from './form/leave'

export default {
  components: {
    leaveForm
  },
  data() {
    return {
      style: "width:25%;textAlign:'center'",
      loading: false,
      flows: [],
      form: {
        show: false,
        data: {}
      }
    }
  },
  methods: {
    create_flow(flow) {
      this.form.data = flow
      this.form.show = true
    },
    onClose() {
      this.form.show = false
    },
    load_flow() {
      this.loading = true
      post_form(this, '/api/erp/admin/Flow/Query', {})
        .then(res => {
          this.flows = res.Data
        })
        .finally(() => {
          this.loading = false
        })
    }
  },
  created() {
    this.load_flow()
  }
}
</script>