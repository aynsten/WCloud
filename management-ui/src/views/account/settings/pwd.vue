<template>
  <div>
    <a-form :form="form">
      <a-form-item label="旧密码">
        <a-input type="password" v-model="form_data.old_pwd" />
      </a-form-item>
      <a-form-item label="新密码">
        <a-input type="password" v-model="form_data.pwd" />
      </a-form-item>
      <a-form-item label="再输一遍">
        <a-input type="password" v-model="form_data.repeat" />
      </a-form-item>
      <a-form-item>
        <a-button type="danger" size="small" @click="check" :disabled="!submitable" icon="lock">修改密码</a-button>
      </a-form-item>
    </a-form>
  </div>
</template>

<script>
import { post_form } from '@/api/ajax'

export default {
  data() {
    return {
      form: this.$form.createForm(this),
      form_data: {
        old_pwd: '',
        pwd: '',
        repeat: ''
      }
    }
  },
  methods: {
    check() {
      if (!confirm('确定修改密码？')) {
        return
      }
      var _this = this
      post_form(this, '/api/member/admin/Account/ChangePwd', this.form_data).then(res => {
        if (!res.Success) {
          alert(res.ErrorMsg)
        } else {
          alert('修改成功')
        }
      })
    }
  },
  computed: {
    submitable() {
      return this.form_data.repeat === this.form_data.pwd && (this.form_data.pwd || '').length > 0
    }
  }
}
</script>